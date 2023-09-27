/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using PtxUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TvpMain.Check;
using TvpMain.Util;

namespace TvpMain.CheckManagement
{
    /// <summary>
    /// This class works with a remote, S3-based repository for checks and fixes.
    /// </summary>
    public class S3Repository : IRepository
    {
        private string _accessKey;
        private string _secretKey;
        private string _region;
        private string _bucket;

        /// <summary>
        /// A default repository name to use when one is not provided.
        /// </summary>
        private static string _defaultName = MainConsts.REMOTE_REPO_NAME;

        /// <summary>
        /// A list of the user names of all administrators for this repository.
        /// </summary>
        private List<string> _admins = null;

        /// <summary>
        /// A unique identifier for the repository.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// The display name for the repository.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Determines if the repository is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Should this repository be synchronized during TVP startup.
        /// </summary>
        public bool SyncOnStartup { get; set; }

        /// <summary>
        /// The service which interacts with S3.
        /// </summary>
        protected virtual IRemoteService Service { get; private set; } = null;

        public S3Repository(string accessKey, string secretKey, string region, string bucket)
            : this(new Guid(), _defaultName, false, false, accessKey, secretKey, region, bucket, false)
        {
        }

        public S3Repository()
        {
            Id = Guid.NewGuid();
            Name = _defaultName;
            Enabled = false;
            _accessKey = "";
            _secretKey = "";
            _region = "";
            _bucket = "";
        }

        public S3Repository(S3RepositoryOptions options) 
            : this(options.Id, options.Name, options.Enabled, options.Verified, options.AwsAccessKey, 
                  options.AwsSecretKey, options.AwsRegion, options.S3Bucket, options.SyncOnStartup)
        {
        }

        public S3Repository(Guid id, string name, bool enabled, bool verified, string accessKey, string secretKey, string region, string bucket, bool syncOnStatup)
        {
            Id = id;
            Name = name;
            SyncOnStartup = syncOnStatup;
            _accessKey = accessKey;
            _secretKey = secretKey;
            _region = region;
            _bucket = bucket;
            Enabled = enabled;
            try
            {
                Service = new S3Service(_accessKey, _secretKey, _region, _bucket);
                Verified = verified;
            }
            catch (Exception) 
            {
                Verified = false;
            }
        }

        /// <summary>
        /// A list of admin user names for this repository.
        /// </summary>
        public List<string> Admins {
            get 
            {
                if (!Enabled)
                {
                    return new List<string>();
                }

                if (_admins is null)
                {
                    try
                    {
                        var stream = Service.GetFileStream(MainConsts.PERMISSIONS_FILE_NAME);
                        using var reader = new StreamReader(stream);
                        IEnumerable<string[]> lines = CSVFile.Read(reader);
                        _admins = lines.ToList().SelectMany(item => item).ToList();
                    }
                    catch (Exception exception)
                    {
                        HostUtil.Instance.LogLine($"Failed to fetch {MainConsts.PERMISSIONS_FILE_NAME} from S3: " + exception, false);
                        _admins = new List<string>();
                    }
                }

                return _admins;
            }
        }

        /// <summary>
        /// Uploads a new admin list consisting of a single user. 
        /// Warning: this will overwrite an existing admin list.
        /// </summary>
        /// <param name="userName">The user name of the admin.</param>
        public void SetAdmin(string userName)
        {
            if (!Enabled) return;

            byte[] byteArray = Encoding.UTF8.GetBytes(userName);
            var stream = new MemoryStream(byteArray);
            Service.PutFileStream(MainConsts.PERMISSIONS_FILE_NAME, stream);
        }

        /// <summary>
        /// Determines if the specified user is in the list of administrators for this repository.
        /// </summary>
        /// <param name="user">The Paratext user name to check.</param>
        /// <returns>true if user is in the admin list. false otherwise.</returns>
        public bool IsAdmin(string user)
        {
            return Admins.Contains(user);
        }

        public void AddItem(string filename, IRunnable item)
        {
            if (String.IsNullOrEmpty(filename)) new ArgumentNullException(nameof(filename));

            Service.PutFileStream(filename, item.WriteToXmlStream());
        }

        public async Task AddItemAsync(string filename, IRunnable item)
        {
            if (String.IsNullOrEmpty(filename)) new ArgumentNullException(nameof(filename));

            await Service.PutFileStreamAsync(filename, item.WriteToXmlStream());
        }

