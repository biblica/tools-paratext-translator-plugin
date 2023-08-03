/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using ScintillaNET;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TvpMain.Check;
using TvpMain.CheckManagement;
using TvpMain.Util;

namespace TvpMain.Forms
{
    /// <summary>
    /// Allows for editing of CFitems
    /// </summary>
    public partial class CheckEditor : Form
    {
        // Keep track when changes are made in the UI.
        private bool _dirty;

        private readonly ICheckManager _checkManager;

        private CheckAndFixItem _checkAndFixItem;

        /// <summary>
        /// Max number of characters in a line number
        /// </summary>
        private int _maxLineNumberCharLength = 5;

        /// <summary>
        /// a set of JavaScript keywords
        /// </summary>
        private const string JS_KEYWORDS = "break case catch class const continue debugger default delete do else export extends finally " + "for function if import in instanceof new return super switch this throw try typeof var void while with yield " + "enum implements interface let package private protected public static yield await abstract boolean byte char " + "double final float goto int long native short synchronized throws transient volatile";

        /// <summary>
        /// Text to display in the Save button when a local check is displayed
        /// </summary>
        private const string SAVE_BUTTON_LOCAL_TEXT = @"Save";
        
        /// <summary>
        /// Text to display in the Save button when a remote check is displayed
        /// </summary>
        private const string SAVE_BUTTON_REMOTE_TEXT = @"Save a Copy";
        
        /// <summary>
        /// Simple progress bar form for when the checks are being synchronized
        /// </summary>
        private GenericProgressForm _progressForm;

        /// <summary>
        /// Backing field. Enables IsRemote to trigger changes when set
        /// </summary>
        private bool _isRemote = false;

        /// <summary>
        /// Whether the editor is editing a remote check
        /// </summary>
        private bool IsRemote
        {
            get => _isRemote;
            set
            {
                // When a check starts as remote, indicate that a copy can be saved locally
                saveToolStripMenuItem.Text = value ? SAVE_BUTTON_REMOTE_TEXT : SAVE_BUTTON_LOCAL_TEXT;
                _isRemote = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public CheckEditor()
        {
            InitializeComponent();
            _checkManager = new CheckManager();
        }
        /// <summary>
        /// Constructor for opening with a specific check loaded
        /// </summary>
        /// <param name="checkAndFixFile">The file to open in the editor.</param>
        /// <param name="isRemote">Whether the file represents a remote check. (Default = false)</param>
        public CheckEditor(FileInfo checkAndFixFile, bool isRemote = false)
        {
            InitializeComponent();
            _checkManager = new CheckManager();
            IsRemote = isRemote;

            using var fileStream = checkAndFixFile.OpenRead();
            _checkAndFixItem = CheckAndFixItem.LoadFromXmlContent(fileStream);
        }

        /// <summary>
        /// On dialog load, set to 'new' state
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void CheckEditor_Load(object sender, EventArgs e)
        {
            if (_checkAndFixItem == null)
            {
                NewToolStripMenuItem_Click(sender, e);
            }

            UpdateUi();
            _dirty = false;
            
            publishToolStripMenuItem.Visible = HostUtil.Instance.IsOnline && HostUtil.Instance.IsCurrentUserTvpAdmin();
            saveIconToolStripMenuItem.Enabled = IsRemote || _dirty;
            saveToolStripMenuItem.Enabled = IsRemote || _dirty;

            SetScintillaRecipe();
        }

        /// <summary>
        /// Set the form to a "new" state; a brand new check/fix item to edit
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
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

            _checkAndFixItem = new CheckAndFixItem
            {
                CheckScript = @"function checkAndFix(checkResultItems) {
  return checkResultItems
}
",
                Scope = CheckAndFixItem.CheckScope.VERSE,
                Id = Guid.NewGuid().ToString()
            };
            checkFixIdLabel.Text = _checkAndFixItem.Id;
            IsRemote = false;

            UpdateUi();
            _dirty = false;
            saveIconToolStripMenuItem.Enabled = _dirty;
            saveToolStripMenuItem.Enabled = _dirty;
        }

        /// <summary>
        /// Open a check/fix file for editing
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
                Filter = @"check/fix files (*.xml)|*.xml"
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using var fileStream = openFileDialog.OpenFile();
            _checkAndFixItem = CheckAndFixItem.LoadFromXmlContent(fileStream);
            IsRemote = false;

            UpdateUi();
            _dirty = false;
            saveIconToolStripMenuItem.Enabled = _dirty;
            saveToolStripMenuItem.Enabled = _dirty;
        }

