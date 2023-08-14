namespace TvpMain.Forms
{
    partial class OptionsForm
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
            this.OkButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.repoEnabled = new System.Windows.Forms.CheckBox();
            this.repoAwsRegionLabel = new System.Windows.Forms.Label();
            this.repoNameLabel = new System.Windows.Forms.Label();
            this.repoS3BucketLabel = new System.Windows.Forms.Label();
            this.repoSyncOnStartup = new System.Windows.Forms.CheckBox();
            this.repoAwsAccessKeyLabel = new System.Windows.Forms.Label();
            this.repoAwsAccessKey = new System.Windows.Forms.TextBox();
            this.repoAwsSecretKeyLabel = new System.Windows.Forms.Label();
            this.repoS3Bucket = new System.Windows.Forms.TextBox();
            this.repoAwsRegion = new System.Windows.Forms.TextBox();
            this.repoName = new System.Windows.Forms.TextBox();
            this.repoAwsSecretKey = new System.Windows.Forms.TextBox();
            this.sharedRepoOptions = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.sharedRepoOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // OkButton
            // 
            this.OkButton.Location = new System.Drawing.Point(280, 323);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(100, 28);
            this.OkButton.TabIndex = 11;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(392, 323);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(100, 28);
            this.CancelButton.TabIndex = 12;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // repoEnabled
            // 
            this.repoEnabled.AutoSize = true;
            this.repoEnabled.Location = new System.Drawing.Point(12, 21);
            this.repoEnabled.Name = "repoEnabled";
            this.repoEnabled.Size = new System.Drawing.Size(180, 20);
            this.repoEnabled.TabIndex = 14;
            this.repoEnabled.Text = "Enable shared repository";
            this.repoEnabled.UseVisualStyleBackColor = true;
            this.repoEnabled.CheckedChanged += new System.EventHandler(this.repoEnabled_CheckedChanged);
            // 
            // repoAwsRegionLabel
            // 
            this.repoAwsRegionLabel.AutoSize = true;
            this.repoAwsRegionLabel.Location = new System.Drawing.Point(16, 71);
            this.repoAwsRegionLabel.Name = "repoAwsRegionLabel";
            this.repoAwsRegionLabel.Size = new System.Drawing.Size(88, 16);
            this.repoAwsRegionLabel.TabIndex = 0;
            this.repoAwsRegionLabel.Text = "AWS Region:";
            // 
            // repoNameLabel
            // 
            this.repoNameLabel.AutoSize = true;
            this.repoNameLabel.Location = new System.Drawing.Point(16, 35);
            this.repoNameLabel.Name = "repoNameLabel";
            this.repoNameLabel.Size = new System.Drawing.Size(47, 16);
            this.repoNameLabel.TabIndex = 0;
            this.repoNameLabel.Text = "Name:";
            // 
            // repoS3BucketLabel
            // 
            this.repoS3BucketLabel.AutoSize = true;
            this.repoS3BucketLabel.Location = new System.Drawing.Point(16, 107);
            this.repoS3BucketLabel.Name = "repoS3BucketLabel";
            this.repoS3BucketLabel.Size = new System.Drawing.Size(70, 16);
            this.repoS3BucketLabel.TabIndex = 7;
            this.repoS3BucketLabel.Text = "S3 Bucket:";
            // 
            // repoSyncOnStartup
            // 
            this.repoSyncOnStartup.AutoSize = true;
            this.repoSyncOnStartup.Location = new System.Drawing.Point(19, 221);
            this.repoSyncOnStartup.Name = "repoSyncOnStartup";
            this.repoSyncOnStartup.Size = new System.Drawing.Size(163, 20);
            this.repoSyncOnStartup.TabIndex = 5;
            this.repoSyncOnStartup.Text = "Synchronize on startup";
            this.repoSyncOnStartup.UseVisualStyleBackColor = true;
            // 
            // repoAwsAccessKeyLabel
            // 
            this.repoAwsAccessKeyLabel.AutoSize = true;
            this.repoAwsAccessKeyLabel.Location = new System.Drawing.Point(16, 143);
            this.repoAwsAccessKeyLabel.Name = "repoAwsAccessKeyLabel";
            this.repoAwsAccessKeyLabel.Size = new System.Drawing.Size(81, 16);
            this.repoAwsAccessKeyLabel.TabIndex = 8;
            this.repoAwsAccessKeyLabel.Text = "Access Key:";
            // 
            // repoAwsAccessKey
            // 
            this.repoAwsAccessKey.Location = new System.Drawing.Point(124, 140);
            this.repoAwsAccessKey.Name = "repoAwsAccessKey";
            this.repoAwsAccessKey.Size = new System.Drawing.Size(341, 22);
            this.repoAwsAccessKey.TabIndex = 3;
            this.repoAwsAccessKey.TextChanged += new System.EventHandler(this.repoAwsAccessKey_TextChanged);
            this.repoAwsAccessKey.Validating += new System.ComponentModel.CancelEventHandler(this.repoAwsAccessKey_Validating);
            // 
            // repoAwsSecretKeyLabel
            // 
            this.repoAwsSecretKeyLabel.AutoSize = true;
            this.repoAwsSecretKeyLabel.Location = new System.Drawing.Point(16, 179);
            this.repoAwsSecretKeyLabel.Name = "repoAwsSecretKeyLabel";
            this.repoAwsSecretKeyLabel.Size = new System.Drawing.Size(75, 16);
            this.repoAwsSecretKeyLabel.TabIndex = 9;
            this.repoAwsSecretKeyLabel.Text = "Secret Key:";
            // 
            // repoS3Bucket
            // 
            this.repoS3Bucket.Location = new System.Drawing.Point(124, 104);
            this.repoS3Bucket.Name = "repoS3Bucket";
            this.repoS3Bucket.Size = new System.Drawing.Size(341, 22);
            this.repoS3Bucket.TabIndex = 2;
            this.repoS3Bucket.TextChanged += new System.EventHandler(this.repoS3Bucket_TextChanged);
            this.repoS3Bucket.Validating += new System.ComponentModel.CancelEventHandler(this.repoS3Bucket_Validating);
            // 
            // repoAwsRegion
            // 
            this.repoAwsRegion.Location = new System.Drawing.Point(124, 68);
            this.repoAwsRegion.Name = "repoAwsRegion";
            this.repoAwsRegion.Size = new System.Drawing.Size(150, 22);
            this.repoAwsRegion.TabIndex = 1;
            this.repoAwsRegion.TextChanged += new System.EventHandler(this.repoAwsRegion_TextChanged);
            this.repoAwsRegion.Validating += new System.ComponentModel.CancelEventHandler(this.repoAwsRegion_Validating);
            // 
            // repoName
            // 
            this.repoName.Enabled = false;
            this.repoName.Location = new System.Drawing.Point(124, 32);
            this.repoName.Name = "repoName";
            this.repoName.Size = new System.Drawing.Size(150, 22);
            this.repoName.TabIndex = 0;
            // 
            // repoAwsSecretKey
            // 
            this.repoAwsSecretKey.Location = new System.Drawing.Point(124, 176);
            this.repoAwsSecretKey.Name = "repoAwsSecretKey";
            this.repoAwsSecretKey.Size = new System.Drawing.Size(341, 22);
            this.repoAwsSecretKey.TabIndex = 4;
            this.repoAwsSecretKey.TextChanged += new System.EventHandler(this.repoAwsSecretKey_TextChanged);
            this.repoAwsSecretKey.Validating += new System.ComponentModel.CancelEventHandler(this.repoAwsSecretKey_Validating);
            // 
            // sharedRepoOptions
            // 
            this.sharedRepoOptions.Controls.Add(this.repoAwsSecretKey);
            this.sharedRepoOptions.Controls.Add(this.repoName);
            this.sharedRepoOptions.Controls.Add(this.repoAwsRegion);
            this.sharedRepoOptions.Controls.Add(this.repoS3Bucket);
            this.sharedRepoOptions.Controls.Add(this.repoAwsSecretKeyLabel);
            this.sharedRepoOptions.Controls.Add(this.repoAwsAccessKey);
            this.sharedRepoOptions.Controls.Add(this.repoAwsAccessKeyLabel);
            this.sharedRepoOptions.Controls.Add(this.repoSyncOnStartup);
            this.sharedRepoOptions.Controls.Add(this.repoS3BucketLabel);
            this.sharedRepoOptions.Controls.Add(this.repoNameLabel);
            this.sharedRepoOptions.Controls.Add(this.repoAwsRegionLabel);
            this.sharedRepoOptions.Location = new System.Drawing.Point(12, 51);
            this.sharedRepoOptions.Name = "sharedRepoOptions";
            this.sharedRepoOptions.Size = new System.Drawing.Size(500, 256);
            this.sharedRepoOptions.TabIndex = 13;
            this.sharedRepoOptions.TabStop = false;
            this.sharedRepoOptions.Text = "Shared Repository";
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.ClientSize = new System.Drawing.Size(524, 375);
            this.Controls.Add(this.repoEnabled);
            this.Controls.Add(this.sharedRepoOptions);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.OkButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.sharedRepoOptions.ResumeLayout(false);
            this.sharedRepoOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.CheckBox repoEnabled;
        private System.Windows.Forms.GroupBox sharedRepoOptions;
        private System.Windows.Forms.TextBox repoAwsSecretKey;
        private System.Windows.Forms.TextBox repoName;
        private System.Windows.Forms.TextBox repoAwsRegion;
        private System.Windows.Forms.TextBox repoS3Bucket;
        private System.Windows.Forms.Label repoAwsSecretKeyLabel;
        private System.Windows.Forms.TextBox repoAwsAccessKey;
        private System.Windows.Forms.Label repoAwsAccessKeyLabel;
        private System.Windows.Forms.CheckBox repoSyncOnStartup;
        private System.Windows.Forms.Label repoS3BucketLabel;
        private System.Windows.Forms.Label repoNameLabel;
        private System.Windows.Forms.Label repoAwsRegionLabel;
    }
}