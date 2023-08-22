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
using System.Xml.Serialization;
using NDesk.DBus;
using System.IdentityModel;

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
        /// The minimum number of characters required to perform a search.
        /// </summary>
        private const int MIN_SEARCH_CHARACTERS = 3;

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
        /// Access to checks repository options
        /// </summary>
        readonly OptionsManager _optionsManager;

        /// <summary>
        /// Access to the checks themselves
        /// </summary>
        readonly ICheckManager _checkManager;

        /// <summary>
        /// The list of installed checks
        /// </summary>
        List<IRunnable> _installedChecks;

        /// <summary>
        /// The list of local checks, can't be set as defaults
        /// </summary>
        List<IRunnable> _localChecks;

        /// <summary>
        /// Simple progress bar form for when the checks are being synchronized
        /// </summary>
        GenericProgressForm _syncProgressForm;

        /// <summary>
        /// Simple progress bar form for when the plugin is attempting to connect to the internet
        /// </summary>
        GenericProgressForm _connectProgressForm;

        /// <summary>
        /// This is a separate list of items to display within the grid. This allows
        /// for tracking state during filtering.
        /// </summary>
        List<GridRowData> _gridData;

        /// <summary>
        /// This is a fixed CF for V1 TVP scripture reference checking
        /// </summary>
        readonly CheckAndFixItem _scriptureReferenceCf = new CheckAndFixItem(
            MainConsts.V1_SCRIPTURE_REFERENCE_CHECK_GUID,
            "Scripture Reference Verifications",
            "Scripture reference tag and formatting checks.",
            "2.0.0.0",
            CheckAndFixItem.CheckScope.VERSE);

        /// <summary>
        /// This is a fixed CF for V1 TVP missing punctuation checking
        /// </summary>
        readonly CheckAndFixItem _missingPunctuationCf = new CheckAndFixItem(MainConsts.V1_PUNCTUATION_CHECK_GUID,
            "Missing Punctuation Verifications",
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
            _syncProgressForm = new GenericProgressForm("Synchronizing Check/Fixes");
            _connectProgressForm = new GenericProgressForm("Checking Connection ...");
            _optionsManager = new OptionsManager();
            _checkManager = new CheckManager(_optionsManager);

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
            if (!HostUtil.Instance.isCurrentUserProjectAdmin(_activeProjectName))
            {
                setDefaultsToSelected.Hide();
            }

            // sets the chapter lengths and such for the current book
            SetCurrentBookDefaults();

            // set the copyright text
            Copyright.Text = MainConsts.COPYRIGHT;
            
            UpdateGridData();

            if (_checkManager.RemoteRepositoryIsEnabled())
            {
                refreshButton.Visible = true;
                if (_checkManager.SyncOnStartup())
                {
                    // Ensure that the user is online so that permissions and synchronization work as expected.
                    ConnectAndSync();
                }
            }

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
        private void UpdateGridData()
        {
            try
            {
                // track grid rows that may already be selected,
                // so they can stay selected
                ISet<string> prevCheckedRows = (_gridData == null
                        ? Enumerable.Empty<string>()
                        : _gridData
                            .Where(foundRow => foundRow.Selected)
                            .Select(foundRow => foundRow.Id))
                    .ToImmutableHashSet();

                // load all the checks into the list
                _gridData = new List<GridRowData>();

                // add the V1 defaults
                // get if the check is available (item1), and if not, the text for the tooltip (item2)
                var isCheckAvailableTupleRef = IsValidForProject(_scriptureReferenceCf);
                _gridData.Add(new GridRowData(
                    prevCheckedRows.Contains(_scriptureReferenceCf.Id) ||
                    IsCheckDefaultForProject(_scriptureReferenceCf),
                    MainConsts.BUILTIN_REPO_NAME,
                    "Check",
                    _scriptureReferenceCf.Name,
                    _scriptureReferenceCf.Description,
                    _scriptureReferenceCf.Version,
                    displayLanguages(_scriptureReferenceCf.Languages),
                    displayTags(_scriptureReferenceCf.Tags),
                    _scriptureReferenceCf.Id,
                    isCheckAvailableTupleRef.Item1,
                    isCheckAvailableTupleRef.Item2,
                    _scriptureReferenceCf
                ));

                var isCheckAvailableTuplePunc = IsValidForProject(_missingPunctuationCf);
                _gridData.Add(new GridRowData(
                    prevCheckedRows.Contains(_missingPunctuationCf.Id) ||
                    IsCheckDefaultForProject(_missingPunctuationCf),
                    MainConsts.BUILTIN_REPO_NAME,
                    "Check",
                    _missingPunctuationCf.Name,
                    _missingPunctuationCf.Description,
                    _missingPunctuationCf.Version,
                    displayLanguages(_missingPunctuationCf.Languages),
                    displayTags(_missingPunctuationCf.Tags),
                    _missingPunctuationCf.Id,
                    isCheckAvailableTuplePunc.Item1,
                    isCheckAvailableTuplePunc.Item2,
                    _missingPunctuationCf
                ));


                // add all the known remote checks
                if (_checkManager.RemoteRepositoryIsEnabled())
                {
                    _installedChecks = _checkManager.GetInstalledItems();
                }
                else
                {
                    _installedChecks = new List<IRunnable>();
                }
                _installedChecks.Sort((x, y) => x.Name.CompareTo(y.Name));
                foreach (var item in _installedChecks)
                {
                    // get if the check is available (item1), and if not, the text for the tooltip (item2)
                    var isCheckAvailableTuple = IsValidForProject(item);
                    _gridData.Add(new GridRowData(
                        prevCheckedRows.Contains(item.Id) || IsCheckDefaultForProject(item),
                        MainConsts.REMOTE_REPO_NAME,
                        "Check",
                        item.Name,
                        item.Description,
                        item.Version,
                        displayLanguages(item.Languages),
                        displayTags(item.Tags),
                        item.Id,
                        isCheckAvailableTuple.Item1,
                        isCheckAvailableTuple.Item2,
                        item
                    ));
                }

                _localChecks = _checkManager.GetSavedItems();
                foreach (IRunnable item in _localChecks)
                {
                    // get if the check is available (item1), and if not, the text for the tooltip (item2)
                    var isCheckAvailableTuple = IsValidForProject(item);
                    _gridData.Add(new GridRowData(
                        prevCheckedRows.Contains(item.Id) || false,
                        MainConsts.LOCAL_REPO_NAME,
                        "Check",
                        item.Name,
                        item.Description,
                        item.Version,
                        displayLanguages(item.Languages),
                        displayTags(item.Tags),
                        item.Id,
                        isCheckAvailableTuple.Item1,
                        isCheckAvailableTuple.Item2,
                        item
                    ));
                }

                UpdateGrid();
            }
            finally
            {
                checksList.Enabled = true;
            }
        }

        /// <summary>
        /// Generates the text the will be displayed in the Languages column of the grid
        /// based on the value of the Languages property of a check or group.
        /// </summary>
        /// <param name="languages">The Languages property value</param>
        /// <returns>The text to display in the Languages column</returns>
        private string displayLanguages(string[] languages)
        {
            if (languages is null)
            {
                return "<INVALID>";
            }
            else if (languages.Length == 0)
            {
                return "All";
            }

            return string.Join(", ", languages);
        }

        /// <summary>
        /// Generates the text the will be displayed in the Tags column of the grid
        /// based on the value of the Tags property of a check or group.
        /// </summary>
        /// <param name="languages">The Tags property value</param>
        /// <returns>The text to display in the Tags column</returns>
        private string displayTags(string[] tags)
        {
            if (tags is null)
            {
                return "<INVALID>";
            }
            else if (tags.Length == 0)
            {
                return "";
            }

            return tags != null ? string.Join(", ", tags) : "";
        }

        /// <summary>
        /// Checks to see if a row contains a built-in item.
        /// </summary>
        /// <param name="rowData">The row data to check.</param>
        /// <returns>true if the item is built-in. false otherwise.</returns>
        private bool isBuiltIn(GridRowData rowData)
        {
            return rowData.Location == MainConsts.BUILTIN_REPO_NAME;
        }

        /// <summary>
        /// Checks to see if a row contains an item that is stored in the local repository.
        /// </summary>
        /// <param name="rowData">The item to check.</param>
        /// <returns>true if the item is stored in the local repository.</returns>
        private bool isLocal(GridRowData rowData)
        {
            return rowData.Location == MainConsts.LOCAL_REPO_NAME;
        }

        /// <summary>
        /// Update what is shown on the form, in the list of checks, filtering for the search if applicable
        /// </summary>
        private void UpdateGrid()
        {
            checksList.Enabled = false;
            checksList.Rows.Clear();

            foreach (var rowData in _gridData)
            {
                if (filterTextBox.Text.Length >= MIN_SEARCH_CHARACTERS &&
                    rowData.Name.IndexOf(filterTextBox.Text, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                var rowIndex = checksList.Rows.Add(
                    rowData.Location,
                    rowData.Type,
                    rowData.Name,
                    rowData.Version,
                    rowData.Languages,
                    rowData.Tags,
                    rowData.Id
                );
                checksList.Rows[rowIndex].Selected = rowData.Selected;
                checksList.Rows[rowIndex].Tag = rowData;

                // disable row if it can't be used on this project
                if (rowData.Active)
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
        /// Display the EULA for the plugin.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void LicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUtil.StartLicenseForm();
        }

        /// <summary>
        /// This function is to handle when the "Run Checks" button is clicked. 
        /// We pass the checks and notion of what to check to the CheckResultsForm to process.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void RunChecksButton_Click(object sender, EventArgs e)
        {
            // grab the selected runnable items
            var selectedRunnables = GetSelectedRunnables();
            if (selectedRunnables.Count == 0)
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
                // Track list of incompatible items
                var cautionaryItems = new List<String>();

                foreach (var runnable in selectedRunnables)
                {
                    foreach (var check in runnable.Checks)
                    {
                        if (check.Tags == null || !check.Tags.Contains("RTL"))
                        {
                            cautionaryItems.Add(check.Name);
                        }
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
                selectedRunnables,
                checkContext
            );

            checkResultsForm.BringToFront();
            checkResultsForm.ShowDialog(this);

            // after the results UI has closed, re-enable the "Run Checks" button
            runChecksButton.Enabled = true;
        }

        /// <summary>
        /// This function will return the runnable items that are selected in the Run Checks list.
        /// </summary>
        /// <returns>The selected runnable items</returns>
        private List<IRunnable> GetSelectedRunnables() 
        {
            var selectedRunnables = new List<IRunnable>();

            foreach (DataGridViewRow row in checksList.Rows)
            {
                if (row.Selected)
                {
                    selectedRunnables.Add(((GridRowData)row.Tag).Item);
                }
            }

            return selectedRunnables;
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
                    var rowData = (GridRowData) checksList.Rows[row.Index].Tag;

                    if (row.Selected)
                    {
                        if (!_projectCheckSettings.DefaultCheckIds.Contains(rowData.Id))
                        {
                            // do not allow local only to be in the defaults list
                            if (!_localChecks.Contains(rowData.Item))
                            {
                                _projectCheckSettings.DefaultCheckIds.Add(rowData.Id);
                            }
                        }
                    }
                    else
                    {
                        if (_projectCheckSettings.DefaultCheckIds.Contains(rowData.Id))
                        {
                            _projectCheckSettings.DefaultCheckIds.Remove(rowData.Id);
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
            UpdateGridData();
        }

        /// <summary>
        /// Determines if the given runnable item is a default on the project
        /// </summary>
        /// <param name="item"></param>
        /// <returns>if the given runnable item is a default on the project</returns>
        private bool IsCheckDefaultForProject(IRunnable item)
        {
            return _projectCheckSettings.DefaultCheckIds.Contains(item.Id);
        }

        /// <summary>
        /// Utility method to determine if the specified check or group can be run on this project
        /// - Will filter out based on language
        /// - Will filter out based on Tags, add additional tag support here
        /// </summary>
        /// <param name="item">The check or group to determine if it can be used against the current project</param>
        /// <returns>If the given item is available (item1) to be used with the project. If not, the tooltip to use for the disabled row (item2).</returns>
        private Tuple<bool, string> IsValidForProject(IRunnable item)
        {
            var isGroup = item is CheckGroup;
            var isValid = true;
            var response = "";

            var languageId = _host.GetProjectLanguageId(_activeProjectName, "translation validation").ToUpper();
            var projectRtl = _host.GetProjectRtoL(_activeProjectName);

            foreach (var check in item.Checks)
            {
                // filter based on language
                var languageEnabled = check.Languages == null
                                      || (check.Languages != null && check.Languages.Length == 0)
                                      || (check.Languages != null && check.Languages.Length > 0 &&
                                          check.Languages.Contains(languageId, StringComparer.OrdinalIgnoreCase));

                // filter based on Tags

                // RTL Tag support
                var checkRtl = (check.Tags != null) && (check.Tags.Contains("RTL"));
                var rtlEnabled = !(checkRtl && !projectRtl);

                Debug.WriteLine("Project Language: " + languageId);
                Debug.WriteLine("Project RTL: " + projectRtl);
                Debug.WriteLine("Item RTL: " + rtlEnabled);

                // set the response strings for the appropriate filter reason
                var filterReasons = new List<string>();
                string typeText = isGroup ? "group contains a check that" : "check";
                if (!languageEnabled)
                {
                    filterReasons.Add($"This {typeText} does not support this project's language.");
                }
                if (!rtlEnabled)
                {
                    filterReasons.Add($"This {typeText} is for RTL languages only.");
                }

                isValid = languageEnabled && rtlEnabled;

                if (!isValid)
                {
                    response = String.Join("\n", filterReasons);
                    break;
                }
            }

            return new Tuple<bool, string>(isValid, response);
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
            if (e.RowIndex <= -1) return;

            var rowData = (GridRowData) checksList.Rows[e.RowIndex].Tag;
            helpTextBox.Clear();
            if (!IsValidForProject(rowData.Item).Item1)
            {
                helpTextBox.AppendText("NOTE: This item is not selectable for this project" + Environment.NewLine +
                                       Environment.NewLine);
            }

            string typeText = rowData.Item is CheckGroup ? "Group: " : "Check: ";
            helpTextBox.AppendText(typeText + rowData.Name + Environment.NewLine);
            helpTextBox.AppendText(rowData.Description);
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
            ConnectAndSync(true);
        }
        
        /// <summary>
        /// Handles UI updates prior to starting synchronization.
        /// </summary>
        private void PrepForSync()
        {
            _syncProgressForm.Show(this);
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
            UpdateGrid();
        }

        /// <summary>
        /// Opens a local check in the editor from the RunChecks UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChecksList_EditCheck(object sender, DataGridViewCellEventArgs e)
        {
            // Get the check that was clicked
            var rowData = _gridData[e.RowIndex];

            // Non-admins can only edit local checks
            if (isBuiltIn(rowData))
            {
                // Dialog box that shows if attempts to edit a built-in check
                MessageBox.Show("Built-in checks are not able to be edited.", "Warning");
                return;
            }
            else if (!isLocal(rowData) && !_checkManager.IsCurrentUserRemoteAdmin())
            {
                // Dialog box that shows if a user attempts to edit a check as a non-admin
                MessageBox.Show("Only administrators can edit non-local checks and groups.", "Warning");
                return;
            }

            var checkDir = isLocal(rowData)
                ? _checkManager.GetLocalRepoDirectory()
                : _checkManager.GetInstalledChecksDirectory();

            // Get the file location for the selected check
            var check = rowData.Item as CheckAndFixItem;
            if (check != null)
            {
                var fileName = _checkManager.GetItemFilename(check);
                var fullPath = Path.Combine(checkDir, fileName);

                // Open the CheckEditor with the selected check
                new CheckEditor(_checkManager, new FileInfo(fullPath), !isLocal(rowData)).ShowDialog(this);
            } 
            else
            {
                var group = rowData.Item as CheckGroup;

                // TODO: get file name & open in editor
            }

            UpdateGridData();
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
            _connectProgressForm.Show(this);
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
                    _syncProgressForm.Hide(); // Ensure that the synchronization form is hidden if it was shown elsewhere.
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
            _connectProgressForm.Hide();
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
            _syncProgressForm.Hide();
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
            UpdateRefreshTooltip(DateTime.Now);
            CheckManager.HasSyncRun = true;
            refreshButton.Enabled = true;
            UpdateGridData();
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
        private void deleteContextMenuItem_Click(object sender, EventArgs e)
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
            var rowData = (GridRowData) checksList.Rows[rowIndex].Tag;
            IRunnable item = rowData.Item;
            if (_localChecks.Contains(item))
            {
                _checkManager.DeleteItem(item);
                _localChecks.Remove(item);
            }
            else if (_installedChecks.Contains(item))
            {
                _checkManager.UnpublishAndUninstallItem(item);
                _installedChecks.Remove(item);
            }
            checksList.Rows.RemoveAt(rowIndex);
        }

        /// <summary>
        /// Checks to see if inactive rows are selected. A row is inactivated
        /// if it contains an item that can not be run on the current project.
        /// </summary>
        /// <param name="dataGridView">The data grid to check.</param>
        /// <returns>true if inactive rows are selected.</returns>
        private bool inactiveChecksAreSelected(DataGridView dataGridView)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Selected && row.Tag != null)
                {
                    var rowData = (GridRowData) row.Tag;
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
                if (!AllSelectedRowsAreDeletable())
                {
                    MessageBox.Show("Built-in checks cannot be deleted. Unselect any built-in checks and try again.");
                } else
                {
                    deleteSelectedRows();
                }
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
                    var rowData = (GridRowData)e.Row.Tag;
                    rowData.Selected = e.Row.Selected;
                }

                runChecksButton.Enabled = !inactiveChecksAreSelected(e.Row.DataGridView);
            }
        }

        /// <summary>
        /// Handles right mouse button clicks.
        /// - When an unselected row is right clicked, select it and unselect all other rows.
        /// - If an item row is right-clicked enable the context menu.
        /// - Disable the edit option if the selected row is not editable.
        /// - Disable the delete option if any of the selected rows are not deleteable
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void checksList_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView dataGridView = sender as DataGridView;
            if (e.RowIndex != -1 && e.Button == MouseButtons.Right)
            {
                // When an unselected row is right clicked, select it and unselect all other rows.
                DataGridViewRow row = dataGridView.Rows[e.RowIndex];
                if (!row.Selected)
                {
                    row.DataGridView.ClearSelection();
                    row.DataGridView.CurrentCell = row.Cells[0];
                    row.Selected = true;
                }

                // If a data row is right clicked enable the context menu.
                checksList.ContextMenuStrip = checksListContextMenu;

                if (checksList.SelectedRows.Count != 1 ||
                    !RowIsEditable(checksList.SelectedRows[0]))
                {
                    editContextMenuItem.Enabled = false;
                }
                // Disable the delete option on the context menu if any selected rows are not deleteable
                if (!AllSelectedRowsAreDeletable())
                {
                    deleteContextMenuItem.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Disable the checks list context menu.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void checksList_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            checksList.ContextMenuStrip = null;
        }

        /// <summary>
        /// Checks whether this row is editable. 
        /// </summary>
        /// <param name="row">The row to check.</param>
        /// <returns>true if the row is editable. false otherwise.</returns>
        private bool RowIsEditable(DataGridViewRow row)
        {
            var item = (GridRowData)row.Tag;
            // Built-in checks cannot be edited.
            if (isBuiltIn(item))
            {
                return false;
            }

            // Non-admins can only edit local checks.
            if (!isLocal(item) && !_checkManager.IsCurrentUserRemoteAdmin())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks whether a row in the checks list is deletable.
        /// </summary>
        /// <param name="row">The row to check</param>
        /// <returns>true if the row is deletable. false otherwise.</returns>
        private bool RowIsDeletable(DataGridViewRow row)
        {
            var item = row.Tag as GridRowData;
            if (item == null || isBuiltIn(item))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verify whether all selected rows in the checks list are deletable.
        /// </summary>
        /// <returns>true if all selected rows are deletable. false otherwise.</returns>
        private bool AllSelectedRowsAreDeletable()
        {
            foreach (DataGridViewRow row in checksList.Rows)
            {
                if (row.Selected && !RowIsDeletable(row))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Reenable disabled menu items when the context menu closes.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void checksListContextMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            editContextMenuItem.Enabled = true;
            deleteContextMenuItem.Enabled = true;
        }

        /// <summary>
        /// Start the check/fix editor from the menu
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void newCheckMenuItem_Click(object sender, EventArgs e)
        {
            new CheckEditor(_checkManager).ShowDialog(this);
            UpdateGridData();
        }

        /// <summary>
        /// Handles the Options menu item. Opens the Options form to modify TVP settings.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void optionsMenuItem_Click(object sender, EventArgs e)
        {
            TvpOptions oldOptions = _optionsManager.LoadOptions();
            var form = new OptionsForm(oldOptions);
            DialogResult result = form.ShowDialog(this);
            if (result == DialogResult.OK && form.HasChanges())
            {
                TvpOptions newOptions = form.getOptions();
                _optionsManager.SaveOptions(newOptions);
                _checkManager.SetupRemoteRepository();
                string error;
                if (!_checkManager.RemoteRepositoryIsVerified(out error) && !String.IsNullOrEmpty(error))
                {
                    var message = "The shared repository has been disabled because the settings could not be " +
                        $"verified. See the message below for more information.\n\n{error}";
                    MessageBox.Show(message, "Verification Failed");
                }
                else if (_checkManager.RemoteRepositoryIsEnabled())
                {
                    refreshButton.Visible = true;
                    ConnectAndSync(true);
                }
                else
                {
                    refreshButton.Visible = false;
                    UpdateGridData();
                    UpdateGrid();
                }
            }
        }

        /// <summary>
        /// Handles the Edit context menu item. Opens a check in the check editor form.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void editContextMenuItem_Click(object sender, EventArgs e)
        {
            if (checksList.SelectedRows.Count != 1 || !RowIsEditable(checksList.SelectedRows[0]))
            {
                return;
            }

            var rowData = (GridRowData)checksList.SelectedRows[0].Tag;

            var checkDir = isLocal(rowData)
                ? _checkManager.GetLocalRepoDirectory()
                : _checkManager.GetInstalledChecksDirectory();

            // Get the file location for the selected check
            var fileName = _checkManager.GetItemFilename(rowData.Item);
            var fullPath = Path.Combine(checkDir, fileName);

            // Open the CheckEditor with the selected check
            new CheckEditor(_checkManager, new FileInfo(fullPath), !isLocal(rowData)).ShowDialog(this);

            UpdateGridData();
        }
    }
}