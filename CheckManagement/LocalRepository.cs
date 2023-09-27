﻿/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using PtxUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TvpMain.Check;
using TvpMain.Util;

namespace TvpMain.CheckManagement
{
    /// <summary>
    /// This class works with a local, file-based repository for checks and fixes.
    /// </summary>
    public class LocalRepository : IRepository
    {
        /// <summary>
        /// The path where checks should be persisted.
        /// </summary>
        private string FolderPath { get; }

        /// <summary>
        /// A default repository name to use when one is not provided.
        /// </summary>
        private string _defaultName = MainConsts.LOCAL_REPO_NAME;

        public LocalRepository(string folderPath)
        {
            FolderPath = folderPath;
        }

        /// <summary>
        /// A unique identifier for the repository.
        /// </summary>
        public Guid Id { get { return default(Guid); } }

        /// <summary>
        /// The display name for the repository.
        /// </summary>
        public string Name
        {
            get 
            {
                // The local repository cannot be renamed.
                return _defaultName;
            }

            set
            {
                throw new Exception("The Name property cannot be set on a local repository.");
            }
        }

        /// <summary>
        /// Determines if this repository is enabled
        /// </summary>
        public bool Enabled
        {
            get
            {
                // The local repository cannot be disabled.
                return true; 
            }

            set 
            {
                throw new Exception("The Enabled property cannot be set on a local repository.");
            }
        }

        /// <summary>
        /// Should this repository be synchronized during TVP startup.
        /// </summary>
        public bool SyncOnStartup
        { 
            get 
            {
                // Local repositories do not need to be synchronized.
                return false; 
            }

            set
            {
                throw new Exception("The SyncOnStartup property cannot be set on a local repository.");
            }
        }

        /// <summary>
        /// A list of admin user names for this repository.
        /// </summary>
        public List<string> Admins { get; } = new List<string>();

        /// <summary>
        /// Uploads a new admin list consisting of a single user.
        /// </summary>
        /// <param name="userName">The user name of the admin.</param>
        public void SetAdmin(string userName) { }

        /// <summary>
        /// Determines if the specified user is in the list of administrators for this repository.
        /// </summary>
        /// <param name="user">The Paratext user name the check.</param>
        /// <returns>true if user is in the admin list. false otherwise.</returns>
        public bool IsAdmin(string user)
        {
            // Always true since local repositories do not have an adminstrator list.
            return true;
        }

        public void AddItem(string filename, IRunnable item)
        {
            if (string.IsNullOrEmpty(filename)) throw new ArgumentNullException(nameof(filename));

            VerifyFolderPath();
            var filePath = Path.Combine(FolderPath, filename);

            try
            {
                item.SaveToXmlFile(filePath);
            }
            catch (Exception e)
            {
                throw new FileWriteException($"There was a problem writing to '{filePath}'.", e.InnerException);
            }
        }

        public Task AddItemAsync(string filename, IRunnable item)
        {
            return Task.Run(() => AddItem(filename, item));
        }

        public void RemoveItem(string filename)
        {
            var filePath = Path.Combine(FolderPath, filename);
            if (!File.Exists(filePath)) return;

            File.Delete(filePath);
        }

        public List<IRunnable> GetItems()
        {
            var items = new List<IRunnable>();
            var groups = new List<CheckGroup>();
            VerifyFolderPath();

            var checkRegex = new Regex($@"\.{Regex.Escape(CheckAndFixItem.FileExtension)}$");
            var groupRegex = new Regex($@"\.{Regex.Escape(CheckGroup.FileExtension)}$");
            var xmlFiles = Directory.GetFiles(FolderPath, "*.xml");

            foreach (var filePath in xmlFiles)
            {
                try
                {
                    IRunnable item;
                    if (groupRegex.IsMatch(filePath))
                    {
                        // *.group.xml files
                        item = CheckGroup.LoadFromXmlFile(filePath);
                        if (item != null) groups.Add((CheckGroup)item);
                    }
                    else if (checkRegex.IsMatch(filePath))
                    {
                        // *.check.xml files
                        item = CheckAndFixItem.LoadFromXmlFile(filePath);
                    }
                    else
                    {
                        // *.xml files
                        item = CheckAndFixItem.LoadFromXmlFile(filePath);
                    }

                    if (item != null)
                    {
                        item.FileName = Path.GetFileName(filePath);
                        items.Add(item);
                    }
                }
                catch (Exception e)
                {
                    throw new FileLoadException($"Unable to load '{filePath}'.", e.InnerException);
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

        /// <summary>
        /// This method ensures that the folder exists.
        /// </summary>
        private void VerifyFolderPath()
        {
            Directory.CreateDirectory(FolderPath);
        }

        /// <summary>
        /// Deletes all checks from this repository.
        /// </summary>
        /// <returns>true if the files were successfully deleted. false otherwise.</returns>
        public bool Clear()
        {
            try
            {
                var xmlFiles = Directory.EnumerateFiles(FolderPath, "*.xml");
                foreach (var xmlFile in xmlFiles)
                {
                    File.Delete(xmlFile);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Tests the settings for this repository to ensure that a connection can be made 
        /// and that the credentials have the needed permissions.
        /// </summary>
        /// <param name="error">An explanation of why the verification failed.</param>
        /// <returns>true if verification was successful. false otherwise.</returns>
        public bool Verify()
        {
            return Verified;
        }

        /// <summary>
        /// Whether the repository connection and credentials have been tested successfully.
        /// </summary>
        public bool Verified { get; } = true;

        /// <summary>
        /// An explanation of why verification failed or empty string if verification has 
        /// not been attempted yet.
        /// </summary>
        public string VerificationError { get; } = "";

    }
}
