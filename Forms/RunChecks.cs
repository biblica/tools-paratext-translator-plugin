/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using AddInSideViews;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Amazon.Runtime;
using TvpMain.Check;
using TvpMain.CheckManagement;
using TvpMain.Project;
using TvpMain.Text;
using TvpMain.Util;

namespace TvpMain.Forms
{
    /// <summary>
    /// The new main dialog for TVP. This dialog allows users
    /// to select which check/fixes to run against the current project
    /// and which books or portion of books to run against.
    /// </summary>
    public partial class RunChecks : Form
    {
        /// <summary>
        /// Whether the user is a TVP Admin
        /// </summary>
        private bool _isCurrentUserTvpAdmin;

        /// <summary>
        /// The minimum number of characters required to perform a search.
        /// </summary>
        private const int MIN_SEARCH_CHARACTERS = 3;

        /// <summary>
        /// Prefix for local checks constant.
        /// </summary>
        private const string LOCAL_CHECKS_PREFIX = "(Local)";

        /// <summary>
        /// Prefix for local checks constant.
        /// </summary>
        private const string BUILTIN_CHECKS_PREFIX = "(Built-in)";

        /// <summary>
        /// Paratext host interface.
        /// </summary>
        private readonly IHost _host;

        /// <summary>
        /// Active project name.
        /// </summary>
        private string _activeProjectName;

        /// <summary>
        /// Provides project setting & metadata access.
        /// </summary>
        private ProjectManager _projectManager;

        /// <summary>
        /// This project's default checks
        /// </summary>
        private ProjectCheckSettings _projectCheckSettings;

        /// <summary>
        /// Storing the default book
        /// </summary>
        private int _defaultCurrentBook;

        /// <summary>
        /// The list of selected books to check
        /// </summary>
        private BookNameItem[] _selectedBooks;

        /// <summary>
        /// Access to the checks themselves
        /// </summary>
        readonly ICheckManager _checkManager;

        /// <summary>
        /// The list of remote checks
        /// </summary>
        List<CheckAndFixItem> _remoteChecks;

        /// <summary>
        /// The list of local checks, can't be set as defaults
        /// </summary>
        List<CheckAndFixItem> _localChecks;

        /// <summary>
        /// Simple progress bar form for when the checks are being synchronized
        /// </summary>
        GenericProgressForm _progressForm;

        /// <summary>
        /// Simple progress bar form for when the plugin is attempting to connect to the internet
        /// </summary>
        GenericProgressForm _connectForm;

        /// <summary>
        /// This is a separate list of items to display within the grid. This allows
        /// for tracking state during filtering.
        /// </summary>
        List<DisplayItem> _displayItems;

        /// <summary>
        /// This is a fixed CF for V1 TVP scripture reference checking
        /// </summary>
        readonly CheckAndFixItem _scriptureReferenceCf = new CheckAndFixItem(
            MainConsts.V1_SCRIPTURE_REFERENCE_CHECK_GUID,
            "(Built-in) Scripture Reference Verifications",
            "Scripture reference tag and formatting checks.",
            "2.0.0.0",
            CheckAndFixItem.CheckScope.VERSE);

        /// <summary>
        /// This is a fixed CF for V1 TVP missing punctuation checking
        /// </summary>
        readonly CheckAndFixItem _missingPunctuationCf = new CheckAndFixItem(MainConsts.V1_PUNCTUATION_CHECK_GUID,
            "(Built-in) Missing Punctuation Verifications",
            "Searches for missing punctuation.",
            "2.0.0.0",
            CheckAndFixItem.CheckScope.VERSE);

        /// <summary>
        /// Standard constructor for kicking off main plugin dialog
        /// </summary>
        /// <param name="host">This is the iHost instance, the interface class to the Paratext Plugin API</param>
        /// <param name="activeProjectName">The current project. Right now this is fixed, but maybe in the future this can be dynamically selected.</param>
        public RunChecks(IHost host, string activeProjectName)
        {
            InitializeComponent();
            _progressForm = new GenericProgressForm("Synchronizing Check/Fixes");
            _connectForm = new GenericProgressForm("Checking Connection ...");
            _checkManager = new CheckManager();

            // set up the needed service dependencies
            _host = host ?? throw new ArgumentNullException(nameof(host));

            SetActiveProject(activeProjectName ?? throw new ArgumentNullException(nameof(activeProjectName)));
        }

