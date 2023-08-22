/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using AddInSideViews;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TvpMain.Check;
using TvpMain.Import;
using TvpMain.Project;
using TvpMain.Properties;
using TvpMain.Punctuation;
using TvpMain.Reference;
using TvpMain.Result;
using TvpMain.Text;
using TvpMain.Util;
using static System.String;
using static TvpMain.Check.CheckAndFixItem;

namespace TvpMain.Forms
{
    /// <summary>
    /// This form displays information about the checks which have been run against a project
    /// as well as options for viewing and fixing any found errors.
    /// </summary>
    public partial class CheckResultsForm : Form
    {
        private const string DENY_BUTTON_TEXT = "Deny";
        private const string UNDENY_BUTTON_TEXT = "Un-Deny";

        // A list of <c>CheckResultItem</c>s which have been denied
        private List<int> _denied;

        /// <summary>
        /// This index indicates to the progress form that the project scope checks are being run.
        /// </summary>
        public const int ALL_PROJECTS_INDEX = -1;

        /// <summary>
        /// This index indicates a scope for BCV is not applicable.
        /// </summary>
        public const int SCOPE_NOT_APPLICABLE = -1;

        /// <summary>
        /// This index indicates no row is selected
        /// </summary>
        public const int DEFAULT_ROW_NOT_SELECTED = -1;

        /// <summary>
        /// Lock for updating check results dictionary.
        /// </summary>
        private readonly object _checkResultslock = new object();

        /// <summary>
        /// Cancellation token source.
        /// </summary>
        private CancellationTokenSource _runCancellationTokenSource;

        /// <summary>
        /// Current run's task semaphore.
        /// </summary>
        private SemaphoreSlim _runSemaphore;

        /// <summary>
        /// The Paratext plugin host.
        /// </summary>
        private IHost Host { get; }

        /// <summary>
        /// Active project name.
        /// </summary>
        private string ActiveProjectName { get; }

        /// <summary>
        /// Provides project setting & metadata access.
        /// </summary>
        private ProjectManager ProjectManager { get; }

        /// <summary>
        /// Provides access to results.
        /// </summary>
        private readonly ResultManager _resultManager;

        private List<IRunnable> Runnables { get; }

        /// <summary>
        /// A collection of the <c>CheckAndFixItem</c>s to run against the content.
        /// </summary>
        private List<CheckAndFixItem> ChecksToRun { get; } = new List<CheckAndFixItem>();

        /// <summary>
        /// The checks organized by check scope.
        /// </summary>
        private Dictionary<CheckScope, List<CheckAndFixItem>> ChecksByScope { get; set; }

        /// <summary>
        /// The context of what we're checking against.
        /// </summary>
        private CheckRunContext CheckRunContext { get; }

        /// <summary>
        /// The Check and Fix executor.
        /// </summary>
        private CheckAndFixRunner CheckRunner { get; set; }

        /// <summary>
        /// The <c>ImportManager</c> used to read in the scripture text.
        /// </summary>
        private ImportManager ImportManager { get; set; }

        /// <summary>
        /// Progress of current run (1-based indexing).
        /// </summary>
        private int _runBookCtr;

        /// <summary>
        /// The collection of <c>CheckResultItem</c>s mapped by the associated <c>CheckAndFixItem</c>.
        /// </summary>
        private Dictionary<CheckAndFixItem, List<CheckResultItem>> CheckResults { get; } = new Dictionary<CheckAndFixItem, List<CheckResultItem>>();

        /// <summary>
        /// The collection of <c>CheckAndFixItem</c>s that are marked broken and shouldn't keep attempting to execute<c>CheckAndFixItem</c>.
        /// </summary>
        private List<CheckAndFixItem> BrokenChecks { get; } = new List<CheckAndFixItem>();

        /// <summary>
        /// The collection for holding project content in the correct book order.
        /// </summary>
        private ConcurrentDictionary<int, StringBuilder> ProjectSb { get; set; } = new ConcurrentDictionary<int, StringBuilder>();

        /// <summary>
        /// Check updated event handler.
        /// </summary>
        public event EventHandler<CheckUpdatedArgs> CheckUpdated;

        /// <summary>
        /// Reusable progress form.
        /// </summary>
        private readonly ProgressForm _progressForm;

        /// <summary>
        /// The list of selected books to check
        /// </summary>
        private BookNameItem[] SelectedBooks { get; set; }

