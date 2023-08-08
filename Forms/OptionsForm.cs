using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TvpMain.Project;

namespace TvpMain.Forms
{
    /// <summary>
    /// A form for editing TVP settings.
    /// </summary>
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Returns the option values entered on the Options form.
        /// </summary>
        /// <returns>A <c>TvpOptions</c> object with all TVP option values.</returns>
        public TvpOptions getOptions()
        {
            var options = new TvpOptions();
            options.SharedRepo.Name = repoName.Text;
            options.SharedRepo.S3Bucket = repoS3Bucket.Text;
            options.SharedRepo.AwsRegion = repoAwsRegion.Text;
            options.SharedRepo.AwsAccessKey = repoAwsAccessKey.Text;
            options.SharedRepo.AwsSecretKey = repoAwsSecretKey.Text;
            options.SharedRepo.SyncOnStartup = repoSyncOnStartup.Checked;

            return options;
        }

        /// <summary>
        /// Handles OK button click. Closes the Options form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
