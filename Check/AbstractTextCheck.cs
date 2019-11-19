﻿using AddInSideViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TvpMain.Data;
using TvpMain.Filter;
using TvpMain.Util;

/*
 * The first validation check in the Translation Validation Plugin framework.
 */
namespace TvpMain.Check
{
    public abstract class AbstractTextCheck : ITextCheck
    {
        private readonly IHost _host;
        private readonly string _activeProjectName;
        private CancellationTokenSource _tokenSource;

        public event EventHandler<int> CheckUpdated;

        public AbstractTextCheck(IHost host, string activeProjectName)
        {
            this._host = host ?? throw new ArgumentNullException(nameof(host));
            this._activeProjectName = activeProjectName ?? throw new ArgumentNullException(nameof(activeProjectName));
        }

        /*
         * 
         */
        private class ExtractorState
        {
            private readonly IScrExtractor _scrExtractor;
            private int _subTotal;

            public IScrExtractor ScrExtractor { get => _scrExtractor; }
            public int SubTotal { get => _subTotal; set => _subTotal = value; }

            public ExtractorState(IScrExtractor extractor)
            {
                this._scrExtractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
                _subTotal = 0;
            }
        }

        public void CancelCheck()
        {
            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
            }
        }

        public CheckResult RunCheck()
        {
            CheckResult checkResult = new CheckResult(_host, _activeProjectName);
            string versificationName = _host.GetProjectVersificationName(_activeProjectName);

            int currProgress = 0;
            _tokenSource = new CancellationTokenSource();

            Thread workThread = new Thread(() =>
            {
                try
                {
                    // Provides multiple threads in order to increase the speed of the Puncutation check.
                    // Use type parameter to make subtotal a long, not an int
                    Parallel.For(1, MainConsts.MAX_BOOK_NUM + 1,
                        new ParallelOptions { MaxDegreeOfParallelism = MainConsts.MAX_CHECK_THREADS, CancellationToken = _tokenSource.Token },
                        () => new ExtractorState(_host.GetScriptureExtractor(_activeProjectName, ExtractorType.USFM)),
                        (bookNum, loopState, extractorState) =>
                      {
                          int currBookNum = bookNum;
                          int currChapterNum = 0;
                          int currVerseNum = 0;

                          try
                          {
                              /*
                              * Handles looping over the entire Bible and storing the results to be indexed.
                              */
                              int lastChapterNum = _host.GetLastChapter(bookNum, versificationName);
                              IList<ResultItem> resultItems = new List<ResultItem>();

                              for (int chapterNum = 1; chapterNum <= lastChapterNum; chapterNum++)
                              {
                                  currChapterNum = chapterNum;
                                  int lastVerseNum = _host.GetLastVerse(bookNum, chapterNum, versificationName);

                                  for (int verseNum = 1; verseNum <= lastVerseNum; verseNum++)
                                  {
                                      if (_tokenSource.IsCancellationRequested)
                                      {
                                          return extractorState;
                                      }

                                      currVerseNum = verseNum;
                                      int coord = bookNum * 1000000;
                                      coord += chapterNum * 1000;
                                      coord += verseNum;

                                      string verseText = null;
                                      try
                                      {
                                          verseText = extractorState.ScrExtractor.Extract(coord, coord);
                                      }
                                      catch (ArgumentException)
                                      {
                                          // arg exceptions occur when verses are missing, 
                                          // which they can be for given translations (ignore and move on)
                                          continue;
                                      }
                                      if (verseText == null)
                                      {
                                          continue;
                                      }

                                      resultItems.Clear();
                                      CheckVerse(bookNum, chapterNum, verseNum, verseText, resultItems);

                                      foreach (ResultItem resultItem in resultItems)
                                      {
                                          checkResult.ResultItems.Enqueue(resultItem);
                                      }
                                  }
                              }
                              extractorState.SubTotal++;

                              lock (this)
                              {
                                  currProgress++;
                                  CheckUpdated?.Invoke(this, currProgress);
                              }
                          }
                          catch (Exception ex)
                          {
                              HostUtil.Instance.ReportError($"Error: Can't check location: {currBookNum}.{currChapterNum}.{currVerseNum}", ex);
                          }
                          return extractorState;
                      },
                        (extractorState) =>
                        {
                            // ignore, at this time
                        });
                }
                catch (OperationCanceledException)
                {
                    // Ignore, at this time
                }
                catch (Exception ex)
                {
                    HostUtil.Instance.ReportError(ex);
                }
            });
            workThread.IsBackground = true;
            workThread.Start();

            int threadSleepInMs = (int)(1000f / (float)MainConsts.CHECK_EVENTS_UPDATE_RATE_IN_FPS);
            while (workThread.IsAlive)
            {
                Application.DoEvents();
                Thread.Sleep(threadSleepInMs);
            }

            if (currProgress >= MainConsts.MAX_BOOK_NUM)
            {
                return checkResult;
            }
            else
            {
                return null;
            }
        }

        protected abstract void CheckVerse(int bookNum, int chapterNum, int verseNum, string verseText, IList<ResultItem> checkResults);
    }
}