        /// <summary>
        /// This form displays information about the checks which have been run against a project
        /// as well as options for viewing and fixing any found errors.
        /// </summary>
        /// <param name="host">The Paratext Host object.</param>
        /// <param name="activeProjectName">The name of the current project</param>
        /// <param name="projectManager">The project manager, to get to book information</param>
        /// <param name="selectedBooks">The selected books when the filter was run</param
        /// <param name="checksToRun">The list of checks to run against the content.</param>
        /// <param name="checkRunContext">The context of what Bible content we're checking against.</param>
        public CheckResultsForm(
            IHost host,
            string activeProjectName,
            ProjectManager projectManager,
            BookNameItem[] selectedBooks,
            List<IRunnable> runnables,
            CheckRunContext checkRunContext)
        {
            // validate inputs
            Host = host ?? throw new ArgumentNullException(nameof(host));
            if (IsNullOrEmpty(activeProjectName))
            {
                throw new ArgumentNullException(nameof(activeProjectName));
            }

            ActiveProjectName = activeProjectName;
            ProjectManager = projectManager ?? throw new ArgumentNullException(nameof(projectManager));
            SelectedBooks = selectedBooks ?? throw new ArgumentNullException(nameof(selectedBooks));
            Runnables = runnables ?? throw new ArgumentNullException(nameof(runnables));
            foreach (IRunnable runnable in runnables)
            {
                ChecksToRun.AddRange(runnable.Checks);
            }
            CheckRunContext = checkRunContext ?? throw new ArgumentNullException(nameof(checkRunContext));

            _progressForm = InitializeProgressForm();
            CheckUpdated += OnCheckUpdated;

            // initialize the components
            InitializeComponent();
            SetUpResultsTable();

            _resultManager = new ResultManager(Host, ActiveProjectName);
            _resultManager.ScheduleLoadBooks(ProjectManager.PresentBookNums);
        }

        /// <summary>
        /// Sets up the results table headers and sort behavior.
        /// </summary>
        private void SetUpResultsTable()
        {
            // set up reference column visibility ,
            // depending on the language we're using
            if (ProjectManager.IsEnglishProject)
            {
                IssuesDataGridView.Columns[1].Visible = false;
                IssuesDataGridView.Columns[2].HeaderText = @"Reference";
                IssuesDataGridView.Columns[2].Width *= 2;
            }
            else
            {
                IssuesDataGridView.Columns[1].Visible = true;
                IssuesDataGridView.Columns[2].HeaderText = @"Ref";
            }

            // set header style to middle/center, as with the cell text
            IssuesDataGridView.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            IssuesDataGridView.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // set up reference column sorting
            IssuesDataGridView.SortCompare += (sender, args) =>
            {
                // either reference column = sort by book, chapter, and verse
                if (args.Column.Index == 1
                    || args.Column.Index == 2)
                {
                    var resultItem1 = (CheckResultItem)IssuesDataGridView.Rows[args.RowIndex1].Tag;
                    var resultItem2 = (CheckResultItem)IssuesDataGridView.Rows[args.RowIndex2].Tag;

                    args.SortResult = CheckResultItem.CompareByLocation(
                        resultItem1, resultItem2);

                    // if references are equivalent, further sort by row index to ensure sorting on
                    // either project or english reference columns produces consistent results
                    // (won't be stable with respect to each other, otherwise).
                    if (args.SortResult == 0)
                    {
                        args.SortResult = args.RowIndex1.CompareTo(args.RowIndex2);
                    }
                }
                else // else, sort by cell text
                {
                    args.SortResult = CompareOrdinal(
                        (args.CellValue1 ?? Empty).ToString(),
                        (args.CellValue2 ?? Empty).ToString());
                }
                args.Handled = true;
            };

            IssuesDataGridView.Sort(
                ProjectManager.IsEnglishProject ? IssuesDataGridView.Columns[2] : IssuesDataGridView.Columns[1],
                ListSortDirection.Ascending);
        }

        /// <summary>
        /// Creates a <c>ProgressForm</c> with default values
        /// </summary>
        private ProgressForm InitializeProgressForm()
        {
            var progressForm = new ProgressForm { StartPosition = FormStartPosition.CenterParent };
            progressForm.Cancelled += OnProgressFormCancelled;
            return progressForm;
        }

        /// <summary>
        /// Set the default set of books, all of them
        /// </summary>
        private void SetSelectedBooks()
        {
            var selectedBooksString = BookSelection.stringFromSelectedBooks(SelectedBooks);
            bookFilterTextBox.Text = selectedBooksString;
        }

        /// <summary>
        /// This method loads the denied <c>CheckResultItem</c>s from the project.
        /// </summary>
        private void LoadDeniedResults()
        {
            _denied = HostUtil.Instance.GetProjectDeniedResults(ActiveProjectName);
        }

        /// <summary>
        /// This method saves the denied <c>CheckResultItem</c>s to the project.
        /// </summary>
        private void SaveDeniedResults()
        {
            HostUtil.Instance.PutProjectDeniedResults(ActiveProjectName, _denied);
        }