        /// <summary>
        /// Get a list of all checks and groups in this repository.
        /// </summary>
        /// <returns>The list of checks and groups</returns>
        public List<IRunnable> GetItems()
        {
            var items = new List<IRunnable>();
            var groups = new List<CheckGroup>();

            // Whether the string represents an XML filename.
            static bool IsXmlFile(string filename) => filename.Trim().ToLowerInvariant().EndsWith(".xml");

            var checkRegex = new Regex($@"\.{Regex.Escape(CheckAndFixItem.FileExtension)}$");
            var groupRegex = new Regex($@"\.{Regex.Escape(CheckGroup.FileExtension)}$");
            var fileNames = Service.ListAllFiles().Where((Func<string,bool>) IsXmlFile).ToList();
            foreach (var fileName in fileNames)
            {
                try 
                {
                    IRunnable item;
                    using var fileStream = Service.GetFileStream(fileName);
                    if (groupRegex.IsMatch(fileName))
                    {
                        // *.group.xml files
                        item = CheckGroup.LoadFromXmlContent(fileStream);
                        if (item != null) groups.Add((CheckGroup)item);
                    }
                    else if (checkRegex.IsMatch(fileName))
                    {
                        // *.check.xml files
                        item = CheckAndFixItem.LoadFromXmlContent(fileStream);
                    }
                    else
                    {
                        // *.xml files
                        item = CheckAndFixItem.LoadFromXmlContent(fileStream);
                    }

                    if (item != null)
                    {
                        item.FileName = Path.GetFileName(fileName);
                        items.Add(item);
                    }
                }
                catch (Exception e)
                {
                    throw new FileLoadException($"Unable to load '{fileName}'.", e.InnerException);
                }
            }

            foreach (var group in groups)
            {
                var newChecks = new List<KeyValuePair<string, CheckAndFixItem>>();
                foreach (var checkKvp in group.Checks)
                {
                    if (checkKvp.Value is null)
                    {
                        var foundItem = items.Find(item => item.Id == checkKvp.Key);
                        if (foundItem != null && foundItem is CheckAndFixItem check)
                        {
                            newChecks.Add(new KeyValuePair<string, CheckAndFixItem>(check.Id, check));
                        }
                        else
                        {
                            newChecks.Add(checkKvp);
                        }
                    }
                    else
                    {
                        newChecks.Add(checkKvp);
                    }
                }

                group.Checks = newChecks;
            }

            return items;
        }

        /// <summary>
        /// Get a list of all checks in this repository.
        /// </summary>
        /// <returns>The list of checks</returns>
        public List<CheckAndFixItem> GetChecks()
        {
            var checks = new List<CheckAndFixItem>();
            var items = GetItems();
            foreach (IRunnable item in items)
            {
                if (item is CheckAndFixItem check)
                {
                    checks.Add(check);
                }
            }

            return checks;
        }

        public void RemoveItem(string filename)
        {
            Service.DeleteFile(filename);
        }

        /// <summary>
        /// Deletes all checks from this repository.
        /// </summary>
        /// <returns>true if the files were successfully deleted. false otherwise.</returns>
        public bool Clear()
        {
            // Deletion of all remote repository files is not currently supported.
            return false;
        }

        /// <summary>
        /// Tests the settings for this repository to ensure that a connection can be made 
        /// and that the credentials have the needed permissions.
        /// </summary>
        /// <param name="error">An explanation of why the verification failed.</param>
        /// <returns>true if verification was successful. false otherwise.</returns>
        public bool Verify()
        {
            if (!S3Service.ValidateAccessKey(_accessKey))
            {
                VerificationError = "Invalid AWS access key format.";
                Verified = false;
                return Verified;
            }

            if (!S3Service.ValidateSecretKey(_secretKey))
            {
                VerificationError = "Invalid AWS secret key format.";
                Verified = false;
                return Verified;
            }

            if (!S3Service.ValidateAwsRegion(_region))
            {
                VerificationError = "Invalid AWS region format.";
                Verified = false;
                return Verified;
            }

            if (!S3Service.ValidateS3BucketName(_bucket))
            {
                VerificationError = "Invalid Amazon S3 bucket name format.";
                Verified = false;
                return Verified;
            }

            try
            {
                if (Service is null)
                {
                    Service = new S3Service(_accessKey, _secretKey, _region, _bucket);
                }
            }
            catch (Exception ex)
            {
                VerificationError = $"Could not initialize the S3 service because an error occurred. {ex.Message}";
                Verified = false;
                return Verified;
            }

            var error = "";
            Verified = Service.Verify(out error);
            VerificationError = error;

            return Verified;
        }

        /// <summary>
        /// Whether the repository connection and credentials have been tested successfully.
        /// </summary>
        public bool Verified { get; private set; }

        /// <summary>
        /// An explanation of why verification failed or empty string if verification has 
        /// not been attempted yet.
        /// </summary>
        public string VerificationError { get; private set; } = "";
    }
}
