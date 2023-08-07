/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

namespace TvpMain.Forms
{
    partial class CheckEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckEditor));
            this.idLabel = new System.Windows.Forms.Label();
            this.checkFixIdLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.checkFixNameTextBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.jsEditor = new ScintillaNET.Scintilla();
            this.label5 = new System.Windows.Forms.Label();
            this.fixRegExTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkFindRegExTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.scopeCombo = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.versionTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tagsTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.languagesTextBox = new System.Windows.Forms.TextBox();
            this.defaultDescTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.helpTextBox = new System.Windows.Forms.TextBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.publishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveIconToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contactSupportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.licenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.publishWorker = new System.ComponentModel.BackgroundWorker();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // idLabel
            // 
            this.idLabel.AutoSize = true;
            this.idLabel.Location = new System.Drawing.Point(8, 25);
            this.idLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.idLabel.Name = "idLabel";
            this.idLabel.Size = new System.Drawing.Size(23, 16);
            this.idLabel.TabIndex = 0;
            this.idLabel.Text = "ID:";
            // 
            // checkFixIdLabel
            // 
            this.checkFixIdLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.checkFixIdLabel.Location = new System.Drawing.Point(44, 19);
            this.checkFixIdLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.checkFixIdLabel.Name = "checkFixIdLabel";
            this.checkFixIdLabel.Size = new System.Drawing.Size(389, 22);
            this.checkFixIdLabel.TabIndex = 1;
            this.checkFixIdLabel.Text = "label1";
            this.checkFixIdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.checkFixIdLabel.MouseEnter += new System.EventHandler(this.CheckFixIdLabel_MouseEnter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 53);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Check/Fix Name:";
            // 
            // checkFixNameTextBox
            // 
            this.checkFixNameTextBox.BackColor = System.Drawing.Color.LightYellow;
            this.checkFixNameTextBox.Location = new System.Drawing.Point(193, 49);
            this.checkFixNameTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.checkFixNameTextBox.Name = "checkFixNameTextBox";
            this.checkFixNameTextBox.Size = new System.Drawing.Size(240, 22);
            this.checkFixNameTextBox.TabIndex = 3;
            this.checkFixNameTextBox.TextChanged += new System.EventHandler(this.Content_TextChanged);
            this.checkFixNameTextBox.MouseEnter += new System.EventHandler(this.CheckFixNameTextBox_MouseEnter);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.jsEditor);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.fixRegExTextBox);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.checkFindRegExTextBox);
            this.groupBox2.Location = new System.Drawing.Point(17, 46);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(502, 507);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            // 
            // jsEditor
            // 
            this.jsEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.jsEditor.Lexer = ScintillaNET.Lexer.Cpp;
            this.jsEditor.Location = new System.Drawing.Point(8, 113);
            this.jsEditor.Margin = new System.Windows.Forms.Padding(4);
            this.jsEditor.Name = "jsEditor";
            this.jsEditor.ScrollWidth = 1000;
            this.jsEditor.Size = new System.Drawing.Size(486, 386);
            this.jsEditor.TabIndex = 6;
            this.jsEditor.CharAdded += new System.EventHandler<ScintillaNET.CharAddedEventArgs>(this.JsEditor_CharAdded);
            this.jsEditor.TextChanged += new System.EventHandler(this.JsEditor_TextChanged);
            this.jsEditor.MouseEnter += new System.EventHandler(this.JsEditor_MouseEnter);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 90);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 16);
            this.label5.TabIndex = 5;
            this.label5.Text = "JavaScript";
            // 
            // fixRegExTextBox
            // 
            this.fixRegExTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fixRegExTextBox.Location = new System.Drawing.Point(155, 53);
            this.fixRegExTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.fixRegExTextBox.Name = "fixRegExTextBox";
            this.fixRegExTextBox.Size = new System.Drawing.Size(339, 22);
            this.fixRegExTextBox.TabIndex = 3;
            this.fixRegExTextBox.TextChanged += new System.EventHandler(this.Content_TextChanged);
            this.fixRegExTextBox.MouseEnter += new System.EventHandler(this.FixRegExTextBox_MouseEnter);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 56);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(126, 16);
            this.label4.TabIndex = 2;
            this.label4.Text = "Fix Replace RegEx:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 25);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 16);
            this.label3.TabIndex = 1;
            this.label3.Text = "Check Find RegEx:";
            // 
            // checkFindRegExTextBox
            // 
            this.checkFindRegExTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkFindRegExTextBox.Location = new System.Drawing.Point(155, 20);
            this.checkFindRegExTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.checkFindRegExTextBox.Name = "checkFindRegExTextBox";
            this.checkFindRegExTextBox.Size = new System.Drawing.Size(339, 22);
            this.checkFindRegExTextBox.TabIndex = 0;
            this.checkFindRegExTextBox.TextChanged += new System.EventHandler(this.Content_TextChanged);
            this.checkFindRegExTextBox.MouseEnter += new System.EventHandler(this.CheckFindRegExTextBox_MouseEnter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 85);
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
            this.scopeCombo.Location = new System.Drawing.Point(193, 113);
            this.scopeCombo.Margin = new System.Windows.Forms.Padding(4);
            this.scopeCombo.Name = "scopeCombo";
            this.scopeCombo.Size = new System.Drawing.Size(240, 24);
            this.scopeCombo.TabIndex = 9;
            this.scopeCombo.MouseEnter += new System.EventHandler(this.ScopeCombo_MouseEnter);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.versionTextBox);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.descriptionTextBox);
            this.groupBox3.Controls.Add(this.checkFixNameTextBox);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.tagsTextBox);
            this.groupBox3.Controls.Add(this.idLabel);
            this.groupBox3.Controls.Add(this.checkFixIdLabel);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.languagesTextBox);
            this.groupBox3.Controls.Add(this.defaultDescTextBox);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.scopeCombo);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(527, 46);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(441, 507);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            // 
            // versionTextBox
            // 
            this.versionTextBox.BackColor = System.Drawing.Color.LightYellow;
            this.versionTextBox.Location = new System.Drawing.Point(193, 81);
            this.versionTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.versionTextBox.Mask = "0.0.0.0";
            this.versionTextBox.Name = "versionTextBox";
            this.versionTextBox.Size = new System.Drawing.Size(240, 22);
            this.versionTextBox.TabIndex = 6;
            this.versionTextBox.TextChanged += new System.EventHandler(this.Content_TextChanged);
            this.versionTextBox.MouseEnter += new System.EventHandler(this.VersionTextBox_MouseEnter);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 254);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(75, 16);
            this.label10.TabIndex = 15;
            this.label10.Text = "Description";
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.AcceptsReturn = true;
            this.descriptionTextBox.AcceptsTab = true;
            this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionTextBox.Location = new System.Drawing.Point(8, 276);
            this.descriptionTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.descriptionTextBox.Multiline = true;
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.descriptionTextBox.Size = new System.Drawing.Size(425, 223);
            this.descriptionTextBox.TabIndex = 21;
            this.descriptionTextBox.TextChanged += new System.EventHandler(this.Content_TextChanged);
            this.descriptionTextBox.MouseEnter += new System.EventHandler(this.DescriptionTextBox_MouseEnter);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 214);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(42, 16);
            this.label9.TabIndex = 13;
            this.label9.Text = "Tags:";
            // 
            // tagsTextBox
            // 
            this.tagsTextBox.Location = new System.Drawing.Point(193, 210);
            this.tagsTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.tagsTextBox.Name = "tagsTextBox";
            this.tagsTextBox.Size = new System.Drawing.Size(240, 22);
            this.tagsTextBox.TabIndex = 18;
            this.tagsTextBox.TextChanged += new System.EventHandler(this.Content_TextChanged);
            this.tagsTextBox.MouseEnter += new System.EventHandler(this.TagsTextBox_MouseEnter);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 182);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(78, 16);
            this.label8.TabIndex = 11;
            this.label8.Text = "Languages:";
            // 
            // languagesTextBox
            // 
            this.languagesTextBox.Location = new System.Drawing.Point(193, 178);
            this.languagesTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.languagesTextBox.Name = "languagesTextBox";
            this.languagesTextBox.Size = new System.Drawing.Size(240, 22);
            this.languagesTextBox.TabIndex = 15;
            this.languagesTextBox.TextChanged += new System.EventHandler(this.Content_TextChanged);
            this.languagesTextBox.MouseEnter += new System.EventHandler(this.LanguagesTextBox_MouseEnter);
            // 
            // defaultDescTextBox
            // 
            this.defaultDescTextBox.BackColor = System.Drawing.Color.LightYellow;
            this.defaultDescTextBox.Location = new System.Drawing.Point(193, 146);
            this.defaultDescTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.defaultDescTextBox.Name = "defaultDescTextBox";
            this.defaultDescTextBox.Size = new System.Drawing.Size(240, 22);
            this.defaultDescTextBox.TabIndex = 12;
            this.defaultDescTextBox.TextChanged += new System.EventHandler(this.Content_TextChanged);
            this.defaultDescTextBox.MouseEnter += new System.EventHandler(this.DefaultDescTextBox_MouseEnter);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 150);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(164, 16);
            this.label7.TabIndex = 8;
            this.label7.Text = "Default Result Description:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 117);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 16);
            this.label6.TabIndex = 7;
            this.label6.Text = "Scope:";
            // 
            // helpTextBox
            // 
            this.helpTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.helpTextBox.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.helpTextBox.Location = new System.Drawing.Point(16, 560);
            this.helpTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.helpTextBox.Multiline = true;
            this.helpTextBox.Name = "helpTextBox";
            this.helpTextBox.Size = new System.Drawing.Size(951, 90);
            this.helpTextBox.TabIndex = 8;
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.saveIconToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(985, 38);
            this.menuStrip.TabIndex = 13;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.publishToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F)));
            this.fileToolStripMenuItem.ShowShortcutKeys = false;
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 34);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.ShowShortcutKeys = false;
            this.newToolStripMenuItem.Size = new System.Drawing.Size(130, 26);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.NewToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.ShowShortcutKeys = false;
            this.openToolStripMenuItem.Size = new System.Drawing.Size(130, 26);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.ShowShortcutKeys = false;
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(130, 26);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // publishToolStripMenuItem
            // 
            this.publishToolStripMenuItem.Name = "publishToolStripMenuItem";
            this.publishToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.publishToolStripMenuItem.ShowShortcutKeys = false;
            this.publishToolStripMenuItem.Size = new System.Drawing.Size(130, 26);
            this.publishToolStripMenuItem.Text = "&Publish";
            this.publishToolStripMenuItem.Click += new System.EventHandler(this.PublishToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.exitToolStripMenuItem.ShowShortcutKeys = false;
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(130, 26);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
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
            this.saveIconToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contactSupportToolStripMenuItem,
            this.licenseToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(55, 34);
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
            // publishWorker
            // 
            this.publishWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.PublishWorker_DoWork);
            this.publishWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.PublishWorker_RunWorkerCompleted);
            // 
            // CheckEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 666);
            this.Controls.Add(this.helpTextBox);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CheckEditor";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Check/Fix Editor & Publisher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.Load += new System.EventHandler(this.CheckEditor_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label idLabel;
        private System.Windows.Forms.Label checkFixIdLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox checkFixNameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox checkFindRegExTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox fixRegExTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox scopeCombo;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox defaultDescTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tagsTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox languagesTextBox;
        private System.Windows.Forms.TextBox helpTextBox;
        private System.Windows.Forms.TextBox descriptionTextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.MaskedTextBox versionTextBox;
        private System.ComponentModel.BackgroundWorker publishWorker;
        private ScintillaNET.Scintilla jsEditor;
        private System.Windows.Forms.ToolStripMenuItem publishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveIconToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contactSupportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem licenseToolStripMenuItem;
    }
}