        /// <summary>
        /// This function will cancel the execution of the checks.
        /// </summary>
        public void CancelChecks()
        {
            _runCancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// This function notifies event listeners that a book has been updated.
        /// </summary>
        /// <param name="updateBookNum">The number of the book that finished being checked.</param>
        private void OnCheckUpdated(int updateBookNum)
        {
            lock (_checkResultslock)
            {
                // we increased the max number of books to account for the project. We pass -1 as the book num to represent the project.
                _runBookCtr++;
                CheckUpdated?.Invoke(this,
                    new CheckUpdatedArgs(CheckRunContext.Books.Count() + 1, _runBookCtr, updateBookNum));
            }
        }

        /// <summary>
        /// Punctuation check update forwarder.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="updatedArgs">Check update details.</param>
        private void OnCheckUpdated(object sender, CheckUpdatedArgs updatedArgs)
        {
            _progressForm.OnCheckUpdated(updatedArgs);
        }

        /// <summary>
        /// This function will lay the foundations for running the checks, executing them asynchronously, and monitoring them.
        /// </summary>
        public void RunChecks()
        {
            CheckRunContext.Validate();
            if (ChecksToRun.Count <= 0)
            {
                MessageBox.Show(
                    @"No checks provided.",
                    @"Notice...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Hide();
                return;
            }

            // Show the progress bar as we kick off our work.
            ShowProgress();

            // clear the previous results
            CheckResults.Clear();

            // Pre-filter the checks and fixes so we don't have to do it repeatedly later.
            ChecksByScope = new Dictionary<CheckScope, List<CheckAndFixItem>>
            {
                    { CheckScope.PROJECT, new List<CheckAndFixItem>() },
                    { CheckScope.BOOK, new List<CheckAndFixItem>() },
                    { CheckScope.CHAPTER, new List<CheckAndFixItem>() },
                    { CheckScope.VERSE, new List<CheckAndFixItem>() },
                };

            ChecksToRun.Aggregate(ChecksByScope, (acc, check) =>
            {
                acc[check.Scope].Add(check);
                return acc;
            });

            // set up semaphore and cancellation token to control execution and termination
            RecycleRunEntities(true);

            // content builders for performing scoped checks.
            var books = CheckRunContext.Books;

            // set up semaphore for parallelism control, results set, and task list
            var taskList = new List<Task>();
            taskList.AddRange(books.Select(book => RunBookCheckTask(book.BookNum)));

            try
            {
                var workThread = new Thread(() =>
                {
                    try
                    {
                        Task.WaitAll(taskList.ToArray(), _runCancellationTokenSource.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        // Ignore (can occur w/cancel).
                    }
                    catch (Exception ex)
                    {
                        var messageText =
                            $"Error: Can't check project: \"{CheckRunContext.Project}\" (error: {ex.Message}).";

                        var runEx = new TextCheckException(messageText, ex);
                        HostUtil.Instance.ReportError(messageText, runEx);
                    }
                })
                { IsBackground = true };
                workThread.Start();

                // busy-wait until helper thread is done,
                // keeping the UI responsive w/DoEvents()
                while (workThread.IsAlive)
                {
                    Application.DoEvents();
                    Thread.Sleep(MainConsts.CHECK_EVENTS_DELAY_IN_MSEC);

                    if (_runCancellationTokenSource.IsCancellationRequested)
                    {
                        return;
                    }
                }

                // once all the books are checked, let's check the accumulated project content (ordered by book).
                var sortedKeys = ProjectSb.Keys.ToList();
                sortedKeys.Sort();

                var finalSb = new StringBuilder();
                foreach (var key in sortedKeys)
                {
                    finalSb.Append(ProjectSb[key]);
                }

                ExecuteChecksAndStoreResults(finalSb.ToString(), ChecksByScope[CheckScope.PROJECT], SCOPE_NOT_APPLICABLE, SCOPE_NOT_APPLICABLE, SCOPE_NOT_APPLICABLE);
                OnCheckUpdated(ALL_PROJECTS_INDEX);
            }
            finally
            {
                HideProgress();
            }

            // populate the checks results table
            PopulateChecksDataGridView();
        }

        /// <summary>
        /// This function will execute the specified checks against the provided content and store the results.
        /// </summary>
        /// <param name="content">The content to check against.</param>
        /// <param name="checksToRun">The list of checks to assess the content.</param>
        /// <param name="book">The current bookNum (1-based).</param>
        /// <param name="chapter">The current chapterNum (1-based).</param>
        /// <param name="verse">The current verseNum (1-based).</param>
        private void ExecuteChecksAndStoreResults(string content, List<CheckAndFixItem> checksToRun, int book, int chapter, int verse)
        {
            // check the content with every specified check
            checksToRun.ForEach(check =>
            {
                // this function only supports the v2 javascript/regex checks
                // we also skip checks that are deemed broken
                if (IsV1Check(check) || BrokenChecks.Contains(check))
                {
                    return;
                }

                try
                {
                    var results = CheckRunner.ExecCheckAndFix(content, check);

                    foreach (var item in results)
                    {
                        item.Book = book;
                        item.Chapter = chapter;
                        item.Verse = verse;
                        item.Reference = content;
                    }

                    lock (_checkResultslock)
                    {
                        // add or append results
                        if (CheckResults.ContainsKey(check))
                        {
                            CheckResults[check].AddRange(results);
                        }
                        else
                        {
                            CheckResults.Add(check, results);
                        }
                    }
                }
                catch (CheckAndFixException checkEx)
                {
                    // report and track broken checks
                    HostUtil.Instance.LogLine(checkEx.Message, true);
                    BrokenChecks.Add(check);
                }
            });
        }

        /// <summary>
        /// This function will execute the specified V1 check
        /// </summary>
        /// <param name="content">The content to check against (required).</param>
        /// <param name="textCheck">The check used to assess the content (required).</param>
        /// <param name="checkItem">Check item (required).</param>
        /// <param name="bookNum">The current bookNum (1-based).</param>
        /// <param name="chapterNum">The current chapterNum (1-based).</param>
        /// <param name="verseNum">The current verseNum (1-based).</param>
        private void ExecuteV1CheckAndStoreResults(
            string content, ITextCheck textCheck, CheckAndFixItem checkItem,
            int bookNum, int chapterNum, int verseNum)
        {
            // check the content with every specified check
            var results = new List<CheckResultItem>();
            var textCheckRunner = new TextCheckRunner(Host, ActiveProjectName, ProjectManager, ImportManager, _resultManager);

            IEnumerable<ITextCheck> allChecks = new List<ITextCheck>
            {
                textCheck
            };

            // by default, select all parts to check
            ISet<PartContext> checkContexts = new HashSet<PartContext>
            {
                PartContext.MainText,
                PartContext.Introductions,
                PartContext.Outlines,
                PartContext.NoteOrReference
            };

            if (textCheckRunner.RunChecks(
                CheckArea.CurrentBook,
                allChecks,
                checkContexts,
                false,
                bookNum,
                out var nextResults))
            {
                IList<ResultItem> allResultItems = nextResults.ResultItems.ToImmutableList();
                // translate the v1 result items to v2 result items that are compatible with displaying
                foreach (var result in allResultItems)
                {
                    var checkResultItem = new CheckResultItem(result.ErrorText,
                        result.MatchText, result.MatchStart,
                        result.CheckType, result.ResultTypeCode)
                    {
                        Book = bookNum,
                        Chapter = chapterNum,
                        Verse = verseNum,
                        Reference = result.VersePart.ProjectVerse.VerseText
                    };
                    results.Add(checkResultItem);
                }
            }

            lock (_checkResultslock)
            {
                // add or append results
                if (CheckResults.ContainsKey(checkItem))
                {
                    CheckResults[checkItem].AddRange(results);
                }
                else
                {
                    CheckResults.Add(checkItem, results);
                }
            }
        }

        /// <summary>
        /// Helper function for detererming if a check is a legacy V1 TVP check
        /// </summary>
        /// <param name="check">Check to assess</param>
        /// <returns>true, if a v1 check; false, otherwise.</returns>
        private bool IsV1Check(CheckAndFixItem check)
        {
            return MainConsts.V1_CHECKS.Contains(check.Id);
        }

        /// <summary>
        /// This function creates and runs a check against a given book number.
        /// </summary>
        /// <param name="inputBookNum">Book number (1-based).</param>
        private Task RunBookCheckTask(
            int inputBookNum)
        {
            return Task.Run(() =>
            {
                // wait to get started
                _runSemaphore.Wait();

                if (!ProjectSb.TryAdd(inputBookNum, new StringBuilder()))
                {
                    throw new ArgumentException($"There's already an entry for book {inputBookNum} in {nameof(ProjectSb)}");
                }
                var bookSb = new StringBuilder();

                // track where we are for error reporting
                var currBookNum = inputBookNum;
                var currChapterNum = 0;
                var currVerseNum = 0;

                try
                {
                    // get utility items
                    var versificationName = Host.GetProjectVersificationName(CheckRunContext.Project);

                    // needed to track empty chapters
                    var emptyVerseCtr = 0;

                    // determine chapter range using check area and user's location in Paratext
                    var minChapter = (CheckRunContext.CheckScope == CheckScope.CHAPTER)
                            ? CheckRunContext.Chapters.Min()
                            : 1;
                    var maxChapter = (CheckRunContext.CheckScope == CheckScope.CHAPTER)
                        ? CheckRunContext.Chapters.Max()
                        : Host.GetLastChapter(currBookNum, versificationName);

                    // iterate chapters
                    for (var chapterNum = minChapter;
                            chapterNum <= maxChapter;
                            chapterNum++)
                    {
                        var chapterSb = new StringBuilder();
                        currChapterNum = chapterNum;

                        // iterate verses
                        var lastVerseNum = Host.GetLastVerse(currBookNum, chapterNum, versificationName);
                        for (var verseNum = 0; // verse 0 = intro text
                            verseNum <= lastVerseNum;
                            verseNum++)
                        {
                            currVerseNum = verseNum;

                            try
                            {
                                // Check if the parallel.ForEach has been cancelled. The verse is the smallest entitity granularity
                                _runCancellationTokenSource.Token.ThrowIfCancellationRequested();

                                var verseLocation = new VerseLocation(currBookNum, chapterNum, verseNum);
                                var verseText = ImportManager.Extract(verseLocation);

                                // empty text = consecutive check, in case we're looking at an empty chapter
                                if (IsNullOrWhiteSpace(verseText))
                                {
                                    emptyVerseCtr++;
                                    if (emptyVerseCtr > MainConsts.MAX_CONSECUTIVE_EMPTY_VERSES)
                                    {
                                        break; // no beginning text = empty chapter (skip)
                                    }
                                }
                                // else, take apart text
                                emptyVerseCtr = 0;

                                // build the scoped strings
                                ProjectSb[inputBookNum].AppendLine(verseText);
                                bookSb.AppendLine(verseText);
                                chapterSb.AppendLine(verseText);

                                // check the verse with verse checks
                                ExecuteChecksAndStoreResults(verseText, ChecksByScope[CheckScope.VERSE], currBookNum, currChapterNum, currVerseNum);
                            }
                            catch (ArgumentException)
                            {
                                // arg exceptions occur when verses are missing,
                                // which they can be for given translations (ignore and move on)
                            }
                        }

                        // check the chapter with chapter checks
                        ExecuteChecksAndStoreResults(chapterSb.ToString(), ChecksByScope[CheckScope.CHAPTER], currBookNum, currChapterNum, SCOPE_NOT_APPLICABLE);
                    }

                    // check the book with book checks
                    ExecuteChecksAndStoreResults(bookSb.ToString(), ChecksByScope[CheckScope.BOOK], currBookNum, SCOPE_NOT_APPLICABLE, SCOPE_NOT_APPLICABLE);

                    // These calls do not need the text passed it, the V1 checks get the text a different way. They are actually verse scope checks, not book scope. But, they
                    // must be run at the book level.
                    var referenceCheck = ChecksByScope[CheckScope.VERSE].Find(i => i.Id == MainConsts.V1_SCRIPTURE_REFERENCE_CHECK_GUID);
                    if (referenceCheck != null)
                    {
                        ExecuteV1CheckAndStoreResults("", new ScriptureReferenceCheck(ProjectManager), referenceCheck, currBookNum, SCOPE_NOT_APPLICABLE, SCOPE_NOT_APPLICABLE);
                    }

                    var punctuationCheck = ChecksByScope[CheckScope.VERSE].Find(i => i.Id == MainConsts.V1_PUNCTUATION_CHECK_GUID);
                    if (punctuationCheck != null)
                    {
                        ExecuteV1CheckAndStoreResults("", new MissingSentencePunctuationCheck(ProjectManager), punctuationCheck, currBookNum, SCOPE_NOT_APPLICABLE, SCOPE_NOT_APPLICABLE);
                    }

                    OnCheckUpdated(inputBookNum);
                }
                catch (OperationCanceledException)
                {
                    // A cancel occurred. We can ignore.
                }
                catch (Exception ex)
                {
                    var messageText =
                        $"Error: Can't check location: {currBookNum}.{currChapterNum}.{currVerseNum} in project: \"{CheckRunContext.Project}\" (error: {ex.Message}).";

                    HostUtil.Instance.ReportError(messageText, ex);
                }
                finally
                {
                    _runSemaphore.Release();
                }
            });
        }

        public virtual new void Dispose()
        {
            RecycleRunEntities(false);
            base.Dispose();
        }

        /// <summary>
        /// Populate the results into the UI results data grid.
        /// </summary>
        protected void PopulateChecksDataGridView()
        {
            var currentSelectedRowIndex = 0;

            if (ChecksDataGridView.CurrentCell != null)
            {
                currentSelectedRowIndex = ChecksDataGridView.SelectedRows[0].Index;
            }

            ChecksDataGridView.Rows.Clear();

            foreach (var result in CheckResults)
            {
                if (!IfCheckFixItemIsNotFiltered(result.Key))
                {
                    continue;
                }

                var filteredResultItems = FilterResults(result.Value);
                var rowIndex = ChecksDataGridView.Rows.Add(false, result.Key.Name.Trim(), result.Key.Description.Trim(), filteredResultItems.Count);

                ChecksDataGridView.Rows[rowIndex].Tag = new KeyValuePair<CheckAndFixItem, List<CheckResultItem>>(result.Key, filteredResultItems);
            }

            // ensure that there are rows to select
            if (ChecksDataGridView.Rows.Count > 0)
            {
                ChecksDataGridView.Rows[currentSelectedRowIndex].Selected = true;
            }

            // Send in the currently selected row. This is needed because this value is incorrect from the control
            // itself until the UI thread has actually updated the control and the event has been triggered.
            // However, we can't rely on that event to update the issues list because it dosn't trigger in every case.
            PopulateIssuesDataGridView(currentSelectedRowIndex);
        }

        /// <summary>
        /// Checks if the text filter matches anything in the name or description of the checkFixItem
        /// </summary>
        /// <param name="checkAndFixItem">The item to be checked for matches</param>
        /// <returns>If the item has a match and is not filtered out</returns>
        private bool IfCheckFixItemIsNotFiltered(CheckAndFixItem checkAndFixItem)
        {
            if (IsNullOrEmpty(filterTextBox.Text))
            {
                return true;
            }

            return checkAndFixItem.Name.IndexOf(filterTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                checkAndFixItem.Description.IndexOf(filterTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Checks the given result items to see if the result is from one of the selected books
        /// </summary>
        /// <param name="results">The results to review</param>
        /// <returns>A filtered set of results that match for the selected books</returns>
        protected List<CheckResultItem> FilterResults(IEnumerable<CheckResultItem> results)
        {
            var filteredItems = new List<CheckResultItem>();

            foreach (var item in results)
            {
                foreach (var bookNameItem in SelectedBooks)
                {
                    if (bookNameItem.BookNum != item.Book)
                    {
                        continue;
                    }

                    if (ShowDeniedCheckbox.Checked)
                    {
                        filteredItems.Add(item);
                    }
                    else
                    {
                        if (_denied.Contains(item.GetHashCode()))
                        {
                            item.ResultState = CheckResultState.Ignored;
                        }
                        else
                        {
                            item.ResultState = CheckResultState.Found;
                            filteredItems.Add(item);
                        }
                    }
                }
            }

            return filteredItems;
        }

        /// <summary>
        /// The function will close the results dialog.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
            CancelChecks();
        }

        /// <summary>
        /// Recycle (dispose and optionally re-create) run semaphore and cancellation token.
        /// </summary>
        /// <param name="isCreateNew">True to create a new one, false to only dispose any existing one.</param>
        private void RecycleRunEntities(bool isCreateNew)
        {
            _runSemaphore?.Dispose();
            _runSemaphore = isCreateNew
                ? new SemaphoreSlim(MainConsts.MAX_CHECK_THREADS)
                : null;

            _runCancellationTokenSource?.Dispose();
            _runCancellationTokenSource = isCreateNew
                ? new CancellationTokenSource()
                : null;
        }

        /// <summary>
        /// Show progress form and reset contents.
        /// </summary>
        private void ShowProgress()
        {
            _progressForm.ResetForm();

            Enabled = false;
            _progressForm.Show(this);
        }

        /// <summary>
        /// Hide progress form and reactivate this form.
        /// </summary>
        private void HideProgress()
        {
            _progressForm.Hide();

            Enabled = true;
            Activate();
        }

        /// <summary>
        /// The progress form cancelled observer.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void OnProgressFormCancelled(object sender, EventArgs e)
        {
            CancelChecks();
            HideProgress();
        }

        /// <summary>
        /// The event handler for clicking the License button. This function will start the License form.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void LicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUtil.StartLicenseForm();
        }

        /// <summary>
        /// When we want to clear the check filter
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void ClearCheckFilterButton_Click(object sender, EventArgs e)
        {
            filterTextBox.Text = "";
            PopulateChecksDataGridView();
        }

        /// <summary>
        /// Set the books filter back to ALL
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void BookFilterClearButton_Click(object sender, EventArgs e)
        {
            // set the default set of books, all of them
            SelectedBooks = ProjectManager.BookNamesByNum.Values.ToArray();
            var selectedBooksString = BookSelection.stringFromSelectedBooks(SelectedBooks);
            bookFilterTextBox.Text = selectedBooksString;
            PopulateChecksDataGridView();
        }

        /// <summary>
        /// Select, for filtering, a set of books
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void SelectBooksButton_Click(object sender, EventArgs e)
        {
            // bring up book selection dialog, use current selection to initialize
            using var form = new BookSelection(ProjectManager, SelectedBooks)
            {
                StartPosition = FormStartPosition.CenterParent
            };

            var result = form.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                // update which books were selected
                bookFilterTextBox.Text = BookSelection.stringFromSelectedBooks(form.GetSelected());
                PopulateChecksDataGridView();
            }
        }

        /// <summary>
        /// This method handles a click event on the "Deny"/"Un-Deny" button.
        /// </summary>
        /// <param name="sender">The "Deny" button</param>
        /// <param name="e">The event information</param>
        private void Deny_Click(object sender, EventArgs e)
        {
            var item = (CheckResultItem)IssuesDataGridView.CurrentRow.Tag;

            switch (item.ResultState)
            {
                case CheckResultState.Found:
                    {
                        item.ResultState = CheckResultState.Ignored;
                        if (!_denied.Contains(item.GetHashCode()))
                        {
                            _denied.Add(item.GetHashCode());
                        }

                        break;
                    }
                case CheckResultState.Ignored:
                    item.ResultState = CheckResultState.Found;
                    _denied.Remove(item.GetHashCode());
                    break;
            }

            SaveDeniedResults();
            PopulateChecksDataGridView();
            UpdateDenyButton();
        }

        /// <summary>
        /// This method handles checking/un-checking the "Show Denied" checkbox.
        /// </summary>
        /// <param name="sender">The "Show Denied" checkbox</param>
        /// <param name="e">The event information</param>
        private void ShowDenied_CheckedChanged(object sender, EventArgs e)
        {
            PopulateChecksDataGridView();
        }

        /// <summary>
        /// When the filter text changes, trigger filtering
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void FilterTextBox_TextChanged(object sender, EventArgs e)
        {
            PopulateChecksDataGridView();
        }

        /// <summary>
        /// When the selection on this item changes, update the dependent list
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void ChecksDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            // only do updates here when the control is focused. Updates to the UI happen in other ways too.
            if (ChecksDataGridView.Focused)
            {
                PopulateIssuesDataGridView();
            }
        }

        /// <summary>
        /// Update the list of issues in the issues list
        /// </summary>
        private void PopulateIssuesDataGridView(int rowIndexOverride = DEFAULT_ROW_NOT_SELECTED)
        {
            // update list of issues
            if (ChecksDataGridView.SelectedRows.Count > 0 && ChecksDataGridView.SelectedRows[0] != null && ChecksDataGridView.SelectedRows[0].Tag != null)
            {
                // use selected rows[0] instead of current row since they aren't the same thing. Since the control is limited to only a single selection, this is the value we want.
                var currentSelectedRowIndex = rowIndexOverride != DEFAULT_ROW_NOT_SELECTED ? rowIndexOverride : ChecksDataGridView.SelectedRows[0].Index;
                var result = (KeyValuePair<CheckAndFixItem, List<CheckResultItem>>)ChecksDataGridView.Rows[currentSelectedRowIndex].Tag;

                IssuesDataGridView.Rows.Clear();

                foreach (var item in result.Value)
                {
                    var verseLocation = new VerseLocation(item.Book, item.Chapter, item.Verse);
                    var rowIndex = IssuesDataGridView.Rows.Add(
                        GetStatusIcon(item.ResultState),
                        ProjectManager.IsEnglishProject // saves cycles on English projects
                            ? Empty
                            : verseLocation.ToProjectString(ProjectManager),
                        verseLocation.ToString(),
                        item.MatchText);
                    IssuesDataGridView.Rows[rowIndex].Tag = item;
                }

                if (IssuesDataGridView.Rows.Count > 0)
                {
                    IssuesDataGridView.Rows[0].Selected = true;
                }
                PopulateMatchFixTextBoxes();
            }

            ReSortIssuesGrid();
            UpdateDenyButton();
        }

        /// <summary>
        /// Re-sorts the issues grid after update, as needed.
        /// </summary>
        private void ReSortIssuesGrid()
        {
            if (IssuesDataGridView.SortOrder != SortOrder.None)
            {
                IssuesDataGridView.Sort(
                    IssuesDataGridView.SortedColumn,
                    IssuesDataGridView.SortOrder switch
                    {
                        SortOrder.Ascending => ListSortDirection.Ascending,
                        _ => ListSortDirection.Descending
                    });
            }
        }

        /// <summary>
        /// Returns the appropriate icon bitmap for the status of the issue
        /// </summary>
        /// <param name="resultState">The current issue result state</param>
        /// <returns>a bitmap to be used in the column</returns>
        private static Icon GetStatusIcon(CheckResultState resultState)
        {
            return resultState switch
            {
                CheckResultState.Ignored => Resources.x_mark_16,
                CheckResultState.Fixed => Resources.checkmark_16,
                _ => null
            };
        }

        /// <summary>
        /// Navigate the project view to the issue location
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void IssuesDataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (IssuesDataGridView.Rows.Count <= 0 || !(IssuesDataGridView.CurrentRow is { Tag: { } }))
            {
                return;
            }

            // navigate project to BCV
            var item = (CheckResultItem)IssuesDataGridView.CurrentRow.Tag;
            HostUtil.Instance.GotoBcvInGui(ActiveProjectName,
                item.Book == -1 ? 0 : item.Book,
                item.Chapter == -1 ? 0 : item.Chapter,
                item.Verse == -1 ? 0 : item.Verse);
        }

        /// <summary>
        /// Update the match and fix texts and the deny button based on new selection
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void IssuesDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            // only do updates here when the control is focused. Updates to the UI happen in other ways too.
            if (IssuesDataGridView.Focused)
            {
                // update match/fix text boxes
                PopulateMatchFixTextBoxes();
            }

            UpdateDenyButton();
        }

