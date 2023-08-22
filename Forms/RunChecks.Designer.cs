/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using TvpMain.Util;

namespace TvpMain.Forms
{
    partial class RunChecks
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RunChecks));
            this.runChecksMenu = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newCheckMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contactSupportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.licenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectLabel = new System.Windows.Forms.Label();
            this.checksGroupBox = new System.Windows.Forms.GroupBox();
            this.tryToConnectButton = new System.Windows.Forms.Button();
            this.filterLabel = new System.Windows.Forms.Label();
            this.filterTextBox = new System.Windows.Forms.TextBox();
            this.refreshButton = new System.Windows.Forms.Button();
            this.setDefaultsToSelected = new System.Windows.Forms.Button();
            this.resetToProjectDefaultsButton = new System.Windows.Forms.Button();
            this.checksList = new System.Windows.Forms.DataGridView();
            this.checksListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextGroupBox = new System.Windows.Forms.GroupBox();
            this.toChapterLabel = new System.Windows.Forms.Label();
            this.chapterLabel = new System.Windows.Forms.Label();
            this.chooseBooksButton = new System.Windows.Forms.Button();
            this.toChapterDropDown = new System.Windows.Forms.ComboBox();
            this.fromChapterDropDown = new System.Windows.Forms.ComboBox();
            this.currentBookText = new System.Windows.Forms.TextBox();
            this.chooseBooksText = new System.Windows.Forms.TextBox();
            this.chooseBooksRadioButton = new System.Windows.Forms.RadioButton();
            this.currentBookRadioButton = new System.Windows.Forms.RadioButton();
            this.Copyright = new System.Windows.Forms.Label();
            this.runChecksButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.helpTextBox = new System.Windows.Forms.TextBox();
            this.projectNameText = new System.Windows.Forms.Label();
            this.runChecksTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.CFLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CFType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CFName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CFVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CFLanguages = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CFTags = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CFId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.runChecksMenu.SuspendLayout();
            this.checksGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checksList)).BeginInit();
            this.checksListContextMenu.SuspendLayout();
            this.contextGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // runChecksMenu
            // 
            this.runChecksMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.runChecksMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.runChecksMenu.Location = new System.Drawing.Point(13, 12);
            this.runChecksMenu.Name = "runChecksMenu";
            this.runChecksMenu.Size = new System.Drawing.Size(1190, 28);
            this.runChecksMenu.TabIndex = 0;
            this.runChecksMenu.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMenuItem,
            this.exitMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileMenuItem.Text = "File";
            // 
            // newMenuItem
            // 
            this.newMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newCheckMenuItem});
            this.newMenuItem.Name = "newMenuItem";
            this.newMenuItem.Size = new System.Drawing.Size(122, 26);
            this.newMenuItem.Text = "New";
            // 
            // newCheckMenuItem
            // 
            this.newCheckMenuItem.Name = "newCheckMenuItem";
            this.newCheckMenuItem.Size = new System.Drawing.Size(140, 26);
            this.newCheckMenuItem.Text = "Check...";
            this.newCheckMenuItem.Click += new System.EventHandler(this.newCheckMenuItem_Click);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(122, 26);
            this.exitMenuItem.Text = "E&xit";
            this.exitMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(58, 24);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(153, 26);
            this.optionsToolStripMenuItem.Text = "Options...";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contactSupportToolStripMenuItem,
            this.licenseToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // contactSupportToolStripMenuItem
            // 
            this.contactSupportToolStripMenuItem.Name = "contactSupportToolStripMenuItem";
            this.contactSupportToolStripMenuItem.Size = new System.Drawing.Size(200, 26);
            this.contactSupportToolStripMenuItem.Text = "Contact Support";
            this.contactSupportToolStripMenuItem.Click += new System.EventHandler(this.contactSupportToolStripMenuItem_Click);
            // 
            // licenseToolStripMenuItem
            // 
            this.licenseToolStripMenuItem.Name = "licenseToolStripMenuItem";
            this.licenseToolStripMenuItem.Size = new System.Drawing.Size(200, 26);
            this.licenseToolStripMenuItem.Text = "License";
            this.licenseToolStripMenuItem.Click += new System.EventHandler(this.LicenseToolStripMenuItem_Click);
            // 
            // projectLabel
            // 
            this.projectLabel.AutoSize = true;
            this.projectLabel.Location = new System.Drawing.Point(19, 54);
            this.projectLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.projectLabel.Name = "projectLabel";
            this.projectLabel.Size = new System.Drawing.Size(49, 16);
            this.projectLabel.TabIndex = 2;
            this.projectLabel.Text = "Project";
            // 
            // checksGroupBox
            // 
            this.checksGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checksGroupBox.Controls.Add(this.tryToConnectButton);
            this.checksGroupBox.Controls.Add(this.filterLabel);
            this.checksGroupBox.Controls.Add(this.filterTextBox);
            this.checksGroupBox.Controls.Add(this.refreshButton);
            this.checksGroupBox.Controls.Add(this.setDefaultsToSelected);
            this.checksGroupBox.Controls.Add(this.resetToProjectDefaultsButton);
            this.checksGroupBox.Controls.Add(this.checksList);
            this.checksGroupBox.Location = new System.Drawing.Point(17, 79);
            this.checksGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.checksGroupBox.Name = "checksGroupBox";
            this.checksGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.checksGroupBox.Size = new System.Drawing.Size(1181, 526);
            this.checksGroupBox.TabIndex = 3;
            this.checksGroupBox.TabStop = false;
            this.checksGroupBox.Text = "Checks";
            // 
            // tryToConnectButton
            // 
            this.tryToConnectButton.Location = new System.Drawing.Point(147, 22);
            this.tryToConnectButton.Margin = new System.Windows.Forms.Padding(4);
            this.tryToConnectButton.Name = "tryToConnectButton";
            this.tryToConnectButton.Size = new System.Drawing.Size(131, 28);
            this.tryToConnectButton.TabIndex = 6;
            this.tryToConnectButton.Text = "Try to Connect";
            this.tryToConnectButton.UseVisualStyleBackColor = true;
            this.tryToConnectButton.Visible = false;
            this.tryToConnectButton.Click += new System.EventHandler(this.tryToReconnectButton_Click);
            // 
            // filterLabel
            // 
            this.filterLabel.AutoSize = true;
            this.filterLabel.Location = new System.Drawing.Point(528, 28);
            this.filterLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.filterLabel.Name = "filterLabel";
            this.filterLabel.Size = new System.Drawing.Size(180, 16);
            this.filterLabel.TabIndex = 5;
            this.filterLabel.Text = "Search (at least 3 characters)";
            // 
            // filterTextBox
            // 
            this.filterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterTextBox.Location = new System.Drawing.Point(731, 25);
            this.filterTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.Size = new System.Drawing.Size(441, 22);
            this.filterTextBox.TabIndex = 4;
            this.filterTextBox.TextChanged += new System.EventHandler(this.FilterTextBox_TextChanged);
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(8, 22);
            this.refreshButton.Margin = new System.Windows.Forms.Padding(4);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(131, 28);
            this.refreshButton.TabIndex = 3;
            this.refreshButton.Text = "Fetch Updates";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Visible = false;
            this.refreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // setDefaultsToSelected
            // 
            this.setDefaultsToSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.setDefaultsToSelected.Location = new System.Drawing.Point(8, 490);
            this.setDefaultsToSelected.Margin = new System.Windows.Forms.Padding(4);
            this.setDefaultsToSelected.Name = "setDefaultsToSelected";
            this.setDefaultsToSelected.Size = new System.Drawing.Size(343, 28);
            this.setDefaultsToSelected.TabIndex = 2;
            this.setDefaultsToSelected.Text = "Set Selected Checks as the Project Defaults";
            this.setDefaultsToSelected.UseVisualStyleBackColor = true;
            this.setDefaultsToSelected.Click += new System.EventHandler(this.SetDefaultsToSelected_Click);
            this.setDefaultsToSelected.MouseEnter += new System.EventHandler(this.SetDefaultsToSelected_MouseEnter);
            // 
            // resetToProjectDefaultsButton
            // 
            this.resetToProjectDefaultsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resetToProjectDefaultsButton.Location = new System.Drawing.Point(984, 490);
            this.resetToProjectDefaultsButton.Margin = new System.Windows.Forms.Padding(4);
            this.resetToProjectDefaultsButton.Name = "resetToProjectDefaultsButton";
            this.resetToProjectDefaultsButton.Size = new System.Drawing.Size(189, 28);
            this.resetToProjectDefaultsButton.TabIndex = 1;
            this.resetToProjectDefaultsButton.Text = "Reset to Project Defaults";
            this.resetToProjectDefaultsButton.UseVisualStyleBackColor = true;
            this.resetToProjectDefaultsButton.Click += new System.EventHandler(this.ResetToProjectDefaults_Click);
            this.resetToProjectDefaultsButton.MouseEnter += new System.EventHandler(this.ResetToProjectDefaults_MouseEnter);
            // 
            // checksList
            // 
            this.checksList.AllowUserToAddRows = false;
            this.checksList.AllowUserToResizeRows = false;
            this.checksList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checksList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.checksList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.checksList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.checksList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CFLocation,
            this.CFType,
            this.CFName,
            this.CFVersion,
            this.CFLanguages,
            this.CFTags,
            this.CFId});
            this.checksList.Location = new System.Drawing.Point(8, 57);
            this.checksList.Margin = new System.Windows.Forms.Padding(4);
            this.checksList.Name = "checksList";
            this.checksList.ReadOnly = true;
            this.checksList.RowHeadersVisible = false;
            this.checksList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.checksList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.checksList.Size = new System.Drawing.Size(1165, 426);
            this.checksList.TabIndex = 0;
            this.checksList.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ChecksList_EditCheck);
            this.checksList.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.checksList_CellMouseDown);
            this.checksList.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.ChecksList_CellMouseEnter);
            this.checksList.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.checksList_CellMouseUp);
            this.checksList.RowStateChanged += new System.Windows.Forms.DataGridViewRowStateChangedEventHandler(this.checksList_RowStateChanged);
            this.checksList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.checksList_KeyDown);
            // 
            // checksListContextMenu
            // 
            this.checksListContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.checksListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editContextMenuItem,
            this.deleteContextMenuItem});
            this.checksListContextMenu.Name = "checksListContextMenuStrip";
            this.checksListContextMenu.Size = new System.Drawing.Size(123, 52);
            this.checksListContextMenu.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.checksListContextMenu_Closed);
            // 
            // editContextMenuItem
            // 
            this.editContextMenuItem.Name = "editContextMenuItem";
            this.editContextMenuItem.Size = new System.Drawing.Size(122, 24);
            this.editContextMenuItem.Text = "Edit...";
            this.editContextMenuItem.Click += new System.EventHandler(this.editContextMenuItem_Click);
            // 
            // deleteContextMenuItem
            // 
            this.deleteContextMenuItem.Name = "deleteContextMenuItem";
            this.deleteContextMenuItem.Size = new System.Drawing.Size(122, 24);
            this.deleteContextMenuItem.Text = "Delete";
            this.deleteContextMenuItem.Click += new System.EventHandler(this.deleteContextMenuItem_Click);
            // 
            // contextGroupBox
            // 
            this.contextGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.contextGroupBox.Controls.Add(this.toChapterLabel);
            this.contextGroupBox.Controls.Add(this.chapterLabel);
            this.contextGroupBox.Controls.Add(this.chooseBooksButton);
            this.contextGroupBox.Controls.Add(this.toChapterDropDown);
            this.contextGroupBox.Controls.Add(this.fromChapterDropDown);
            this.contextGroupBox.Controls.Add(this.currentBookText);
            this.contextGroupBox.Controls.Add(this.chooseBooksText);
            this.contextGroupBox.Controls.Add(this.chooseBooksRadioButton);
            this.contextGroupBox.Controls.Add(this.currentBookRadioButton);
            this.contextGroupBox.Location = new System.Drawing.Point(17, 612);
            this.contextGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.contextGroupBox.Name = "contextGroupBox";
            this.contextGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.contextGroupBox.Size = new System.Drawing.Size(1181, 101);
            this.contextGroupBox.TabIndex = 4;
            this.contextGroupBox.TabStop = false;
            // 
            // toChapterLabel
            // 
            this.toChapterLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.toChapterLabel.AutoSize = true;
            this.toChapterLabel.Location = new System.Drawing.Point(1044, 26);
            this.toChapterLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.toChapterLabel.Name = "toChapterLabel";
            this.toChapterLabel.Size = new System.Drawing.Size(18, 16);
            this.toChapterLabel.TabIndex = 8;
            this.toChapterLabel.Text = "to";
            // 
            // chapterLabel
            // 
            this.chapterLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chapterLabel.AutoSize = true;
            this.chapterLabel.Location = new System.Drawing.Point(863, 26);
            this.chapterLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.chapterLabel.Name = "chapterLabel";
            this.chapterLabel.Size = new System.Drawing.Size(61, 16);
            this.chapterLabel.TabIndex = 7;
            this.chapterLabel.Text = "Chapters";
            // 
            // chooseBooksButton
            // 
            this.chooseBooksButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chooseBooksButton.Location = new System.Drawing.Point(1073, 57);
            this.chooseBooksButton.Margin = new System.Windows.Forms.Padding(4);
            this.chooseBooksButton.Name = "chooseBooksButton";
            this.chooseBooksButton.Size = new System.Drawing.Size(100, 28);
            this.chooseBooksButton.TabIndex = 6;
            this.chooseBooksButton.Text = "Choose...";
            this.chooseBooksButton.UseVisualStyleBackColor = true;
            this.chooseBooksButton.Click += new System.EventHandler(this.ChooseBooksButton_Click);
            this.chooseBooksButton.MouseEnter += new System.EventHandler(this.ChooseBooksRadioButton_MouseEnter);
            // 
            // toChapterDropDown
            // 
            this.toChapterDropDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.toChapterDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toChapterDropDown.FormattingEnabled = true;
            this.toChapterDropDown.Location = new System.Drawing.Point(1073, 23);
            this.toChapterDropDown.Margin = new System.Windows.Forms.Padding(4);
            this.toChapterDropDown.Name = "toChapterDropDown";
            this.toChapterDropDown.Size = new System.Drawing.Size(99, 24);
            this.toChapterDropDown.TabIndex = 5;
            this.toChapterDropDown.MouseEnter += new System.EventHandler(this.ToChapterDropDown_MouseEnter);
            // 
            // fromChapterDropDown
            // 
            this.fromChapterDropDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.fromChapterDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fromChapterDropDown.FormattingEnabled = true;
            this.fromChapterDropDown.Location = new System.Drawing.Point(936, 22);
            this.fromChapterDropDown.Margin = new System.Windows.Forms.Padding(4);
            this.fromChapterDropDown.Name = "fromChapterDropDown";
            this.fromChapterDropDown.Size = new System.Drawing.Size(99, 24);
            this.fromChapterDropDown.TabIndex = 4;
            this.fromChapterDropDown.MouseEnter += new System.EventHandler(this.FromChapterDropDown_MouseEnter);
            // 
            // currentBookText
            // 
            this.currentBookText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.currentBookText.Location = new System.Drawing.Point(133, 23);
            this.currentBookText.Margin = new System.Windows.Forms.Padding(4);
            this.currentBookText.Name = "currentBookText";
            this.currentBookText.ReadOnly = true;
            this.currentBookText.Size = new System.Drawing.Size(720, 22);
            this.currentBookText.TabIndex = 3;
            this.currentBookText.MouseEnter += new System.EventHandler(this.CurrentBookRadioButton_MouseEnter);
            // 
            // chooseBooksText
            // 
            this.chooseBooksText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chooseBooksText.Location = new System.Drawing.Point(133, 59);
            this.chooseBooksText.Margin = new System.Windows.Forms.Padding(4);
            this.chooseBooksText.Name = "chooseBooksText";
            this.chooseBooksText.ReadOnly = true;
            this.chooseBooksText.Size = new System.Drawing.Size(931, 22);
            this.chooseBooksText.TabIndex = 2;
            this.chooseBooksText.Text = "*none*";
            this.chooseBooksText.MouseEnter += new System.EventHandler(this.ChooseBooksText_MouseEnter);
            // 
            // chooseBooksRadioButton
            // 
            this.chooseBooksRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chooseBooksRadioButton.AutoSize = true;
            this.chooseBooksRadioButton.Location = new System.Drawing.Point(8, 62);
            this.chooseBooksRadioButton.Margin = new System.Windows.Forms.Padding(4);
            this.chooseBooksRadioButton.Name = "chooseBooksRadioButton";
            this.chooseBooksRadioButton.Size = new System.Drawing.Size(117, 20);
            this.chooseBooksRadioButton.TabIndex = 1;
            this.chooseBooksRadioButton.TabStop = true;
            this.chooseBooksRadioButton.Text = "Choose Books";
            this.chooseBooksRadioButton.UseVisualStyleBackColor = true;
            this.chooseBooksRadioButton.Click += new System.EventHandler(this.ChooseBooksRadioButton_Click);
            this.chooseBooksRadioButton.MouseEnter += new System.EventHandler(this.ChooseBooksRadioButton_MouseEnter);
            // 
            // currentBookRadioButton
            // 
            this.currentBookRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.currentBookRadioButton.AutoSize = true;
            this.currentBookRadioButton.Location = new System.Drawing.Point(9, 25);
            this.currentBookRadioButton.Margin = new System.Windows.Forms.Padding(4);
            this.currentBookRadioButton.Name = "currentBookRadioButton";
            this.currentBookRadioButton.Size = new System.Drawing.Size(105, 20);
            this.currentBookRadioButton.TabIndex = 0;
            this.currentBookRadioButton.TabStop = true;
            this.currentBookRadioButton.Text = "Current Book";
            this.currentBookRadioButton.UseVisualStyleBackColor = true;
            this.currentBookRadioButton.Click += new System.EventHandler(this.CurrentBookRadioButton_Click);
            this.currentBookRadioButton.MouseEnter += new System.EventHandler(this.CurrentBookRadioButton_MouseEnter);
            // 
            // Copyright
            // 
            this.Copyright.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Copyright.AutoSize = true;
            this.Copyright.Location = new System.Drawing.Point(23, 862);
            this.Copyright.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Copyright.Name = "Copyright";
            this.Copyright.Size = new System.Drawing.Size(148, 16);
            this.Copyright.TabIndex = 10;
            this.Copyright.Text = "© 2020-2022 Biblica, Inc.";
            // 
            // runChecksButton
            // 
            this.runChecksButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.runChecksButton.Location = new System.Drawing.Point(1091, 855);
            this.runChecksButton.Margin = new System.Windows.Forms.Padding(4);
            this.runChecksButton.Name = "runChecksButton";
            this.runChecksButton.Size = new System.Drawing.Size(100, 28);
            this.runChecksButton.TabIndex = 11;
            this.runChecksButton.Text = "Run Checks";
            this.runChecksButton.UseVisualStyleBackColor = true;
            this.runChecksButton.Click += new System.EventHandler(this.RunChecksButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(983, 855);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 28);
            this.cancelButton.TabIndex = 13;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // helpTextBox
            // 
            this.helpTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.helpTextBox.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.helpTextBox.Location = new System.Drawing.Point(17, 720);
            this.helpTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.helpTextBox.Multiline = true;
            this.helpTextBox.Name = "helpTextBox";
            this.helpTextBox.ReadOnly = true;
            this.helpTextBox.Size = new System.Drawing.Size(1180, 127);
            this.helpTextBox.TabIndex = 14;
            // 
            // projectNameText
            // 
            this.projectNameText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.projectNameText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.projectNameText.Location = new System.Drawing.Point(80, 49);
            this.projectNameText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.projectNameText.Name = "projectNameText";
            this.projectNameText.Size = new System.Drawing.Size(1118, 25);
            this.projectNameText.TabIndex = 15;
            this.projectNameText.Text = "Project Name";
            this.projectNameText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // runChecksTooltip
            // 
            this.runChecksTooltip.ShowAlways = true;
            // 
            // CFLocation
            // 
            this.CFLocation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.CFLocation.DefaultCellStyle = dataGridViewCellStyle2;
            this.CFLocation.HeaderText = "Location";
            this.CFLocation.MinimumWidth = 6;
            this.CFLocation.Name = "CFLocation";
            this.CFLocation.ReadOnly = true;
            this.CFLocation.Width = 99;
            // 
            // CFType
            // 
            this.CFType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.CFType.DefaultCellStyle = dataGridViewCellStyle3;
            this.CFType.HeaderText = "Type";
            this.CFType.MinimumWidth = 6;
            this.CFType.Name = "CFType";
            this.CFType.ReadOnly = true;
            this.CFType.Width = 73;
            // 
            // CFName
            // 
            this.CFName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.CFName.HeaderText = "Name";
            this.CFName.MinimumWidth = 6;
            this.CFName.Name = "CFName";
            this.CFName.ReadOnly = true;
            // 
            // CFVersion
            // 
            this.CFVersion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.CFVersion.DefaultCellStyle = dataGridViewCellStyle4;
            this.CFVersion.FillWeight = 80F;
            this.CFVersion.HeaderText = "Version";
            this.CFVersion.MinimumWidth = 6;
            this.CFVersion.Name = "CFVersion";
            this.CFVersion.ReadOnly = true;
            this.CFVersion.Width = 92;
            // 
            // CFLanguages
            // 
            this.CFLanguages.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.CFLanguages.FillWeight = 80F;
            this.CFLanguages.HeaderText = "Languages";
            this.CFLanguages.MinimumWidth = 70;
            this.CFLanguages.Name = "CFLanguages";
            this.CFLanguages.ReadOnly = true;
            this.CFLanguages.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.CFLanguages.Width = 117;
            // 
            // CFTags
            // 
            this.CFTags.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.CFTags.FillWeight = 80F;
            this.CFTags.HeaderText = "Tags";
            this.CFTags.MinimumWidth = 70;
            this.CFTags.Name = "CFTags";
            this.CFTags.ReadOnly = true;
            this.CFTags.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // CFId
            // 
            this.CFId.HeaderText = "Id";
            this.CFId.MinimumWidth = 6;
            this.CFId.Name = "CFId";
            this.CFId.ReadOnly = true;
            this.CFId.Visible = false;
            // 
            // RunChecks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(1216, 901);
            this.Controls.Add(this.projectNameText);
            this.Controls.Add(this.helpTextBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.runChecksButton);
            this.Controls.Add(this.Copyright);
            this.Controls.Add(this.contextGroupBox);
            this.Controls.Add(this.checksGroupBox);
            this.Controls.Add(this.projectLabel);
            this.Controls.Add(this.runChecksMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.runChecksMenu;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RunChecks";
            this.Padding = new System.Windows.Forms.Padding(13, 12, 13, 31);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Run Checks";
            this.Load += new System.EventHandler(this.RunChecks_Load);
            this.runChecksMenu.ResumeLayout(false);
            this.runChecksMenu.PerformLayout();
            this.checksGroupBox.ResumeLayout(false);
            this.checksGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checksList)).EndInit();
            this.checksListContextMenu.ResumeLayout(false);
            this.contextGroupBox.ResumeLayout(false);
            this.contextGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button tryToConnectButton;

        private System.Windows.Forms.ToolTip runChecksTooltip;

        #endregion

        private System.Windows.Forms.MenuStrip runChecksMenu;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.Label projectLabel;
        private System.Windows.Forms.GroupBox checksGroupBox;
        private System.Windows.Forms.DataGridView checksList;
        private System.Windows.Forms.GroupBox contextGroupBox;
        private System.Windows.Forms.Label chapterLabel;
        private System.Windows.Forms.Button chooseBooksButton;
        private System.Windows.Forms.ComboBox toChapterDropDown;
        private System.Windows.Forms.ComboBox fromChapterDropDown;
        private System.Windows.Forms.TextBox currentBookText;
        private System.Windows.Forms.TextBox chooseBooksText;
        private System.Windows.Forms.RadioButton chooseBooksRadioButton;
        private System.Windows.Forms.RadioButton currentBookRadioButton;
        private System.Windows.Forms.Button resetToProjectDefaultsButton;
        private System.Windows.Forms.Button setDefaultsToSelected;
        private System.Windows.Forms.Label Copyright;
        private System.Windows.Forms.Label toChapterLabel;
        private System.Windows.Forms.Button runChecksButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox helpTextBox;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.Label projectNameText;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Label filterLabel;
        private System.Windows.Forms.TextBox filterTextBox;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contactSupportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem licenseToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip checksListContextMenu;
        private System.Windows.Forms.ToolStripMenuItem deleteContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newCheckMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editContextMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn CFLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn CFType;
        private System.Windows.Forms.DataGridViewTextBoxColumn CFName;
        private System.Windows.Forms.DataGridViewTextBoxColumn CFVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn CFLanguages;
        private System.Windows.Forms.DataGridViewTextBoxColumn CFTags;
        private System.Windows.Forms.DataGridViewTextBoxColumn CFId;
    }
}