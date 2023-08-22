/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TvpMain.Check;

namespace TvpMain.CheckManagement
{
    /// <summary>
    /// This interface defines a class that manages a repository of checks.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// A unique identifier for the repository.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// The display name for the repository.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Determines if this repository is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Should this repository be synchronized during TVP startup.
        /// </summary>
        public bool SyncOnStartup { get; set; }

        /// <summary>
        /// Determines if the specified user is in the list of administrators for this repository.
        /// </summary>
        /// <param name="user">The Paratext user name to check.</param>
        /// <returns>true if user is in the admin list. false otherwise.</returns>
        public bool IsAdmin(string user);

        /// <summary>
        /// A list of admin user names for this repository.
        /// </summary>
        public List<string> Admins { get; }

        /// <summary>
        /// Uploads a new admin list consisting of a single user. 
        /// Warning: this will overwrite an existing admin list.
        /// </summary>
        /// <param name="userName">The user name of the admin.</param>
        public void SetAdmin(string userName);

        /// <summary>
        /// Gets a list of all checks and groups in this repository.
        /// </summary>
        /// <returns>The list of checks and groups</returns>
        public List<IRunnable> GetItems();

        /// <summary>
        /// Get a list of all checks in this repository.
        /// </summary>
        /// <returns>The list of checks</returns>
        public List<CheckAndFixItem> GetChecks();

        /// <summary>
        /// This method adds an item to a repository.
        /// </summary>
        /// <param name="filename">The filename to use for the item in the repository.</param>
        /// <param name="item">The item to add to the repository.</param>
        public void AddItem(string filename, IRunnable item);

        /// <summary>
        /// This method asynchronously adds an item to a repository.
        /// </summary>
        /// <param name="filename">The filename to use for the item in the repository.</param>
        /// <param name="item">The item to add to the repository.</param>
        /// <returns>A task representing the result of the operation.</returns>
        public Task AddItemAsync(string filename, IRunnable item);

        /// <summary>
        /// This method removes an item from a repository.
        /// </summary>
        /// <param name="filename">The filename of the item to remove.</param>
        public void RemoveItem(string filename);

        /// <summary>
        /// Deletes all checks from this repository.
        /// </summary>
        /// <returns>true if the files were successfully deleted. false otherwise.</returns>
        public bool Clear();

        /// <summary>
        /// Tests the settings for this repository to ensure that a connection can be made 
        /// and that the credentials have the needed permissions.
        /// </summary>
        /// <param name="error">An explanation of why the verification failed.</param>
        /// <returns>true if verification was successful. false otherwise.</returns>
        public bool Verify();

        /// <summary>
        /// Whether the repository connection and credentials have been tested successfully.
        /// </summary>
        public bool Verified { get; }

        /// <summary>
        /// An explanation of why verification failed or empty string if verification has 
        /// not been attempted yet.
        /// </summary>
        public string VerificationError { get; }
    }
}