        /// <summary>
        /// Update the text on the deny button based on the currently selected row
        /// </summary>
        private void UpdateDenyButton()
        {
            if (IssuesDataGridView.Rows.Count <= 0 || !(IssuesDataGridView.CurrentRow is { Tag: { } }))
            {
                return;
            }

            var item = (CheckResultItem)IssuesDataGridView.CurrentRow.Tag;
            DenyButton.Text = item.ResultState switch
            {
                CheckResultState.Ignored => UNDENY_BUTTON_TEXT,
                _ => DENY_BUTTON_TEXT
            };
        }

        /// <summary>
        /// Fill in the match/fix text boxes based on current selections
        /// </summary>
        private void PopulateMatchFixTextBoxes()
        {
            if (IssuesDataGridView.Rows.Count > 0 && IssuesDataGridView.CurrentRow is { Tag: { } })
            {
                var item = (CheckResultItem)IssuesDataGridView.CurrentRow.Tag;

                // update match text
                matchTextBox.Text = item.Reference;

                matchTextBox.SelectAll();
                matchTextBox.SelectionBackColor = Color.White;
                matchTextBox.SelectionStart = MinusPrecedingChars(
                    item.Reference,
                    item.MatchStart,
                    Environment.NewLine,
                    1);
                matchTextBox.SelectionLength =
                    MinusPrecedingChars(
                        item.MatchText,
                        item.MatchLength,
                        Environment.NewLine,
                        1);
                matchTextBox.SelectionBackColor = Color.Yellow;
                matchTextBox.ScrollToCaret();
                matchTextBox.Refresh();

                // update fix text
                if (!IsNullOrEmpty(item.FixText))
                {
                    var stringBuilder = new StringBuilder(item.Reference);
                    stringBuilder.Remove(item.MatchStart, item.MatchLength);
                    stringBuilder.Insert(item.MatchStart, item.FixText);

                    var fixReference = stringBuilder.ToString();
                    fixTextBox.Text = fixReference;
                    fixTextBox.SelectAll();
                    fixTextBox.SelectionBackColor = Color.White;

                    fixTextBox.SelectionStart = MinusPrecedingChars(
                        fixReference,
                        item.MatchStart,
                        Environment.NewLine,
                        1);
                    fixTextBox.SelectionLength = MinusPrecedingChars(
                        item.FixText,
                        item.FixText.Length,
                        Environment.NewLine,
                        1);
                    fixTextBox.SelectionBackColor = Color.LightGreen;
                    fixTextBox.ScrollToCaret();
                    fixTextBox.Refresh();

                    fixTextBox.Enabled = true;
                }
                else
                {
                    fixTextBox.Text = "";
                    fixTextBox.Refresh();
                    fixTextBox.Enabled = false;
                }
            }
            else
            {
                matchTextBox.Text = "";
                matchTextBox.Refresh();

                fixTextBox.Text = "";
                fixTextBox.Refresh();
            }
        }