        /// <summary>
        /// Save the file that represents the check/fix
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!UpdateCheckAndFix() || !VerifyCheckAndFix())
            {
                return;
            };

            _checkManager.SaveCheckAndFixItem(_checkAndFixItem);
            IsRemote = false;
            _dirty = false;

            saveIconToolStripMenuItem.Enabled = _dirty;
            saveToolStripMenuItem.Enabled = _dirty;
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
        /// Save and Publish the check/fix
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void PublishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!UpdateCheckAndFix() || !VerifyCheckAndFix())
            {
                return;
            };
            
            var publishResult = MessageBox.Show(@"Are you sure you wish to publish this check/fix?",
                @"Publish?", MessageBoxButtons.YesNo);
            if (publishResult != DialogResult.Yes)
            {
                return;
            }

            // It's expected that a remote check might be published without saving, but not a local check
            if (_dirty && !IsRemote)
            {
                var changesResult = MessageBox.Show(
                    @"You have unsaved changes. Would you like to save your changes before publishing?",
                    @"Exit?", MessageBoxButtons.YesNo);
                if (changesResult == DialogResult.Yes)
                {
                    SaveToolStripMenuItem_Click(sender, e);
                }
            }

            _progressForm = new GenericProgressForm("Publishing check/fix item...");
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
            var remoteChecks = _checkManager.GetInstalledCheckAndFixItems();
            var found = false;

            foreach (var checkAndFixItem in remoteChecks.Where(checkAndFixItem =>
                checkAndFixItem.Id.Equals(_checkAndFixItem.Id) && checkAndFixItem.Version.Equals(_checkAndFixItem.Version)))
            {
                found = true;
            }

            if (found)
            {
                MessageBox.Show(@"This version of the Check/Fix already exists in the repository, you must increment the version before trying to publish.",
                    @"Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                _checkManager.PublishCheckAndFixItem(_checkAndFixItem);
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
        }

        /// <summary>
        /// Update the UI when a CFitem is loaded
        /// </summary>
        private void UpdateUi()
        {
            checkFixIdLabel.Text = _checkAndFixItem.Id ?? string.Empty;
            checkFixNameTextBox.Text = _checkAndFixItem.Name ?? string.Empty;
            versionTextBox.Text = _checkAndFixItem.Version ?? string.Empty;
            scopeCombo.SelectedItem = _checkAndFixItem.Scope.ToString();
            defaultDescTextBox.Text = _checkAndFixItem.DefaultItemDescription ?? string.Empty;
            languagesTextBox.Text = _checkAndFixItem.Languages == null
                ? string.Empty
                : string.Join(", ", _checkAndFixItem.Languages);
            tagsTextBox.Text = _checkAndFixItem.Tags == null
                ? string.Empty
                : string.Join(", ", _checkAndFixItem.Tags);
            descriptionTextBox.Text = _checkAndFixItem.Description;

            checkFindRegExTextBox.Text = _checkAndFixItem.CheckRegex ?? string.Empty;
            fixRegExTextBox.Text = _checkAndFixItem.FixRegex ?? string.Empty;
            jsEditor.Text = _checkAndFixItem.CheckScript == null
                ? string.Empty
                : _checkAndFixItem.CheckScript.Replace("\n", Environment.NewLine);
        }
        
        /// <summary>
        /// An error dialog to show when a check is not well-formed
        /// </summary>
        private readonly Func<DialogResult> _verificationErrorDialog = () => MessageBox.Show(@"Name, Version, Default Description, and either the Check Regex or the Check Script, must be entered.",
            @"Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);

        /// <summary>
        /// Update the CFitem from the UI before saves
        /// </summary>
        /// <returns>Whether the update succeeded</returns>
        private bool UpdateCheckAndFix()
        {
            try
            {
                _checkAndFixItem.Name = checkFixNameTextBox.Text;
                _checkAndFixItem.Version = versionTextBox.Text;
                _checkAndFixItem.Scope = (CheckAndFixItem.CheckScope)scopeCombo.SelectedIndex;
                _checkAndFixItem.DefaultItemDescription = defaultDescTextBox.Text;
                _checkAndFixItem.Languages = languagesTextBox.Text.Trim().Split(',')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToArray();
                _checkAndFixItem.Tags = tagsTextBox.Text.Trim().Split(',').Select(x => x.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToArray();
                _checkAndFixItem.Description = descriptionTextBox.Text;

                _checkAndFixItem.CheckRegex = checkFindRegExTextBox.Text;
                _checkAndFixItem.FixRegex = fixRegExTextBox.Text;
                _checkAndFixItem.CheckScript = jsEditor.Text;
            }
            catch
            {
                _verificationErrorDialog();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the current check is well-formed
        /// </summary>
        /// <returns>Whether the current check is well-formed</returns>
        private bool VerifyCheckAndFix()
        {
            if (!string.IsNullOrEmpty(_checkAndFixItem.Name.Trim()) &&
                !string.IsNullOrEmpty(_checkAndFixItem.Version.Trim()) &&
                !string.IsNullOrEmpty(_checkAndFixItem.DefaultItemDescription.Trim()) &&
                (!string.IsNullOrEmpty(_checkAndFixItem.CheckRegex.Trim()) ||
                 !string.IsNullOrEmpty(_checkAndFixItem.CheckScript.Trim()))) return true;
            
            // Something isn't right--show an error and return false
            _verificationErrorDialog();
            return false;

        }

        /// <summary>
        /// Keep track of changes and mark the form dirty
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void Content_TextChanged(object sender, EventArgs e)
        {
            _dirty = true;
            saveIconToolStripMenuItem.Enabled = _dirty;
            saveToolStripMenuItem.Enabled = _dirty;
        }

        /// <summary>
        /// Update the help text for the regex control
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void CheckFindRegExTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The regular expression to find issues. This value may be empty if the check/fix relies on JavaScript to perform modifications.");
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
        /// Update the help text for the check/fix id label control
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void CheckFixIdLabel_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The automatically assigned unique identifier.");
        }

        /// <summary>
        /// Update the help text for the check/fix name text box
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void CheckFixNameTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The name of your check/fix.");
        }

        /// <summary>
        /// Update the help text for the version text box
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void VersionTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The version of the check/fix. Increment each time you publish an update. Use semantic versioning scheme: https://semver.org/");
        }

        /// <summary>
        /// Update the help text for the scope combo box
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void ScopeCombo_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The text scope to run this check/fix at; PROJECT, BOOK, CHAPTER, VERSE" + Environment.NewLine);
            helpTextBox.AppendText("Leave defaulted to VERSE if unsure.");
        }

        /// <summary>
        /// Update the help text for the check/fix description text box
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void DefaultDescTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("This is the default description associated with matched results.");
        }

        /// <summary>
        /// Update the help text for the languages text box
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void LanguagesTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("Enter Languages associated with this check/fix. Separate lanugages by comma." + Environment.NewLine);
            helpTextBox.AppendText("Use language codes found in projects like eng-US, zh, ja, etc.");
        }

        /// <summary>
        /// Update the help text for the tags text box
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void TagsTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("Enter Tags associated with this check/fix. Separate tags by comma." + Environment.NewLine);
            helpTextBox.AppendText("Currently supported tags: RTL = right to left only languages.");
        }

        /// <summary>
        /// Update the help text for the description text box
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void DescriptionTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.Text = @"Enter the full description for this check/fix.";
        }

        /// <summary>
        /// Creates and sets the style information and keywords for JavaScript
        /// </summary>
        private void SetScintillaRecipe()
        {
            // Configuring the default style with properties
            // we have common to every lexer style saves time.
            jsEditor.StyleResetDefault();
            jsEditor.Styles[Style.Default].Font = "Consolas";
            jsEditor.Styles[Style.Default].Size = 10;
            jsEditor.StyleClearAll();

            // Configure the CPP (C#) lexer styles
            jsEditor.Styles[Style.Cpp.Default].ForeColor = Color.Silver;
            jsEditor.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            jsEditor.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0); // Green
            jsEditor.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.FromArgb(128, 128, 128); // Gray
            jsEditor.Styles[Style.Cpp.Number].ForeColor = Color.Olive;
            jsEditor.Styles[Style.Cpp.Word].ForeColor = Color.Blue;
            jsEditor.Styles[Style.Cpp.Word2].ForeColor = Color.Blue;
            jsEditor.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
            jsEditor.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21); // Red
            jsEditor.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(163, 21, 21); // Red
            jsEditor.Styles[Style.Cpp.StringEol].BackColor = Color.Pink;
            jsEditor.Styles[Style.Cpp.Operator].ForeColor = Color.Purple;
            jsEditor.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Maroon;
            jsEditor.Lexer = Lexer.Cpp;

            jsEditor.SetKeywords(0, JS_KEYWORDS);
        }

        /// <summary>
        /// On text change, look to increase the size of the margin to handle full line numbers
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void JsEditor_TextChanged(object sender, EventArgs e)
        {
            // Did the number of characters in the line number display change?
            // i.e. nnn VS nn, or nnnn VS nn, etc...
            var maxLineNumberCharLength = jsEditor.Lines.Count.ToString().Length;
            if (maxLineNumberCharLength == _maxLineNumberCharLength)
                return;

            // Calculate the width required to display the last line number
            // and include some padding for good measure.
            const int padding = 2;
            jsEditor.Margins[0].Width = jsEditor.TextWidth(Style.LineNumber, new string('9', maxLineNumberCharLength + 1)) + padding;
            _maxLineNumberCharLength = maxLineNumberCharLength;
        }

        /// <summary>
        /// While characters are being added, allow for hints for keywords
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void JsEditor_CharAdded(object sender, CharAddedEventArgs e)
        {
            // Find the word start
            var currentPos = jsEditor.CurrentPosition;
            var wordStartPos = jsEditor.WordStartPosition(currentPos, true);

            // Display the auto-completion list
            var lenEntered = currentPos - wordStartPos;
            if (lenEntered <= 0)
            {
                return;
            }
            if (!jsEditor.AutoCActive)
            {
                jsEditor.AutoCShow(lenEntered, JS_KEYWORDS);
            }
        }

        /// <summary>
        /// Update the help text for the JavaScript control
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void JsEditor_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("JavaScript that can be called after the two regular expressions are run, if they are defined." + Environment.NewLine);
            helpTextBox.AppendText("This script MUST implement the function checkAndFix(checkResultItems). The CheckResultItems are the results" +
                " found in the regular expression pass.");
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
    }
}
