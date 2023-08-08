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
            this.repoName = new System.Windows.Forms.TextBox();
            this.repoAwsRegion = new System.Windows.Forms.TextBox();
            this.repoS3Bucket = new System.Windows.Forms.TextBox();
            this.repoAwsAccessKey = new System.Windows.Forms.TextBox();
            this.repoSyncOnStartup = new System.Windows.Forms.CheckBox();
            this.repoNameLabel = new System.Windows.Forms.Label();
            this.repoAwsRegionLabel = new System.Windows.Forms.Label();
            this.repoS3BucketLabel = new System.Windows.Forms.Label();
            this.repoAwsAccessKeyLabel = new System.Windows.Forms.Label();
            this.repoAwsSecretKeyLabel = new System.Windows.Forms.Label();
            this.repoAwsSecretKey = new System.Windows.Forms.TextBox();
            this.OkButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.sharedRepoBox = new System.Windows.Forms.GroupBox();
            this.sharedRepoBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // repoName
            // 
            this.repoName.Location = new System.Drawing.Point(124, 32);
            this.repoName.Name = "repoName";
            this.repoName.Size = new System.Drawing.Size(150, 22);
            this.repoName.TabIndex = 0;
            // 
            // repoAwsRegion
            // 
            this.repoAwsRegion.Location = new System.Drawing.Point(124, 68);
            this.repoAwsRegion.Name = "repoAwsRegion";
            this.repoAwsRegion.Size = new System.Drawing.Size(150, 22);
            this.repoAwsRegion.TabIndex = 1;
            // 
            // repoS3Bucket
            // 
            this.repoS3Bucket.Location = new System.Drawing.Point(124, 104);
            this.repoS3Bucket.Name = "repoS3Bucket";
            this.repoS3Bucket.Size = new System.Drawing.Size(250, 22);
            this.repoS3Bucket.TabIndex = 2;
            // 
            // repoAwsAccessKey
            // 
            this.repoAwsAccessKey.Location = new System.Drawing.Point(124, 140);
            this.repoAwsAccessKey.Name = "repoAwsAccessKey";
            this.repoAwsAccessKey.Size = new System.Drawing.Size(330, 22);
            this.repoAwsAccessKey.TabIndex = 3;
            // 
            // repoSyncOnStartup
            // 
            this.repoSyncOnStartup.AutoSize = true;
            this.repoSyncOnStartup.Location = new System.Drawing.Point(19, 221);
            this.repoSyncOnStartup.Name = "repoSyncOnStartup";
            this.repoSyncOnStartup.Size = new System.Drawing.Size(163, 20);
            this.repoSyncOnStartup.TabIndex = 4;
            this.repoSyncOnStartup.Text = "Synchronize on startup";
            this.repoSyncOnStartup.UseVisualStyleBackColor = true;
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
            // repoAwsRegionLabel
            // 
            this.repoAwsRegionLabel.AutoSize = true;
            this.repoAwsRegionLabel.Location = new System.Drawing.Point(16, 71);
            this.repoAwsRegionLabel.Name = "repoAwsRegionLabel";
            this.repoAwsRegionLabel.Size = new System.Drawing.Size(88, 16);
            this.repoAwsRegionLabel.TabIndex = 0;
            this.repoAwsRegionLabel.Text = "AWS Region:";
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
            // repoAwsAccessKeyLabel
            // 
            this.repoAwsAccessKeyLabel.AutoSize = true;
            this.repoAwsAccessKeyLabel.Location = new System.Drawing.Point(16, 143);
            this.repoAwsAccessKeyLabel.Name = "repoAwsAccessKeyLabel";
            this.repoAwsAccessKeyLabel.Size = new System.Drawing.Size(81, 16);
            this.repoAwsAccessKeyLabel.TabIndex = 8;
            this.repoAwsAccessKeyLabel.Text = "Access Key:";
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
            // repoAwsSecretKey
            // 
            this.repoAwsSecretKey.Location = new System.Drawing.Point(124, 176);
            this.repoAwsSecretKey.Name = "repoAwsSecretKey";
            this.repoAwsSecretKey.Size = new System.Drawing.Size(330, 22);
            this.repoAwsSecretKey.TabIndex = 10;
            // 
            // OkButton
            // 
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkButton.Location = new System.Drawing.Point(111, 286);
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
            this.CancelButton.Location = new System.Drawing.Point(229, 286);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(100, 28);
            this.CancelButton.TabIndex = 12;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // sharedRepoBox
            // 
            this.sharedRepoBox.Controls.Add(this.repoAwsSecretKey);
            this.sharedRepoBox.Controls.Add(this.repoName);
            this.sharedRepoBox.Controls.Add(this.repoAwsRegion);
            this.sharedRepoBox.Controls.Add(this.repoS3Bucket);
            this.sharedRepoBox.Controls.Add(this.repoAwsSecretKeyLabel);
            this.sharedRepoBox.Controls.Add(this.repoAwsAccessKey);
            this.sharedRepoBox.Controls.Add(this.repoAwsAccessKeyLabel);
            this.sharedRepoBox.Controls.Add(this.repoSyncOnStartup);
            this.sharedRepoBox.Controls.Add(this.repoS3BucketLabel);
            this.sharedRepoBox.Controls.Add(this.repoNameLabel);
            this.sharedRepoBox.Controls.Add(this.repoAwsRegionLabel);
            this.sharedRepoBox.Location = new System.Drawing.Point(12, 15);
            this.sharedRepoBox.Name = "sharedRepoBox";
            this.sharedRepoBox.Size = new System.Drawing.Size(467, 256);
            this.sharedRepoBox.TabIndex = 13;
            this.sharedRepoBox.TabStop = false;
            this.sharedRepoBox.Text = "Shared Repository";
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.OkButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(491, 327);
            this.Controls.Add(this.sharedRepoBox);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.OkButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Options";
            this.sharedRepoBox.ResumeLayout(false);
            this.sharedRepoBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox repoName;
        private System.Windows.Forms.TextBox repoAwsRegion;
        private System.Windows.Forms.TextBox repoS3Bucket;
        private System.Windows.Forms.TextBox repoAwsAccessKey;
        private System.Windows.Forms.CheckBox repoSyncOnStartup;
        private System.Windows.Forms.Label repoNameLabel;
        private System.Windows.Forms.Label repoAwsRegionLabel;
        private System.Windows.Forms.Label repoS3BucketLabel;
        private System.Windows.Forms.Label repoAwsAccessKeyLabel;
        private System.Windows.Forms.Label repoAwsSecretKeyLabel;
        private System.Windows.Forms.TextBox repoAwsSecretKey;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.GroupBox sharedRepoBox;
    }
}