        /// <summary>
        /// Set up to support selecting a different project when we enable that. Right now the Plugin API
        /// does not allow for getting a list of projects.
        /// </summary>
        /// <param name="activeProjectName">Allows for setting the current project to work against</param>
        private void SetActiveProject(string activeProjectName)
        {
            _activeProjectName = activeProjectName;
            _projectManager = new ProjectManager(_host, _activeProjectName);
            _projectCheckSettings = HostUtil.Instance.GetProjectCheckSettings(_activeProjectName);

            SetCurrentBookDefaults();
            checksList.ClearSelection();
        }

        /// <summary>
        /// On Load method for the dialog
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void RunChecks_Load(object sender, EventArgs e)
        {
            // the project name text, will eventually be the selected current project from the list of projects
            projectNameText.Text = _activeProjectName;
            
            // sets up for just the current book by default
            SetCurrentBook();

            // disable the ability to save the project check defaults if not an admin
            if (!HostUtil.Instance.isCurrentUserAdmin(_activeProjectName))
            {
                setDefaultsToSelected.Hide();
            }

            // sets the chapter lengths and such for the current book
            SetCurrentBookDefaults();

            // set the copyright text
            Copyright.Text = MainConsts.COPYRIGHT;
            
            UpdateDisplayItems();

            // Ensure that the user is online so that permissions and synchronization work as expected.
            ConnectAndSync();
            
            // Update with the last-known refresh time.
            UpdateRefreshTooltip(null); 
        }

        /// <summary>
        /// Update the refresh button tooltip with a last synchronized time.
        /// </summary>
        /// <param name="syncTime">The time of the last sync. Overrides the value captured by the CheckManager. (optional)</param>
        private void UpdateRefreshTooltip(DateTime? syncTime)
        {
            runChecksTooltip.SetToolTip(refreshButton,
                $"Click to check for updates. \n(last updated: {syncTime ?? CheckManager.LastSyncTime})");
        }

        /// <summary>
        /// After doing async download of latest checks, update the list (must be run in main thread)
        /// </summary>
        private void UpdateDisplayItems()
        {
            try
            {
                // track display items that may already be selected,
                // so they can stay selected
                ISet<string> prevCheckedItems = (_displayItems == null
                        ? Enumerable.Empty<string>()
                        : _displayItems
                            .Where(foundItem => foundItem.Selected)
                            .Select(foundItem => foundItem.Name))
                    .ToImmutableHashSet();

                // load all the checks into the list
                _remoteChecks = _checkManager.GetInstalledCheckAndFixItems();
                _localChecks = _checkManager.GetSavedCheckAndFixItems();
                _displayItems = new List<DisplayItem>();

                // add the V1 defaults
                // get if the check is available (item1), and if not, the text for the tooltip (item2)
                var isCheckAvailableTupleRef = IsCheckAvailableForProject(_scriptureReferenceCf);
                _displayItems.Add(new DisplayItem(
                    prevCheckedItems.Contains(_scriptureReferenceCf.Name) ||
                    IsCheckDefaultForProject(_scriptureReferenceCf),
                    _scriptureReferenceCf.Name,
                    _scriptureReferenceCf.Description,
                    _scriptureReferenceCf.Version,
                    _scriptureReferenceCf.Languages != null && _scriptureReferenceCf.Languages.Length > 0
                        ? string.Join(", ", _scriptureReferenceCf.Languages)
                        : "All",
                    _scriptureReferenceCf.Tags != null ? string.Join(", ", _scriptureReferenceCf.Tags) : "",
                    _scriptureReferenceCf.Id,
                    isCheckAvailableTupleRef.Item1,
                    isCheckAvailableTupleRef.Item2,
                    _scriptureReferenceCf
                ));

                var isCheckAvailableTuplePunc = IsCheckAvailableForProject(_missingPunctuationCf);
                _displayItems.Add(new DisplayItem(
                    prevCheckedItems.Contains(_missingPunctuationCf.Name) ||
                    IsCheckDefaultForProject(_missingPunctuationCf),
                    _missingPunctuationCf.Name,
                    _missingPunctuationCf.Description,
                    _missingPunctuationCf.Version,
                    _missingPunctuationCf.Languages != null && _missingPunctuationCf.Languages.Length > 0
                        ? string.Join(", ", _missingPunctuationCf.Languages)
                        : "All",
                    _missingPunctuationCf.Tags != null ? string.Join(", ", _missingPunctuationCf.Tags) : "",
                    _missingPunctuationCf.Id,
                    isCheckAvailableTuplePunc.Item1,
                    isCheckAvailableTuplePunc.Item2,
                    _missingPunctuationCf
                ));

                // add all the known remote checks
                _remoteChecks.Sort((x, y) => x.Name.CompareTo(y.Name));
                foreach (var item in _remoteChecks)
                {
                    // get if the check is available (item1), and if not, the text for the tooltip (item2)
                    var isCheckAvailableTuple = IsCheckAvailableForProject(item);
                    _displayItems.Add(new DisplayItem(
                        prevCheckedItems.Contains(item.Name) || IsCheckDefaultForProject(item),
                        item.Name,
                        item.Description,
                        item.Version,
                        item.Languages != null && item.Languages.Length > 0 ? string.Join(", ", item.Languages) : "All",
                        item.Tags != null ? string.Join(", ", item.Tags) : "",
                        item.Id,
                        isCheckAvailableTuple.Item1,
                        isCheckAvailableTuple.Item2,
                        item
                    ));
                }

                // add all the local checks
                foreach (var item in _localChecks)
                {
                    // get if the check is available (item1), and if not, the text for the tooltip (item2)
                    var isCheckAvailableTuple = IsCheckAvailableForProject(item);
                    var localName = "(Local) " + item.Name;
                    _displayItems.Add(new DisplayItem(
                        prevCheckedItems.Contains(localName) || false,
                        localName,
                        item.Description,
                        item.Version,
                        item.Languages != null && item.Languages.Length > 0 ? string.Join(", ", item.Languages) : "All",
                        item.Tags != null ? string.Join(", ", item.Tags) : "",
                        item.Id,
                        isCheckAvailableTuple.Item1,
                        isCheckAvailableTuple.Item2,
                        item
                    ));
                }

                UpdateDisplayGrid();
            }
            finally
            {
                checksList.Enabled = true;
            }
        }

