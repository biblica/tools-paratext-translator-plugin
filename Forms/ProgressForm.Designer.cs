/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace TvpMain.Forms
{
    partial class ProgressForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressForm));
            this.pbrStatus = new System.Windows.Forms.ProgressBar();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblElapsedTime = new System.Windows.Forms.Label();
            this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // pbrStatus
            // 
            this.pbrStatus.Location = new System.Drawing.Point(15, 33);
            this.pbrStatus.MarqueeAnimationSpeed = 10;
            this.pbrStatus.Maximum = 111;
            this.pbrStatus.Name = "pbrStatus";
            this.pbrStatus.Size = new System.Drawing.Size(367, 23);
            this.pbrStatus.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbrStatus.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(95, 13);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Running Checks...";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(307, 64);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.OnCancelClick);
            // 
            // lblElapsedTime
            // 
            this.lblElapsedTime.AutoSize = true;
            this.lblElapsedTime.Location = new System.Drawing.Point(12, 69);
            this.lblElapsedTime.Name = "lblElapsedTime";
            this.lblElapsedTime.Size = new System.Drawing.Size(34, 13);
            this.lblElapsedTime.TabIndex = 4;
            this.lblElapsedTime.Text = "00:00";
            this.lblElapsedTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tmrUpdate
            // 
            this.tmrUpdate.Enabled = true;
            this.tmrUpdate.Interval = 200;
            this.tmrUpdate.Tick += new System.EventHandler(this.OnTimerUpdate);
            // 
            // ProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(391, 99);
            this.ControlBox = false;
            this.Controls.Add(this.lblElapsedTime);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pbrStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProgressForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.ProgressForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbrStatus;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblElapsedTime;
        private System.Windows.Forms.Timer tmrUpdate;
    }
}