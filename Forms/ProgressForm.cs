﻿using System;
using System.Text;
using System.Windows.Forms;
using TvpMain.Check;
using TvpMain.Text;

/// <summary>
/// Form to provide progress of Translation Validation Checks.
/// </summary>
namespace TvpMain.Forms
{
    /// <summary>
    /// Validation progress form.
    /// </summary>
    public partial class ProgressForm : Form
    {
        /// <summary>
        /// Start time for timer label.
        /// </summary>
        private DateTime _startTime;

        /// <summary>
        /// Last check update.
        /// </summary>
        private CheckUpdatedArgs _lastUpdate;

        /// <summary>
        /// Cancel event handler, for use by workflow.
        /// </summary>
        public event EventHandler Cancelled;

        /// <summary>
        /// Lock for progress state.
        /// </summary>
        private readonly object _progressLock;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        public ProgressForm()
        {
            InitializeComponent();

            _progressLock = new object();
            ResetForm();
        }

        /// <summary>
        /// On notification validation updates.
        /// </summary>
        /// <param name="updatedArgs"></param>
        public void OnCheckUpdated(CheckUpdatedArgs updatedArgs)
        {
            lock (_progressLock)
            {
                _lastUpdate = updatedArgs;
            }
        }

        /// <summary>
        /// Cancellation click handler.
        /// </summary>
        /// <param name="sender">Sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnCancelClick(object sender, EventArgs e)
        {
            pbrStatus.Value = pbrStatus.Minimum;
            pbrStatus.Style = ProgressBarStyle.Marquee;

            lblTitle.Text = "Cancelling Validation...";
            Cancelled?.Invoke(sender, e);
        }
        /// <summary>
        /// Reset the form back to now
        /// </summary>
        public void ResetForm()
        {
            _startTime = DateTime.Now;
            _lastUpdate = null;

            ResetFormContents();
        }

        /// <summary>
        /// Reset the form contents
        /// </summary>
        private void ResetFormContents()
        {
            pbrStatus.Value = pbrStatus.Minimum;
            pbrStatus.Style = ProgressBarStyle.Marquee;

            lblTitle.Text = "Running Validation...";
        }

        /// <summary>
        /// Helper method that reports time as "MM:SS" text form TimeSpan, showing the ":" only on odd seconds.
        /// </summary>
        /// <param name="timeSpan">Input time span (required).</param>
        /// <returns>Reported time text.</returns>
        private string GetElapsedTime(TimeSpan timeSpan)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"{(int)timeSpan.TotalMinutes:D2}");
            stringBuilder.Append(((timeSpan.Seconds % 2) == 0) ? " " : ":");
            stringBuilder.Append($"{timeSpan.Seconds:D2}");

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Timer update method that synchronizes progress bar, label text, and elapsed time label.
        /// </summary>
        /// <param name="sender">Event source (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnTimerUpdate(object sender, EventArgs e)
        {
            lblElapsedTime.Text = GetElapsedTime(DateTime.Now.Subtract(_startTime));
            CheckUpdatedArgs currUpdate;

            lock (_progressLock)
            {
                currUpdate = _lastUpdate;
            }
            if (currUpdate == null)
            {
                return;
            }

            if (currUpdate.BookCtr != pbrStatus.Value
                || currUpdate.TotalBooks != pbrStatus.Maximum)
            {
                pbrStatus.Maximum = currUpdate.TotalBooks;

                if (currUpdate.BookCtr > pbrStatus.Maximum
                    || currUpdate.BookCtr <= pbrStatus.Minimum)
                {
                    ResetFormContents();
                }
                else
                {
                    pbrStatus.Value = currUpdate.BookCtr;
                    pbrStatus.Style = ProgressBarStyle.Continuous;

                    lblTitle.Text = BookUtil.BookIdsByNum.TryGetValue(currUpdate.BookNum, out var bookId)
                            ? $"Checked {bookId.BookTitle} (#{currUpdate.BookCtr} of {currUpdate.TotalBooks})..."
                            : $"Checked #{currUpdate.BookNum} (#{currUpdate.BookCtr} of {currUpdate.TotalBooks})...";
                }

                Activate();
            }
        }
    }
}