        /// <summary>
        /// Update what is shown on the form, in the list of checks, filtering for the search if applicable
        /// </summary>
        private void UpdateDisplayGrid()
        {
            checksList.Enabled = false;
            checksList.Rows.Clear();

            foreach (var displayItem in _displayItems)
            {
                if (filterTextBox.Text.Length >= MIN_SEARCH_CHARACTERS &&
                    displayItem.Name.IndexOf(filterTextBox.Text, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                var rowIndex = checksList.Rows.Add(
                    displayItem.Selected,
                    displayItem.Name,
                    displayItem.Version,
                    displayItem.Languages,
                    displayItem.Tags,
                    displayItem.Id
                );
                checksList.Rows[rowIndex].Selected = displayItem.Selected;
                checksList.Rows[rowIndex].Tag = displayItem;

                // Whether a check is local
                var isLocal = displayItem.Name.StartsWith(LOCAL_CHECKS_PREFIX);
                var isBuiltin = displayItem.Name.StartsWith(BUILTIN_CHECKS_PREFIX);

                // loop through all the cells in the row since tool tips can only be placed on the cell
                for (var i = 0; i < checksList.Columns.Count; i++)
                {
                    var toolTipBuilder = new StringBuilder(displayItem.Tooltip);

                    if (!isBuiltin && (isLocal || _isCurrentUserTvpAdmin))
                    {
                        toolTipBuilder.Append(Environment.NewLine);
                        toolTipBuilder.Append(Environment.NewLine);
                        toolTipBuilder.Append(_isCurrentUserTvpAdmin ? "C" : "Local c");
                        toolTipBuilder.Append("hecks can be edited by double-clicking on the name of the check.");
                    }

                    checksList.Rows[rowIndex].Cells[i].ToolTipText = toolTipBuilder.ToString().Trim();
                }

                // disable row if it can't be used on this project
                if (displayItem.Active)
                {
                    continue;
                }

                checksList.Rows[rowIndex].DefaultCellStyle.BackColor = SystemColors.Control;
                checksList.Rows[rowIndex].DefaultCellStyle.ForeColor = SystemColors.GrayText;
            }

            checksList.Enabled = true;
        }

        /// <summary>
        /// Sets the defaults for the current book, including the name and chapter counts
        /// </summary>
        private void SetCurrentBookDefaults()
        {
            var versificationName = _host.GetProjectVersificationName(_activeProjectName);
            BookUtil.RefToBcv(_host.GetCurrentRef(versificationName),
                out var runBookNum, out _, out _);

            _defaultCurrentBook = runBookNum;

            var lastChapter = _host.GetLastChapter(runBookNum, versificationName);

            fromChapterDropDown.Items.Clear();
            toChapterDropDown.Items.Clear();

            // add items for all the chapters
            for (var i = 0; i < lastChapter; i++)
            {
                fromChapterDropDown.Items.Add(i.ToString());
                toChapterDropDown.Items.Add(i.ToString());
            }

            // set the chapter indexes
            fromChapterDropDown.SelectedIndex = 0;
            toChapterDropDown.SelectedIndex = lastChapter - 1;

            // by default, use the current book
            currentBookRadioButton.Checked = true;

            // check that the current book by ID is available first
            if (!_projectManager.BookNamesByNum.ContainsKey(runBookNum))
            {
                var currentBook = BookUtil.BookIdsByNum[runBookNum];

                // let the user know they have not set the book's abbreviation, shortname, or longname
                throw new Exception(
                    $"The Book '{currentBook.BookCode}' has not had its Book Names set: abbreviation, short name, or long name. Please set these before continuing.");
            }

            // set the current book name
            currentBookText.Text = _projectManager.BookNamesByNum[runBookNum].BookCode;
            _selectedBooks = new[] {_projectManager.BookNamesByNum[runBookNum]};
        }

        /// <summary>
        /// Quit the dialog - File -> Exit from menu
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Start the check/fix editor from the menu
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void EditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new CheckEditor().ShowDialog(this);
            UpdateDisplayItems();
        }

        /// <summary>
        /// Display the EULA for the plugin.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void LicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUtil.StartLicenseForm();
        }

