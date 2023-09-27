/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

namespace TvpMain.Forms
{
    partial class GroupEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GroupEditorForm));
            this.helpTextBox = new System.Windows.Forms.TextBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveIconToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contactSupportMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.licenseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.publishWorker = new System.ComponentModel.BackgroundWorker();
            this.label2 = new System.Windows.Forms.Label();
            this.scopeCombo = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.defaultDescTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupIdLabel = new System.Windows.Forms.Label();
            this.idLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.checkFixNameTextBox = new System.Windows.Forms.TextBox();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.versionTextBox = new System.Windows.Forms.MaskedTextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tagsLabel = new System.Windows.Forms.Label();
            this.languagesLabel = new System.Windows.Forms.Label();
            this.locationLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.addToGroupButton = new System.Windows.Forms.Button();
            this.removeFromGroupButton = new System.Windows.Forms.Button();
            this.moveUpButton = new System.Windows.Forms.Button();
            this.moveDownButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.availableChecks = new System.Windows.Forms.ListView();
            this.availableChecksColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupChecks = new System.Windows.Forms.ListView();
            this.groupChecksColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // helpTextBox
            // 
            this.helpTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.helpTextBox.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.helpTextBox.Location = new System.Drawing.Point(476, 431);
            this.helpTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.helpTextBox.Multiline = true;
            this.helpTextBox.Name = "helpTextBox";
            this.helpTextBox.ReadOnly = true;
            this.helpTextBox.Size = new System.Drawing.Size(711, 105);
            this.helpTextBox.TabIndex = 8;
            this.helpTextBox.TabStop = false;
            this.helpTextBox.TextChanged += new System.EventHandler(this.helpTextBox_TextChanged);
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.saveIconToolStripMenuItem,
            this.helpMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1200, 38);
            this.menuStrip.TabIndex = 13;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMenuItem,
            this.openMenuItem,
            this.saveMenuItem,
            this.exitMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F)));
            this.fileMenuItem.ShowShortcutKeys = false;
            this.fileMenuItem.Size = new System.Drawing.Size(46, 34);
            this.fileMenuItem.Text = "&File";
            // 
            // newMenuItem
            // 
            this.newMenuItem.Name = "newMenuItem";
            this.newMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newMenuItem.ShowShortcutKeys = false;
            this.newMenuItem.Size = new System.Drawing.Size(224, 26);
            this.newMenuItem.Text = "&New";
            this.newMenuItem.Click += new System.EventHandler(this.NewMenuItem_Click);
            // 
            // openMenuItem
            // 
            this.openMenuItem.Name = "openMenuItem";
            this.openMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openMenuItem.ShowShortcutKeys = false;
            this.openMenuItem.Size = new System.Drawing.Size(224, 26);
            this.openMenuItem.Text = "&Open";
            this.openMenuItem.Click += new System.EventHandler(this.OpenMenuItem_Click);
            // 
            // saveMenuItem
            // 
            this.saveMenuItem.Name = "saveMenuItem";
            this.saveMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveMenuItem.ShowShortcutKeys = false;
            this.saveMenuItem.Size = new System.Drawing.Size(224, 26);
            this.saveMenuItem.Text = "&Save";
            this.saveMenuItem.Click += new System.EventHandler(this.SaveMenuItem_Click);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.exitMenuItem.ShowShortcutKeys = false;
            this.exitMenuItem.Size = new System.Drawing.Size(224, 26);
            this.exitMenuItem.Text = "E&xit";
            this.exitMenuItem.Click += new System.EventHandler(this.ExitMenuItem_Click);
            // 
            // saveIconToolStripMenuItem
            // 
            this.saveIconToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.saveIconToolStripMenuItem.AutoToolTip = true;
            this.saveIconToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveIconToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveIconToolStripMenuItem.Image")));
            this.saveIconToolStripMenuItem.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.saveIconToolStripMenuItem.Margin = new System.Windows.Forms.Padding(5, 0, 5, 5);
            this.saveIconToolStripMenuItem.Name = "saveIconToolStripMenuItem";
            this.saveIconToolStripMenuItem.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.saveIconToolStripMenuItem.ShortcutKeyDisplayString = "Ctlr+S";
            this.saveIconToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveIconToolStripMenuItem.ShowShortcutKeys = false;
            this.saveIconToolStripMenuItem.Size = new System.Drawing.Size(24, 29);
            this.saveIconToolStripMenuItem.Text = "saveIconMenuItem";
            this.saveIconToolStripMenuItem.ToolTipText = "Ctrl+S";
            this.saveIconToolStripMenuItem.Click += new System.EventHandler(this.SaveMenuItem_Click);
            // 
            // helpMenuItem
            // 
            this.helpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contactSupportMenuItem,
            this.licenseMenuItem});
            this.helpMenuItem.Name = "helpMenuItem";
            this.helpMenuItem.Size = new System.Drawing.Size(55, 34);
            this.helpMenuItem.Text = "Help";
            // 
            // contactSupportMenuItem
            // 
            this.contactSupportMenuItem.Name = "contactSupportMenuItem";
            this.contactSupportMenuItem.Size = new System.Drawing.Size(200, 26);
            this.contactSupportMenuItem.Text = "Contact Support";
            this.contactSupportMenuItem.Click += new System.EventHandler(this.contactSupportMenuItem_Click);
            // 
            // licenseMenuItem
            // 
            this.licenseMenuItem.Name = "licenseMenuItem";
            this.licenseMenuItem.Size = new System.Drawing.Size(200, 26);
            this.licenseMenuItem.Text = "License";
            this.licenseMenuItem.Click += new System.EventHandler(this.LicenseMenuItem_Click);
            // 
            // publishWorker
            // 
            this.publishWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.PublishWorker_DoWork);
            this.publishWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.PublishWorker_RunWorkerCompleted);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 122);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Version:";
            // 
            // scopeCombo
            // 
            this.scopeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.scopeCombo.FormattingEnabled = true;
            this.scopeCombo.Items.AddRange(new object[] {
            "PROJECT",
            "BOOK",
            "CHAPTER",
            "VERSE"});
            this.scopeCombo.Location = new System.Drawing.Point(197, 151);
            this.scopeCombo.Margin = new System.Windows.Forms.Padding(4);
            this.scopeCombo.Name = "scopeCombo";
            this.scopeCombo.Size = new System.Drawing.Size(240, 24);
            this.scopeCombo.TabIndex = 6;
            this.scopeCombo.SelectedIndexChanged += new System.EventHandler(this.Content_Changed);
            this.scopeCombo.MouseEnter += new System.EventHandler(this.ScopeCombo_MouseEnter);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 155);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 16);
            this.label6.TabIndex = 7;
            this.label6.Text = "Scope:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 188);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(164, 16);
            this.label7.TabIndex = 8;
            this.label7.Text = "Default Result Description:";
            // 
            // defaultDescTextBox
            // 
            this.defaultDescTextBox.BackColor = System.Drawing.Color.LightYellow;
            this.defaultDescTextBox.Location = new System.Drawing.Point(197, 184);
            this.defaultDescTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.defaultDescTextBox.Name = "defaultDescTextBox";
            this.defaultDescTextBox.Size = new System.Drawing.Size(240, 22);
            this.defaultDescTextBox.TabIndex = 8;
            this.defaultDescTextBox.TextChanged += new System.EventHandler(this.Content_Changed);
            this.defaultDescTextBox.MouseEnter += new System.EventHandler(this.DefaultDescTextBox_MouseEnter);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 220);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(78, 16);
            this.label8.TabIndex = 11;
            this.label8.Text = "Languages:";
            // 
            // groupIdLabel
            // 
            this.groupIdLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.groupIdLabel.Location = new System.Drawing.Point(103, 55);
            this.groupIdLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.groupIdLabel.Name = "groupIdLabel";
            this.groupIdLabel.Size = new System.Drawing.Size(334, 22);
            this.groupIdLabel.TabIndex = 0;
            this.groupIdLabel.Text = "groupIdLabel";
            this.groupIdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.groupIdLabel.MouseEnter += new System.EventHandler(this.CheckFixIdLabel_MouseEnter);
            // 
            // idLabel
            // 
            this.idLabel.AutoSize = true;
            this.idLabel.Location = new System.Drawing.Point(12, 58);
            this.idLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.idLabel.Name = "idLabel";
            this.idLabel.Size = new System.Drawing.Size(23, 16);
            this.idLabel.TabIndex = 0;
            this.idLabel.Text = "ID:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 90);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Group Name:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 252);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(42, 16);
            this.label9.TabIndex = 13;
            this.label9.Text = "Tags:";
            // 
            // checkFixNameTextBox
            // 
            this.checkFixNameTextBox.BackColor = System.Drawing.Color.LightYellow;
            this.checkFixNameTextBox.Location = new System.Drawing.Point(197, 87);
            this.checkFixNameTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.checkFixNameTextBox.Name = "checkFixNameTextBox";
            this.checkFixNameTextBox.Size = new System.Drawing.Size(240, 22);
            this.checkFixNameTextBox.TabIndex = 2;
            this.checkFixNameTextBox.TextChanged += new System.EventHandler(this.Content_Changed);
            this.checkFixNameTextBox.MouseEnter += new System.EventHandler(this.CheckFixNameTextBox_MouseEnter);
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.AcceptsReturn = true;
            this.descriptionTextBox.AcceptsTab = true;
            this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.descriptionTextBox.Location = new System.Drawing.Point(12, 301);
            this.descriptionTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.descriptionTextBox.Multiline = true;
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.descriptionTextBox.Size = new System.Drawing.Size(425, 176);
            this.descriptionTextBox.TabIndex = 14;
            this.descriptionTextBox.TextChanged += new System.EventHandler(this.Content_Changed);
            this.descriptionTextBox.MouseEnter += new System.EventHandler(this.DescriptionTextBox_MouseEnter);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 280);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(75, 16);
            this.label10.TabIndex = 15;
            this.label10.Text = "Description";
            // 
            // versionTextBox
            // 
            this.versionTextBox.BackColor = System.Drawing.Color.LightYellow;
            this.versionTextBox.Location = new System.Drawing.Point(197, 119);
            this.versionTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.versionTextBox.Mask = "0.0.0.0";
            this.versionTextBox.Name = "versionTextBox";
            this.versionTextBox.Size = new System.Drawing.Size(240, 22);
            this.versionTextBox.TabIndex = 4;
            this.versionTextBox.TextChanged += new System.EventHandler(this.Content_Changed);
            this.versionTextBox.MouseEnter += new System.EventHandler(this.VersionTextBox_MouseEnter);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.tagsLabel);
            this.groupBox3.Controls.Add(this.languagesLabel);
            this.groupBox3.Controls.Add(this.locationLabel);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.versionTextBox);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.descriptionTextBox);
            this.groupBox3.Controls.Add(this.checkFixNameTextBox);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.idLabel);
            this.groupBox3.Controls.Add(this.groupIdLabel);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.defaultDescTextBox);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.scopeCombo);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(13, 47);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(450, 492);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            // 
            // tagsLabel
            // 
            this.tagsLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tagsLabel.Location = new System.Drawing.Point(197, 249);
            this.tagsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.tagsLabel.Name = "tagsLabel";
            this.tagsLabel.Size = new System.Drawing.Size(240, 22);
            this.tagsLabel.TabIndex = 19;
            this.tagsLabel.Text = "tagsLabel";
            this.tagsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tagsLabel.MouseEnter += new System.EventHandler(this.TagsLabel_MouseEnter);
            // 
            // languagesLabel
            // 
            this.languagesLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.languagesLabel.Location = new System.Drawing.Point(197, 217);
            this.languagesLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.languagesLabel.Name = "languagesLabel";
            this.languagesLabel.Size = new System.Drawing.Size(240, 22);
            this.languagesLabel.TabIndex = 18;
            this.languagesLabel.Text = "languagesLabel";
            this.languagesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.languagesLabel.MouseEnter += new System.EventHandler(this.LanguagesLabel_MouseEnter);
            // 
            // locationLabel
            // 
            this.locationLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.locationLabel.Location = new System.Drawing.Point(103, 23);
            this.locationLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.locationLabel.Name = "locationLabel";
            this.locationLabel.Size = new System.Drawing.Size(334, 22);
            this.locationLabel.TabIndex = 17;
            this.locationLabel.Text = "locationLabel";
            this.locationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 16);
            this.label3.TabIndex = 16;
            this.label3.Text = "Location:";
            // 
            // addToGroupButton
            // 
            this.addToGroupButton.Location = new System.Drawing.Point(276, 173);
            this.addToGroupButton.Name = "addToGroupButton";
            this.addToGroupButton.Size = new System.Drawing.Size(75, 23);
            this.addToGroupButton.TabIndex = 18;
            this.addToGroupButton.Text = "Add";
            this.addToGroupButton.UseVisualStyleBackColor = true;
            this.addToGroupButton.Click += new System.EventHandler(this.addToGroupButton_Click);
            // 
            // removeFromGroupButton
            // 
            this.removeFromGroupButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.removeFromGroupButton.Location = new System.Drawing.Point(623, 212);
            this.removeFromGroupButton.Name = "removeFromGroupButton";
            this.removeFromGroupButton.Size = new System.Drawing.Size(75, 23);
            this.removeFromGroupButton.TabIndex = 26;
            this.removeFromGroupButton.Text = "Remove";
            this.removeFromGroupButton.UseVisualStyleBackColor = true;
            this.removeFromGroupButton.Click += new System.EventHandler(this.removeFromGroupButton_Click);
            // 
            // moveUpButton
            // 
            this.moveUpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.moveUpButton.Location = new System.Drawing.Point(623, 135);
            this.moveUpButton.Name = "moveUpButton";
            this.moveUpButton.Size = new System.Drawing.Size(75, 23);
            this.moveUpButton.TabIndex = 22;
            this.moveUpButton.Text = "Up";
            this.moveUpButton.UseVisualStyleBackColor = true;
            this.moveUpButton.Click += new System.EventHandler(this.moveUpButton_Click);
            // 
            // moveDownButton
            // 
            this.moveDownButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.moveDownButton.Location = new System.Drawing.Point(623, 174);
            this.moveDownButton.Name = "moveDownButton";
            this.moveDownButton.Size = new System.Drawing.Size(75, 23);
            this.moveDownButton.TabIndex = 24;
            this.moveDownButton.Text = "Down";
            this.moveDownButton.UseVisualStyleBackColor = true;
            this.moveDownButton.Click += new System.EventHandler(this.moveDownButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.availableChecks);
            this.groupBox1.Controls.Add(this.groupChecks);
            this.groupBox1.Controls.Add(this.moveDownButton);
            this.groupBox1.Controls.Add(this.moveUpButton);
            this.groupBox1.Controls.Add(this.removeFromGroupButton);
            this.groupBox1.Controls.Add(this.addToGroupButton);
            this.groupBox1.Location = new System.Drawing.Point(476, 47);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(712, 373);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Checks";
            // 
            // availableChecks
            // 
            this.availableChecks.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.availableChecks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.availableChecks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.availableChecksColumnHeader});
            this.availableChecks.FullRowSelect = true;
            this.availableChecks.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.availableChecks.HideSelection = false;
            this.availableChecks.Location = new System.Drawing.Point(14, 22);
            this.availableChecks.MultiSelect = false;
            this.availableChecks.Name = "availableChecks";
            this.availableChecks.ShowItemToolTips = true;
            this.availableChecks.Size = new System.Drawing.Size(250, 336);
            this.availableChecks.TabIndex = 28;
            this.availableChecks.UseCompatibleStateImageBehavior = false;
            this.availableChecks.View = System.Windows.Forms.View.Details;
            this.availableChecks.DoubleClick += new System.EventHandler(this.availableChecks_DoubleClick);
            this.availableChecks.MouseEnter += new System.EventHandler(this.availableChecks_MouseEnter);
            this.availableChecks.Resize += new System.EventHandler(this.availableChecks_Resize);
            // 
            // availableChecksColumnHeader
            // 
            this.availableChecksColumnHeader.Text = "";
            this.availableChecksColumnHeader.Width = 184;
            // 
            // groupChecks
            // 
            this.groupChecks.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.groupChecks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupChecks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.groupChecksColumnHeader});
            this.groupChecks.FullRowSelect = true;
            this.groupChecks.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.groupChecks.HideSelection = false;
            this.groupChecks.Location = new System.Drawing.Point(365, 22);
            this.groupChecks.MultiSelect = false;
            this.groupChecks.Name = "groupChecks";
            this.groupChecks.ShowItemToolTips = true;
            this.groupChecks.Size = new System.Drawing.Size(245, 336);
            this.groupChecks.TabIndex = 27;
            this.groupChecks.UseCompatibleStateImageBehavior = false;
            this.groupChecks.View = System.Windows.Forms.View.Details;
            this.groupChecks.DoubleClick += new System.EventHandler(this.groupChecks_DoubleClick);
            this.groupChecks.MouseEnter += new System.EventHandler(this.groupChecks_MouseEnter);
            this.groupChecks.Resize += new System.EventHandler(this.groupChecks_Resize);
            // 
            // groupChecksColumnHeader
            // 
            this.groupChecksColumnHeader.Text = "";
            // 
            // GroupEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 550);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.helpTextBox);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GroupEditorForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Group Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.Load += new System.EventHandler(this.CheckEditor_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox helpTextBox;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.ComponentModel.BackgroundWorker publishWorker;
        private System.Windows.Forms.ToolStripMenuItem saveIconToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contactSupportMenuItem;
        private System.Windows.Forms.ToolStripMenuItem licenseMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox scopeCombo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox defaultDescTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label groupIdLabel;
        private System.Windows.Forms.Label idLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox checkFixNameTextBox;
        private System.Windows.Forms.TextBox descriptionTextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.MaskedTextBox versionTextBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button addToGroupButton;
        private System.Windows.Forms.Button removeFromGroupButton;
        private System.Windows.Forms.Button moveUpButton;
        private System.Windows.Forms.Button moveDownButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label locationLabel;
        private System.Windows.Forms.ListView groupChecks;
        private System.Windows.Forms.ListView availableChecks;
        private System.Windows.Forms.ColumnHeader availableChecksColumnHeader;
        private System.Windows.Forms.ColumnHeader groupChecksColumnHeader;
        private System.Windows.Forms.Label languagesLabel;
        private System.Windows.Forms.Label tagsLabel;
    }
}
