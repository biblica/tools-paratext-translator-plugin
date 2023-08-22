/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Paratext.Data.ProjectProgress;
using Paratext.LexicalContracts;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TvpMain.Check;
using TvpMain.CheckManagement;
using TvpMain.Util;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace TvpMain.Forms
{
    /// <summary>
    /// Allows for editing of CFitems
    /// </summary>
    public partial class GroupEditorForm : Form
    {
        // Keep track when changes are made in the UI.
        private bool _dirty;

        private bool Dirty
        {
            get
            {
                return _dirty;
            }

            set
            {
                _dirty = value;
                saveIconToolStripMenuItem.Enabled = _dirty;
                saveToolStripMenuItem.Enabled = _dirty;
            }
        }

        private readonly ICheckManager _checkManager;

        private CheckGroup _checkGroup;
        private CheckGroup _lastCheckGroupSave = null;

        /// <summary>
        /// Max number of characters in a line number
        /// </summary>
        private int _maxLineNumberCharLength = 5;

        /// <summary>
        /// a set of JavaScript keywords
        /// </summary>
        private const string JS_KEYWORDS = "break case catch class const continue debugger default delete do else export extends finally " + "for function if import in instanceof new return super switch this throw try typeof var void while with yield " + "enum implements interface let package private protected public static yield await abstract boolean byte char " + "double final float goto int long native short synchronized throws transient volatile";
        
        /// <summary>
        /// Simple progress bar form for when the checks are being synchronized
        /// </summary>
        private GenericProgressForm _progressForm;

        /// <summary>
        /// Name of the repository where the group being edited is stored. 
        /// </summary>
        private string RepositoryName { get; set; }

        /// <summary>
        /// Whether the editor is editing a remote check
        /// </summary>
        private bool IsRemote {
            get
            {
                return RepositoryName == MainConsts.REMOTE_REPO_NAME;
            } 
        }

        /// <summary>
        /// Constructor for editing a new check group.
        /// </summary>
        /// <param name="checkManager"></param>
        /// <param name="repositoryName">Name of the repository where this group is located.</param>\
        public GroupEditorForm(ICheckManager checkManager, string repositoryName)
        {
            if (checkManager is null) throw new ArgumentNullException(nameof(checkManager));
            if (repositoryName is null) throw new ArgumentNullException(nameof(repositoryName));
            if (String.IsNullOrWhiteSpace(repositoryName)) throw new ArgumentException(nameof(repositoryName));

            InitializeComponent();
            _checkManager = checkManager;
            _checkGroup = new CheckGroup();
            RepositoryName = repositoryName;
        }

        /// <summary>
        /// Constructor for editing an existing check group.
        /// </summary>
        /// <param name="checkManager"></param>
        /// <param name="group">The group to open in the editor.</param>
        /// <param name="repositoryName">Name of the repository where this group is located.</param>\
        public GroupEditorForm(ICheckManager checkManager, CheckGroup group, string repositoryName)
        {
            InitializeComponent();
            _checkManager = checkManager;
            _checkGroup = (CheckGroup)group.Clone();
            RepositoryName = repositoryName;
        }

        /// <summary>
        /// On dialog load, set to 'new' state
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void CheckEditor_Load(object sender, EventArgs e)
        {
            if (_checkGroup == null)
            {
                NewGroup();
            }
            else
            {
                UpdateForm();
                Dirty = false;
            }
        }

        /// <summary>
        /// Set the form to a "new" state with a brand new group to edit
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void NewGroup()
        {
            // prevent overwriting changes unless explicit
            if (_dirty)
            {
                var dialogResult = MessageBox.Show(
                    @"You have unsaved changes, are you sure you wish to proceed?",
                    @"Verify", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }

            _checkGroup = new CheckGroup();
            UpdateForm();
            Dirty = false;
        }

        public CheckGroup GetCheckGroup()
        {
            return _lastCheckGroupSave;
        }

        /// <summary>
        /// Set the form to a "new" state; a brand new group item to edit
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGroup();
        }

        /// <summary>
        /// Open a check group file for editing
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // prevent overwriting changes unless explicit
            if (_dirty)
            {
                var dialogResult = MessageBox.Show(
                    @"You have unsaved changes, are you sure you wish to proceed?",
                    @"Verify", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }

            using var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = _checkManager.GetLocalRepoDirectory(),
                Filter = @"Check group files (*.group.xml)|*.group.xml"
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using var fileStream = openFileDialog.OpenFile();
            _checkGroup = CheckGroup.LoadFromXmlContent(fileStream);
            RepositoryName = MainConsts.LOCAL_REPO_NAME;
            UpdateForm();
            Dirty = false;
        }

        /// <summary>
        /// Save check group settings to a file.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!UpdateCheckGroup() || !VerifyCheckGroup()) return;

            if (IsRemote)
            {
                var publishResult = MessageBox.Show(@"Are you sure you would like to save this group?",
    @"Confirm Save", MessageBoxButtons.YesNo);
                if (publishResult == DialogResult.Yes)
                {
                    SaveToRemote();
                }
            } 
            else
            {
                if (_checkManager.SaveItem(_checkGroup))
                {
                    _lastCheckGroupSave = (CheckGroup)_checkGroup.Clone();
                    Dirty = false;
                }
            }
        }

        /// <summary>
        /// Exit this dialog
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Save the check group to the remote repository.
        /// </summary>
        private void SaveToRemote()
        {
            _progressForm = new GenericProgressForm("Saving group...");
            _progressForm.Show(this);

            publishWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Worker for doing publish updates asynchronously
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void PublishWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _checkManager.SynchronizeInstalledChecks();
            var remoteChecks = _checkManager.GetInstalledItems();
            var found = false;

            foreach (var checkAndFixItem in remoteChecks.Where(checkAndFixItem =>
                checkAndFixItem.Id.Equals(_checkGroup.Id) && checkAndFixItem.Version.Equals(_checkGroup.Version)))
            {
                found = true;
            }

            if (found)
            {
                MessageBox.Show(@"This version of the group already exists in the repository, you must increment the version before trying to publish.",
                    @"Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Result = false;
            }
            else
            {
                _checkManager.PublishItem(_checkGroup);
                e.Result = true;
            }
        }

        /// <summary>
        /// Callback for when the async worker is complete
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void PublishWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _progressForm.Close();
            
            if ((bool)e.Result)
            {
                _lastCheckGroupSave = (CheckGroup)_checkGroup.Clone();
                Dirty = false;
            }
        }

        /// <summary>
        /// Update the form when a check group is loaded.
        /// </summary>
        private void UpdateForm()
        {
            locationLabel.Text = RepositoryName;
            groupIdLabel.Text = _checkGroup.Id ?? string.Empty;
            checkFixNameTextBox.Text = _checkGroup.Name ?? string.Empty;
            versionTextBox.Text = _checkGroup.Version ?? string.Empty;
            scopeCombo.SelectedItem = _checkGroup.Scope.ToString();
            defaultDescTextBox.Text = _checkGroup.DefaultDescription ?? string.Empty;
            updateLanguagesLabel();
            updateTagsLabel();
            descriptionTextBox.Text = _checkGroup.Description;

            List<CheckAndFixItem> checks;
            if (IsRemote)
            {
                checks = _checkManager.GetInstalledChecks();
            }
            else
            {
                checks = _checkManager.GetLocalChecks();
            }

            availableChecks.Items.Clear();
            foreach (CheckAndFixItem check in checks)
            {
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Text = check.Name;
                listViewItem.Tag = check;
                listViewItem.ToolTipText = "Check: " + check.Name + Environment.NewLine;
                listViewItem.ToolTipText += "Description: " + check.Description;
                availableChecks.Items.Add(listViewItem);
            }
            SetListViewColumnWidth(availableChecks);

            groupChecks.Items.Clear();
            foreach (KeyValuePair<string, CheckAndFixItem> checkKvp in _checkGroup.Checks)
            {
                ListViewItem listViewItem = new ListViewItem();
                var foundCheck = checkKvp.Value is null ? checks.Find(check => check.Id == checkKvp.Key) : checkKvp.Value;
                if (foundCheck is null)
                {
                    // The check has been deleted.
                    listViewItem.Text = checkKvp.Key;
                    listViewItem.Tag = null;
                    listViewItem.ToolTipText = "Check ID: " + checkKvp.Key;
                    listViewItem.ToolTipText += Environment.NewLine + "NOTE: This check could not be found. It may have been deleted.";
                    listViewItem.ForeColor = Color.DarkRed;
                    groupChecks.Items.Add(listViewItem);
                }
                else
                {
                    listViewItem.Text = foundCheck.Name;
                    listViewItem.Tag = foundCheck;
                    listViewItem.ToolTipText = "Check: " + foundCheck.Name + Environment.NewLine;
                    listViewItem.ToolTipText += "Description: " + foundCheck.Description;
                    groupChecks.Items.Add(listViewItem);
                }
            }
            SetListViewColumnWidth(groupChecks);
        }

        /// <summary>
        /// Updates the language information displayed on the form.
        /// </summary>
        private void updateLanguagesLabel()
        {
            if (_checkGroup.Languages == null)
            {
                languagesLabel.Text = "<INVALID>";

            } 
            else if (_checkGroup.Languages.Count() == 0)
            {
                languagesLabel.Text = "All";
            } 
            else
            {
                languagesLabel.Text = string.Join(", ", _checkGroup.Languages);
            }
        }

        /// <summary>
        /// Updates the tag information displayed on the form.
        /// </summary>
        private void updateTagsLabel()
        {
            if (_checkGroup.Tags == null)
            {
                tagsLabel.Text = "<INVALID>";
            }
            else if (_checkGroup.Tags.Count() == 0)
            {
                tagsLabel.Text = "";
            }
            else
            {
                tagsLabel.Text = string.Join(", ", _checkGroup.Tags);
            }
        }

        /// <summary>
        /// Adjust the colunn width of a ListView control to prevent showing horizontal scrollbars.
        /// </summary>
        /// <param name="listView">The ListView control</param>
        private void SetListViewColumnWidth(ListView listView)
        {
            if (listView.Height < (4 + (listView.Items.Count * 17)))
            {
                listView.Columns[0].Width = listView.Width - 21;
            }
            else
            {
                listView.Columns[0].Width = listView.Width - 4;
            }
        }

        /// <summary>
        /// Trigger column resizing when the size of the groupChecks ListView control changes.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void groupChecks_Resize(object sender, EventArgs e)
        {
            if (sender is ListView listView)
            {
                SetListViewColumnWidth(listView);
            }
        }

        /// <summary>
        /// Trigger column resizing when the size of the availableChecks ListView control changes.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void availableChecks_Resize(object sender, EventArgs e)
        {
            if (sender is ListView listView)
            {
                SetListViewColumnWidth(listView);
            }
        }

        /// <summary>
        /// An error dialog to show when a check is not well-formed
        /// </summary>
        private readonly Func<DialogResult> _verificationErrorDialog = () => MessageBox.Show(@"Name, Version, and Default Description must be entered. At least one check must be added to the checks to run list.",
            @"Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);

        /// <summary>
        /// Update the group from the form controls.
        /// </summary>
        /// <returns>Whether the update succeeded</returns>
        private bool UpdateCheckGroup()
        {
            try
            {
                _checkGroup.Name = checkFixNameTextBox.Text.Trim();
                _checkGroup.Version = versionTextBox.Text.Trim();
                //_checkGroup.Scope = (CheckAndFixItem.CheckScope)scopeCombo.SelectedIndex;
                _checkGroup.DefaultDescription = defaultDescTextBox.Text.Trim();
                _checkGroup.Description = descriptionTextBox.Text;
                _checkGroup.Checks.Clear();
                foreach (ListViewItem listViewItem in groupChecks.Items)
                {
                    if (listViewItem.Tag is CheckAndFixItem check)
                    {
                        _checkGroup.AddCheck(new KeyValuePair<string, CheckAndFixItem>(check.Id, check));
                    }
                    else
                    {
                        _checkGroup.AddCheck(new KeyValuePair<string, CheckAndFixItem>(listViewItem.Text, null));
                    }
                }
            }
            catch
            {
                _verificationErrorDialog();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the current check group is well-formed
        /// </summary>
        /// <returns>Whether the current check group is well-formed</returns>
        private bool VerifyCheckGroup()
        {
            if (!string.IsNullOrEmpty(_checkGroup.Name.Trim()) &&
                !string.IsNullOrEmpty(_checkGroup.Version.Trim()) &&
                !string.IsNullOrEmpty(_checkGroup.DefaultDescription.Trim()) &&
                _checkGroup.Checks.Count > 0) return true;
            
            // Something isn't right--show an error and return false
            _verificationErrorDialog();
            return false;

        }

        /// <summary>
        /// Keep track of changes and mark the form dirty.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void Content_Changed(object sender, EventArgs e)
        {
            Dirty = true;
        }

        /// <summary>
        /// Update the help text for the fix regex control
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void FixRegExTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The regular expression replacement pattern, using $1 type replacement values from the groupings found in the check find regex.");
        }

        /// <summary>
        /// Update the help text for the group id label control
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void CheckFixIdLabel_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The automatically assigned unique identifier.");
        }

        /// <summary>
        /// Update the help text for the group name text box
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void CheckFixNameTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The name of your group.");
        }

        /// <summary>
        /// Update the help text for the version text box
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void VersionTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The version of the group. Increment each time you publish an update. Use semantic versioning scheme: https://semver.org/");
        }

        /// <summary>
        /// Update the help text for the scope combo box
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void ScopeCombo_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The text scope to run this group at; PROJECT, BOOK, CHAPTER, VERSE" + Environment.NewLine);
            helpTextBox.AppendText("Leave defaulted to VERSE if unsure.");
        }

        /// <summary>
        /// Update the help text for the group description text box
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void DefaultDescTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("This is the default description associated with matched results.");
        }

        /// <summary>
        /// Update the help text for the languages label.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void LanguagesLabel_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The languages that this group applies to. The languages " +
                "supported by a group are determined by the languages of the checks it references." + Environment.NewLine);
        }

        /// <summary>
        /// Update the help text for the tags label.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void TagsLabel_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The tags which apply to this group. The group's tags are " +
                "determined by the checks it references." + Environment.NewLine);
        }

        /// <summary>
        /// Update the help text for the description text box
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void DescriptionTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.Text = @"Enter the full description for this group.";
        }

        /// <summary>
        /// A callback for handling when a form is closing
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_dirty)
            {
                return;
            }

            var dialogResult = MessageBox.Show(@"You've made changes. Are you sure you wish to exit without saving?",
                @"Exit?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No)
            {
                e.Cancel = true;
            }
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
        private void LicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUtil.StartLicenseForm();
        }
        
        /// <summary>
        /// Add the selected check to this group. 
        /// </summary>
        private void AddCheckToGroup()
        {
            if (availableChecks.SelectedIndices.Count < 1) return;
            int currentIndex = availableChecks.SelectedIndices[0];
            groupChecks.Items.Add((ListViewItem)availableChecks.Items[currentIndex].Clone());
            SetListViewColumnWidth(groupChecks);
            UpdateCheckGroup();
            updateLanguagesLabel();
            updateTagsLabel();

            Dirty = true;
        }

        /// <summary>
        /// Handle "Add" button clicks.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void addToGroupButton_Click(object sender, EventArgs e)
        {
            AddCheckToGroup();
        }

        /// <summary>
        /// Handle "Remove" button clicks.
        /// Remove the selected check from the group.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void removeFromGroupButton_Click(object sender, EventArgs e)
        {
            if (groupChecks.SelectedIndices.Count < 1) return;
            int currentIndex = groupChecks.SelectedIndices[0];
            groupChecks.Items.RemoveAt(currentIndex);
            int newSelectedIndex = currentIndex == groupChecks.Items.Count ? currentIndex - 1 : currentIndex;
            groupChecks.SelectedIndices.Clear();
            groupChecks.SelectedIndices.Add(newSelectedIndex);
            SetListViewColumnWidth(groupChecks);
            UpdateCheckGroup();
            updateLanguagesLabel();
            updateTagsLabel();

            Dirty = true;
        }

        /// <summary>
        /// Handle "Up" button clicks. 
        /// Move the selected check higher in this group's list of checks.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void moveUpButton_Click(object sender, EventArgs e)
        {
            if (groupChecks.SelectedIndices.Count < 1) return;
            int currentIndex = groupChecks.SelectedIndices[0];
            if (currentIndex == 0) return;
            int newIndex = currentIndex - 1;
            var itemToMove = groupChecks.Items[currentIndex];
            groupChecks.Items.RemoveAt(currentIndex);
            groupChecks.Items.Insert(newIndex, itemToMove);
            groupChecks.SelectedIndices.Clear();
            groupChecks.SelectedIndices.Add(newIndex);

            Dirty = true;
        }

        /// <summary>
        /// Handle "Down" button clicks. 
        /// Move the selected check lower in this group's list of checks.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void moveDownButton_Click(object sender, EventArgs e)
        {
            if (groupChecks.SelectedIndices.Count < 1) return;
            int currentIndex = groupChecks.SelectedIndices[0];
            if (currentIndex == (groupChecks.Items.Count - 1)) return;
            int newIndex = currentIndex + 1;
            var itemToMove = groupChecks.Items[currentIndex];
            groupChecks.Items.RemoveAt(currentIndex);
            groupChecks.Items.Insert(newIndex, itemToMove);
            groupChecks.SelectedIndices.Clear();
            groupChecks.SelectedIndices.Add(newIndex);

            Dirty = true;
        }

        /// <summary>
        /// Display scroll bars in the help text box when needed.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void helpTextBox_TextChanged(object sender, EventArgs e)
        {
            if (helpTextBox.Lines.Count() > 7)
            {
                helpTextBox.ScrollBars = ScrollBars.Vertical;
            }
            else
            {
                helpTextBox.ScrollBars = ScrollBars.None;
            }
        }

        /// <summary>
        /// Update the help text for the availableChecks ListView control.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void availableChecks_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("A list of checks which are available to add to this group.");
        }

        /// <summary>
        /// Update the help text for the groupChecks ListView control.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void groupChecks_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("A list of checks which are part of this group.");
        }

        /// <summary>
        /// When an available check is double-clicked, add it to the group.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void availableChecks_DoubleClick(object sender, EventArgs e)
        {
            AddCheckToGroup();
        }
    }
}