        /// <summary>
        /// This function is to handle when the "Run Checks" button is clicked. We pass the checks and notion of what to check to the CheckResultsForm to process.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void RunChecksButton_Click(object sender, EventArgs e)
        {
            // grab the selected checks
            var selectedChecks = GetSelectedChecks();
            if (selectedChecks.Count == 0)
            {
                MessageBox.Show(
                    @"No checks provided.",
                    @"Notice...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Provides warning when running non-RTL checks on an RTL project
            var projectRtoL = _host.GetProjectRtoL(_activeProjectName);
            if (projectRtoL)
            {
                // Track list of incompatible checks
                var cautionaryItems = new List<String>();

                foreach (var item in selectedChecks)
                {
                    if (item.Tags == null || !item.Tags.Contains("RTL"))
                    {
                        cautionaryItems.Add(item.Name);
                    }
                }

                if (cautionaryItems.Count > 0)
                {
                    MessageBox.Show(
                        $"The following checks have not been confirmed to work on a RTL language. Use with caution.\n• {String.Join("\n• ", cautionaryItems)}",
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }

            // grab the check run context
            var checkContext = GetCheckRunContext();
            checkContext.Validate();

            // prevent clicking the "Run Checks" button multiple times
            runChecksButton.Enabled = false;

            // pass the checks and specification of what to check to the CheckResultsForm to perform the necessary search with.
            var checkResultsForm = new CheckResultsForm(
                _host,
                _activeProjectName,
                _projectManager,
                _selectedBooks,
                selectedChecks,
                checkContext
            );

            checkResultsForm.BringToFront();
            checkResultsForm.ShowDialog(this);

            // after the results UI has closed, re-enable the "Run Checks" button
            runChecksButton.Enabled = true;
        }

        /// <summary>
        /// This function will return the <c>CheckAndFixItem</c>s that are selected in the Run Checks list.
        /// </summary>
        /// <returns>The selected <c>CheckAndFixItem</c>s</returns>
        private List<CheckAndFixItem> GetSelectedChecks()
        {
            var selectedChecks = new List<CheckAndFixItem>();

            // grab the selected checks
            foreach (DataGridViewRow row in checksList.Rows)
            {
                var item = ((DisplayItem) row.Tag).Item;
                if ((bool) row.Cells[0].Value)
                {
                    selectedChecks.Add(item);
                }
            }

            return selectedChecks;
        }

        /// <summary>
        /// This function create and return the <c>CheckRunContext</c> of what's being checked against.
        /// </summary>
        /// <returns>The <c>CheckRunContext</c> of what's being checked against.</returns>
        private CheckRunContext GetCheckRunContext()
        {
            // initialize the context with the project name.
            var checkRunContext = new CheckRunContext
            {
                Project = _activeProjectName,
                Books = (BookNameItem[]) _selectedBooks.Clone()
            };

            // track the selected books
            if (currentBookRadioButton.Checked)
            {
                checkRunContext.CheckScope = CheckAndFixItem.CheckScope.CHAPTER;

                // track the specified chapters
                checkRunContext.Chapters = new List<int>();

                var chapterStart = int.Parse(fromChapterDropDown.Text);
                var chapterEnd = int.Parse(toChapterDropDown.Text);

                // flip the values, if the end is larger than the start
                if (chapterStart > chapterEnd)
                {
                    var temp = chapterStart;
                    chapterStart = chapterEnd;
                    chapterEnd = temp;
                }

                // add the chapters to check
                for (var i = chapterStart; i <= chapterEnd; i++)
                {
                    checkRunContext.Chapters.Add(i);
                }
            }
            else
            {
                checkRunContext.CheckScope = CheckAndFixItem.CheckScope.BOOK;
            }

            return checkRunContext;
        }

        /// <summary>
        /// Select which books, if not using the default single book, to seach through
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void ChooseBooksButton_Click(object sender, EventArgs e)
        {
            // bring up book selection dialog, use current selection to initialize
            using (var form = new BookSelection(_projectManager, _selectedBooks))
            {
                form.StartPosition = FormStartPosition.CenterParent;

                var result = form.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    // update which books were selected
                    _selectedBooks = form.GetSelected();
                    var selectedBooksString = BookSelection.stringFromSelectedBooks(_selectedBooks);
                    chooseBooksText.Text = selectedBooksString;
                }
            }

            // set up UI
            SetChooseBooks();
        }

        /// <summary>
        /// Quit the dialog
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Allow for the selection dialog to pop up
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void ChooseBooksRadioButton_Click(object sender, EventArgs e)
        {
            SetChooseBooks();
            // If switching from just one, show the selection dialog.
            // Don't do that again if switching back-and-forth
            if (_selectedBooks.Length < 2)
            {
                ChooseBooksButton_Click(sender, e);
            }
        }

        /// <summary>
        /// Allow for switching back to just the current book
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void CurrentBookRadioButton_Click(object sender, EventArgs e)
        {
            SetCurrentBook();
        }

        /// <summary>
        /// Update the UI
        /// </summary>
        private void SetChooseBooks()
        {
            currentBookRadioButton.Checked = false;
            chooseBooksRadioButton.Checked = true;
            fromChapterDropDown.Enabled = false;
            toChapterDropDown.Enabled = false;
        }

        /// <summary>
        /// Update the UI, and reset selected books list
        /// </summary>
        private void SetCurrentBook()
        {
            currentBookRadioButton.Checked = true;
            chooseBooksRadioButton.Checked = false;
            fromChapterDropDown.Enabled = true;
            toChapterDropDown.Enabled = true;

            _selectedBooks = new[] {_projectManager.BookNamesByNum[_defaultCurrentBook]};
        }

        /// <summary>
        /// Update the project default check/fixes, saving to project file
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void SetDefaultsToSelected_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show(
                @"Are you sure you wish to set the default checks/fixes for this project? ",
                @"Verify Change", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                foreach (DataGridViewRow row in checksList.Rows)
                {
                    var item = (DisplayItem) checksList.Rows[row.Index].Tag;

                    if ((bool) row.Cells[0].Value)
                    {
                        if (!_projectCheckSettings.DefaultCheckIds.Contains(item.Id))
                        {
                            // do not allow local only to be in the defaults list
                            if (!_localChecks.Contains(item.Item))
                            {
                                _projectCheckSettings.DefaultCheckIds.Add(item.Id);
                            }
                        }
                    }
                    else
                    {
                        if (_projectCheckSettings.DefaultCheckIds.Contains(item.Id))
                        {
                            _projectCheckSettings.DefaultCheckIds.Remove(item.Id);
                        }
                    }
                }

                // save
                HostUtil.Instance.PutProjectCheckSettings(_activeProjectName, _projectCheckSettings);
            }
        }

        /// <summary>
        /// Update the list of selected checks to the project defaults
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void ResetToProjectDefaults_Click(object sender, EventArgs e)
        {
            UpdateDisplayItems();
        }

        /// <summary>
        /// Determines if the given check/fix item is a default on the project
        /// </summary>
        /// <param name="item"></param>
        /// <returns>if the given check/fix item is a default on the project</returns>
        private bool IsCheckDefaultForProject(CheckAndFixItem item)
        {
            return _projectCheckSettings.DefaultCheckIds.Contains(item.Id);
        }

        /// <summary>
        /// Utility method to determine if the specified check can be run on this project
        ///  Will filter out based on language
        ///  Will filter out based on Tags, add additional tag support here
        /// </summary>
        /// <param name="item">The check/fix item to use to determine if it can be used against the current project</param>
        /// <returns>If the given CFitem is available (item1) to be used with the project. If not, the tooltip to use for the disabled row (item2).</returns>
        private Tuple<bool, string> IsCheckAvailableForProject(CheckAndFixItem item)
        {
            var languageId = _host.GetProjectLanguageId(_activeProjectName, "translation validation").ToUpper();
            var projectRtl = _host.GetProjectRtoL(_activeProjectName);
            var builtInCheck = item.Name.StartsWith(BUILTIN_CHECKS_PREFIX);

            // filter based on language
            var languageEnabled = item.Languages == null
                                  || (item.Languages != null && item.Languages.Length == 0)
                                  || (item.Languages != null && item.Languages.Length > 0 &&
                                      item.Languages.Contains(languageId, StringComparer.OrdinalIgnoreCase));

            // filter based on Tags

            // RTL Tag support
            var itemRtl = (item.Tags != null) && (item.Tags.Contains("RTL"));

            var rtlEnabled = !(itemRtl && !projectRtl);

            Debug.WriteLine("Project Language: " + languageId);
            Debug.WriteLine("Project RTL: " + projectRtl);
            Debug.WriteLine("Item RTL: " + rtlEnabled);

            var filterReasons = new List<string>();


            // set the response strings for the appropriate filter reason
            if (builtInCheck)
            {
                filterReasons.Add("This check is built-in and cannot be edited.");
            }

            if (!languageEnabled)
            {
                filterReasons.Add("This check doesn't support this project's language.");
            }

            if (!rtlEnabled)
            {
                filterReasons.Add("This check is for RTL languages only.");
            }

            var response = String.Join("\n", filterReasons);
            return new Tuple<bool, string>(languageEnabled && rtlEnabled, response);
        }

        // 
        // The following methods are for updating the instructions at the bottom of the page 
        // as the mouse is moved around the dialog
        // 

        /// <summary>
        /// For handling updating the instructions as the mouse moves around the table of available
        /// checks. Will note if a check is disabled.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void ChecksList_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex <= -1)
            {
                return;
            }

            var item = (DisplayItem) checksList.Rows[e.RowIndex].Tag;
            helpTextBox.Clear();

            if (!IsCheckAvailableForProject(item.Item).Item1)
            {
                helpTextBox.AppendText("NOTE: This check/fix is not selectable for this project" + Environment.NewLine +
                                       Environment.NewLine);
            }

            helpTextBox.AppendText("Check/Fix: " + item.Name + Environment.NewLine);
            helpTextBox.AppendText(item.Description);
        }

        /// <summary>
        /// Update help text for choosing multiple books
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void ChooseBooksRadioButton_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Text = @"Select the set of books to be checked.";
        }

