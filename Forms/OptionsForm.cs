using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TvpMain.CheckManagement;
using TvpMain.Project;
using TvpMain.Util;

namespace TvpMain.Forms
{
    /// <summary>
    /// A form for editing TVP settings.
    /// </summary>
    public partial class OptionsForm : Form
    {
        private TvpOptions _oldOptions = null;
        private TvpOptions _newOptions = null;
        private bool _submitted = false;

        public string repoId { get; set; }
        public bool repoVerified { get; set; }

        public OptionsForm(TvpOptions options)
        {
            InitializeComponent();
            _oldOptions = options;

            if (options.SharedRepositories.Count > 0)
            {
                repoId = options.SharedRepositories[0].Id.ToString();
                repoVerified = options.SharedRepositories[0].Verified;
                repoName.Text = options.SharedRepositories[0].Name;
                repoEnabled.Checked = options.SharedRepositories[0].Enabled;
                repoS3Bucket.Text = options.SharedRepositories[0].S3Bucket;
                repoAwsRegion.Text = options.SharedRepositories[0].AwsRegion;
                repoAwsAccessKey.Text = options.SharedRepositories[0].AwsAccessKey;
                repoAwsSecretKey.Text = options.SharedRepositories[0].AwsSecretKey;
                repoSyncOnStartup.Checked = options.SharedRepositories[0].SyncOnStartup;
            }
            else
            {
                repoId = Guid.NewGuid().ToString();
                repoVerified = false;
                repoName.Text = MainConsts.REMOTE_REPO_NAME;
                repoEnabled.Checked = false;
                repoSyncOnStartup.Checked = false;
            }
            sharedRepoOptions.Enabled = repoEnabled.Checked;
        }

        /// <summary>
        /// Checks to see if any repository settings have changed since the form was created. 
        /// </summary>
        /// <returns></returns>
        public bool HasChanges()
        {
            if (!_submitted) return false;
            if (_oldOptions.SharedRepositories.Count < 1) return true;

            var newS3Options = getOptions().SharedRepositories[0];
            var oldS3Options = _oldOptions.SharedRepositories[0];
            if (newS3Options.Id != oldS3Options.Id) return true;
            if (newS3Options.Enabled != oldS3Options.Enabled) return true;
            if (newS3Options.Name != oldS3Options.Name) return true;
            if (newS3Options.AwsRegion != oldS3Options.AwsRegion) return true;
            if (newS3Options.S3Bucket != oldS3Options.S3Bucket) return true;
            if (newS3Options.AwsAccessKey != oldS3Options.AwsAccessKey) return true;
            if (newS3Options.AwsSecretKey != oldS3Options.AwsSecretKey) return true;
            if (newS3Options.SyncOnStartup != oldS3Options.SyncOnStartup) return true;

            return false; 
        }

        /// <summary>
        /// Returns the option values entered on the Options form.
        /// </summary>
        /// <returns>A <c>TvpOptions</c> object with all TVP option values.</returns>
        public TvpOptions getOptions()
        {
            if (_newOptions is null)
            {
                _newOptions = new TvpOptions();
                var newS3Options = new CheckManagement.S3RepositoryOptions();
                newS3Options.Id = new Guid(repoId);
                newS3Options.Name = repoName.Text;
                newS3Options.Enabled = repoEnabled.Checked;
                newS3Options.Verified = repoVerified;
                newS3Options.S3Bucket = repoS3Bucket.Text;
                newS3Options.AwsRegion = repoAwsRegion.Text;
                newS3Options.AwsAccessKey = repoAwsAccessKey.Text;
                newS3Options.AwsSecretKey = repoAwsSecretKey.Text;
                newS3Options.SyncOnStartup = repoSyncOnStartup.Checked;
                _newOptions.SharedRepositories.Add(newS3Options);
            }

            return _newOptions;
        }

        /// <summary>
        /// Handles OK button click. Closes the Options form.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void OkButton_Click(object sender, EventArgs e)
        {
            if (ValidateChildren(ValidationConstraints.Enabled))
            {
                DialogResult = DialogResult.OK;
                _submitted = true;
                Close();
            }
        }