        /// <summary>
        /// Subtract the occurrences of a search char from before an input position.
        ///
        /// Used to deal with rich text control's half-filtering of '\r\n' from input text.
        /// </summary>
        /// <param name="inputText">Text to search (required).</param>
        /// <param name="inputPosition">Position to search up to (0-based).</param>
        /// <param name="searchText">Character to search for.</param>
        /// <param name="matchOffset">Number of characters to subtract per match (e.g., match length, or a constant value).</param>
        /// <returns>Input position minus occurrences of search text before it, times the match offset.</returns>
        private static int MinusPrecedingChars(
            string inputText,
            int inputPosition,
            string searchText,
            int matchOffset)
        {
            if (IsNullOrEmpty(inputText))
            {
                return inputPosition;
            }

            var totalOffset = 0;
            var searchPosition = inputText.IndexOf(
                searchText,
                StringComparison.Ordinal);

            while (searchPosition >= 0
                   && searchPosition < inputPosition)
            {
                totalOffset += matchOffset;
                searchPosition = inputText.IndexOf(
                    searchText, searchPosition + searchText.Length,
                    StringComparison.Ordinal);
            }

            return Math.Max(inputPosition - totalOffset, 0);
        }

        private void CheckResultsForm_Shown(object sender, EventArgs e)
        {
            CheckRunner ??= new CheckAndFixRunner();
            ImportManager ??= new ImportManager(Host, ActiveProjectName);

            // set the status column so that empty doesn't show unfound image
            ((DataGridViewImageColumn)IssuesDataGridView.Columns["statusIconColumn"]).DefaultCellStyle.NullValue = null;
            LoadDeniedResults();
            SetSelectedBooks();

            RunChecks();
        }

        /// <summary>
        /// Opens a link to the support URL from the plugin
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void contactSupportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Call the Process.Start method to open the default browser
            //with a URL:
            Process.Start(MainConsts.SUPPORT_URL);
        }

        /// <summary>
        /// Display the EULA for the plugin.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void LicenseToolStripMenuItem_Click1(object sender, EventArgs e)
        {
            FormUtil.StartLicenseForm();
        }
    }

    /// <summary>
    /// Basic exception for text check errors.
    /// </summary>
    public class TextCheckException : ApplicationException, ISerializable
    {
        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="messageText">Message text (optional, may be null).</param>
        /// <param name="causeEx">Cause exception (optional, may be null).</param>
        public TextCheckException(string messageText, Exception causeEx)
            : base(messageText, causeEx)
        {
        }
    }
}