        /// <summary>
        /// Update help text for choosing multiple books
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void ChooseBooksText_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Text = @"The set of books chosen to check.";
        }

        /// <summary>
        /// Update help text for the single book selection radio button
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void CurrentBookRadioButton_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Text = @"Check the current book";
        }

        /// <summary>
        /// Update help text for the chapter dropdown control
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void FromChapterDropDown_MouseEnter(object sender, EventArgs e)
        {
            if (fromChapterDropDown.Enabled)
            {
                helpTextBox.Text = @"Select the starting chapter in the current book to begin the check.";
            }
        }

        /// <summary>
        /// Update help text for the to chapter dropdown control
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void ToChapterDropDown_MouseEnter(object sender, EventArgs e)
        {
            if (fromChapterDropDown.Enabled)
            {
                helpTextBox.Text = @"Select the ending chapter in the current book to finish checking.";
            }
        }

        /// <summary>
        /// Update help text for setting the whole dialog back to the project default checks
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void ResetToProjectDefaults_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Text = @"Sets the selected checks/fixes back to the project defaults, " +
                               @"or if there are no defaults, deselects all.";
        }

        /// <summary>
        /// Update help text for saving the project defaults
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void SetDefaultsToSelected_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Text = @"Saves the currently selected checks/fixes as the default set " +
                               @"for this project. This does not include local checks/fixes as they can not be " +
                               @"set as defaults. This may only be performed by accounts with the sufficient privileges.";
        }

        /// <summary>
        /// Refresh the check/item list to see if there any new check/fixes available
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void RefreshButton_Click(object sender, EventArgs e)
        {
            PrepForSync(); // Update the UI immediately so that it doesn't appear to hang during connection check.
            ConnectAndSync(true);
        }
        
        /// <summary>
        /// Handles UI updates prior to starting synchronization.
        /// </summary>
        private void PrepForSync()
        {
            _progressForm.Show();
            refreshButton.Enabled = false;
            Enabled = false;
        }

        /// <summary>
        /// Filter the available checks based on the entry here
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void FilterTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateDisplayGrid();
        }

        /// <summary>
        /// Opens a local check in the editor from the RunChecks UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChecksList_EditCheck(object sender, DataGridViewCellEventArgs e)
        {
            const string localCheckPrefix = "(Local)";
            const string builtInCheckPrefix = "(Built-in)";

            // Get the check that was clicked
            var selectedCheck = _displayItems[e.RowIndex];

            var isTvpAdmin = _isCurrentUserTvpAdmin;
            var isLocalCheck = selectedCheck.Name.StartsWith(localCheckPrefix);
            var builtInCheck = selectedCheck.Name.StartsWith(builtInCheckPrefix);

            // Non-admins can only edit local checks
            if (builtInCheck)
            {
                // Dialog box that shows if attempts to edit a built-in check
                MessageBox.Show("Built-in checks are not able to be edited.", "Warning");
                return;
            }
            else if (!isLocalCheck && !isTvpAdmin)
            {
                // Dialog box that shows if a user attempts to edit a check as a non-admin
                MessageBox.Show("Only administrators can edit non-local checks.", "Warning");
                return;
            }

            var name = isLocalCheck ? selectedCheck.Name.Replace(localCheckPrefix, "") : selectedCheck.Name;
            var checkDir = isLocalCheck
                ? _checkManager.GetLocalRepoDirectory()
                : _checkManager.GetInstalledChecksDirectory();

            // Get the file location for the selected check
            var fileName = _checkManager.GetCheckAndFixItemFilename(selectedCheck.Item);
            var fullPath = Path.Combine(checkDir, fileName);

            // Open the CheckEditor with the selected check
            new CheckEditor(new FileInfo(fullPath), !isLocalCheck).ShowDialog(this);

            UpdateDisplayItems();
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
        /// Default, show the EULA for the project.
        /// License from menu
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void LicenseToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            FormUtil.StartLicenseForm();
        }

        /// <summary>
        /// Attempts to reconnect to the internet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tryToReconnectButton_Click(object sender, EventArgs e)
        {
            _connectForm.Show();
            ConnectAndSync();
        }

        /// <summary>
        /// Verifies internet connection and synchronizes checks.
        /// </summary>
        /// <param name="forceSync"></param>
        private void ConnectAndSync(bool forceSync = false)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += connectWorker_DoWork;
            worker.RunWorkerCompleted += connectWorker_RunWorkerCompleted;
            worker.RunWorkerCompleted += (o, args) =>
            {
                if (!HostUtil.Instance.IsOnline || (!forceSync && CheckManager.HasSyncRun))
                {
                    _progressForm.Hide(); // Ensure that the synchronization form is hidden if it was shown elsewhere.
                    return;
                };
                PrepForSync();
                StartCheckSynchronization();
            };
            Enabled = false;
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Handles checking for an internet connection in the background.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            HostUtil.Instance.TryGoOnline();
        }

        /// <summary>
        /// Handles closing the internet connection progress form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UpdateOnlineStatus();
            Enabled = true;
            _connectForm.Hide();
        }
        
        /// <summary>
        /// Handle starting the synchronization process.
        /// </summary>
        private void StartCheckSynchronization()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += SynchronizationWorker_DoWork;
            worker.RunWorkerCompleted += SynchronizationWorker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }
        
        /// <summary>
        /// Handle ending the synchronization process.
        /// </summary>
        private void EndCheckSynchronization()
        {
            Enabled = true;
            _progressForm.Hide();
        }

        /// <summary>
        /// Async method for synchronizing the check/fixes for the project and selecting the defaults
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void SynchronizationWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // sync with repo
                _checkManager.SynchronizeInstalledChecks();
                UpdateRefreshTooltip(DateTime.Now);
                CheckManager.HasSyncRun = true;
                refreshButton.Enabled = true;
            }
            catch (AmazonServiceException)
            {
                HostUtil.Instance.IsOnline = false;
                UpdateOnlineStatus();
            }
        }

        /// <summary>
        /// Close the progress form when complete
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void SynchronizationWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            checksList.Invoke(new MethodInvoker(UpdateDisplayItems));
            EndCheckSynchronization();
        }

        /// <summary>
        /// Handle UI updates that reflect the current online status.
        /// </summary>
        private void UpdateOnlineStatus()
        {
            var onlineWindowLabel = Name;
            var offlineWindowLabel = $@"{Name} (offline)";

            if (HostUtil.Instance.IsOnline)
            {
                _isCurrentUserTvpAdmin = HostUtil.Instance.IsCurrentUserTvpAdmin();
                Text = onlineWindowLabel;
            }
            else
            {
                Text = offlineWindowLabel;
                MessageBox.Show(
                    @"You appear to be offline, which may limit your ability to edit checks. Use the 'Try to Connect' button to try again.",
                    @"Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            tryToConnectButton.Visible = !HostUtil.Instance.IsOnline;
            refreshButton.Enabled = HostUtil.Instance.IsOnline;
        }

        /// <summary>
        /// Handles when a user clicks the "Delete" context menu option.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteSelectedRows();
        }

        /// <summary>
        /// Deletes the currently selected rows in the checks list DataGridView.
        /// </summary>
        private void deleteSelectedRows()
        {
            if (checksList.SelectedRows.Count > 0)
            {
                if (confirmDeleteSelectedRows())
                {
                    foreach (DataGridViewRow row in checksList.SelectedRows)
                    {
                        deleteChecksListItem(row.Index);
                    }
                }
            }
        }

        /// <summary>
        /// Deletes a row in the DataGridView. Also deletes any corresponding xml files.
        /// </summary>
        /// <param name="rowIndex">Index of the row to delete.</param>
        private void deleteChecksListItem(int rowIndex)
        {
            DisplayItem displayItem = (DisplayItem) checksList.Rows[rowIndex].Tag;
            CheckAndFixItem checkItem = displayItem.Item;
            if (_localChecks.Contains(checkItem))
            {
                _checkManager.DeleteCheckAndFixItem(checkItem);
                _localChecks.Remove(checkItem);
            }
            else if (_remoteChecks.Contains(checkItem))
            {
                _checkManager.UnpublishAndUninstallCheckAndFixItem(checkItem);
                _remoteChecks.Remove(checkItem);
            }
            checksList.Rows.RemoveAt(rowIndex);

        }

        /// <summary>
        /// Checks to see if invalid checks are selected.
        /// </summary>
        /// <param name="dataGridView">The data grid to check.</param>
        /// <returns>True if invalid checks are selected. False otherwise. </returns>
        private bool inactiveChecksAreSelected(DataGridView dataGridView)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Selected && row.Tag != null)
                {
                    var rowData = (DisplayItem) row.Tag;
                    if (!rowData.Active)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Shows a dialog box to confirm deletion of any selected rows. 
        /// </summary>
        /// <returns>Returns true if the user confirms deletion. False otherwise.</returns>
        private bool confirmDeleteSelectedRows()
        {
            string dialogCaption = "Confirm Delete";
            bool firstItem = true;
            string itemList = "";
            foreach (DataGridViewRow row in checksList.SelectedRows)
            {
                string itemName = row.Cells["CFName"].Value.ToString();
                //string itemType = row.Cells["TypeColumn"].Value.ToString();
                string separator = firstItem ? "" : ", ";
                itemList += separator + "\"" + itemName + "\"";
                firstItem = false;
            }
            string dialogTextFormat = "Delete the following items?" + Environment.NewLine + Environment.NewLine + "{0}";
            string dialogText = String.Format(dialogTextFormat, itemList);
            DialogResult result = MessageBox.Show(dialogText, dialogCaption, MessageBoxButtons.YesNo);

            return result == DialogResult.Yes;
        }

        /// <summary>
        /// Triggers row deletion when the delete key is pressed.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void checksList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46 && checksList.SelectedRows.Count > 0)
            {
                e.Handled = true;
                deleteSelectedRows();
            }
        }

        /// <summary>
        /// When a row is selected or deselected:
        /// - set the Selected check box to the same value.
        /// - disable the run checks button if an invalid check is selected
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void checksList_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (e.StateChanged == DataGridViewElementStates.Selected)
            {
                if (e.Row.Tag != null)
                {
                    var rowData = (DisplayItem)e.Row.Tag;
                    rowData.Selected = e.Row.Selected;
                }
                e.Row.Cells["CFSelected"].Value = e.Row.Selected;

                runChecksButton.Enabled = !inactiveChecksAreSelected(e.Row.DataGridView);
            }
        }

        /// <summary>
        /// Handles row selection logic when a right mouse button is clicked.
        /// When an unselected row is right clicked, select it and unselect all other rows.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void checksList_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView dataGridView = sender as DataGridView;
            if (e.RowIndex != -1 && e.Button == MouseButtons.Right)
            {
                DataGridViewRow row = dataGridView.Rows[e.RowIndex];
                if (!row.Selected)
                {
                    row.DataGridView.ClearSelection();
                    row.DataGridView.CurrentCell = row.Cells["CFSelected"];
                    row.Selected = true;
                }
            }
        }
    }
}