        /// <summary>
        /// Validates that the value entered is in the correct format for an AWS access key.
        /// It does not guarantee that the value is a valid key that actually exists in AWS.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void repoAwsAccessKey_Validating(object sender, CancelEventArgs e)
        {
            if (!S3Service.ValidateAccessKey(repoAwsAccessKey.Text))
            {
                e.Cancel = true;
                errorProvider.SetError(repoAwsAccessKey, "Please enter a valid AWS access key.");
            }
            else
            {
                e.Cancel = false;
                errorProvider.SetError(repoAwsAccessKey, "");
            }
        }

        /// <summary>
        /// Validates that the key entered is in the correct format for an AWS secret key.
        /// It does not guarantee that the key is a valid key that actually exists in AWS.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void repoAwsSecretKey_Validating(object sender, CancelEventArgs e)
        {
            if (!S3Service.ValidateSecretKey(repoAwsSecretKey.Text))
            {
                e.Cancel = true;
                errorProvider.SetError(repoAwsSecretKey, "Please enter a valid AWS secret key.");
            }
            else
            {
                e.Cancel = false;
                errorProvider.SetError(repoAwsSecretKey, "");
            }
        }

        /// <summary>
        /// Validates that the value entered is in the correct format for an Amazon S3 bucket name.
        /// It does not guarantee that it is the name of a valid bucket that actually exists in AWS.
        /// reference: https://docs.aws.amazon.com/AmazonS3/latest/userguide/bucketnamingrules.html
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void repoS3Bucket_Validating(object sender, CancelEventArgs e)
        {
            if (!S3Service.ValidateS3BucketName(repoS3Bucket.Text))
            {
                e.Cancel = true;
                errorProvider.SetError(repoS3Bucket, "Please enter a valid Amazon S3 bucket name");
            }
            else
            {
                e.Cancel = false;
                errorProvider.SetError(repoS3Bucket, "");
            }
        }

        /// <summary>
        /// Validates that the value entered is in the correct format for an AWS region name based 
        /// on existing regions. It does not guarantee that it is the name of a valid region that 
        /// actually exists in AWS.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void repoAwsRegion_Validating(object sender, CancelEventArgs e)
        {
            if (!S3Service.ValidateAwsRegion(repoAwsRegion.Text))
            {
                e.Cancel = true;
                errorProvider.SetError(repoAwsRegion, "Please enter a valid AWS region name.");
            }
            else
            {
                e.Cancel = false;
                errorProvider.SetError(repoAwsRegion, "");
            }
        }

        /// <summary>
        /// When the repository is enabled/disabled:
        /// - enable/disable the repository settings controls
        /// - if disabled, remove any existing validation errors 
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void repoEnabled_CheckedChanged(object sender, EventArgs e)
        {
            sharedRepoOptions.Enabled = repoEnabled.Checked;
            if (!repoEnabled.Checked)
            {
                errorProvider.SetError(repoAwsRegion, "");
                errorProvider.SetError(repoS3Bucket, "");
                errorProvider.SetError(repoAwsAccessKey, "");
                errorProvider.SetError(repoAwsSecretKey, "");
            }
        }

        /// <summary>
        /// Invalidates the repository settings if the AWS region has been changed.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void repoAwsRegion_TextChanged(object sender, EventArgs e)
        {
            repoVerified = false;
        }

        /// <summary>
        /// Invalidates the repository settings if the S3 bucket name has been changed.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void repoS3Bucket_TextChanged(object sender, EventArgs e)
        {
            repoVerified = false;
        }

        /// <summary>
        /// Invalidates the repository settings if the AWS access key has been changed.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void repoAwsAccessKey_TextChanged(object sender, EventArgs e)
        {
            repoVerified = false;
        }

        /// <summary>
        /// Invalidates the repository settings if the AWS secret key has been changed.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void repoAwsSecretKey_TextChanged(object sender, EventArgs e)
        {
            repoVerified = false;
        }
    }
}
