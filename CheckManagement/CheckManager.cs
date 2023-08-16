/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using PtxUtils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.ServiceModel.Channels;
using System.Web.UI;
using TvpMain.Check;
using TvpMain.Project;
using TvpMain.Util;

namespace TvpMain.CheckManagement
{
    /// <summary>
    /// This class saves, deletes, publishes, and synchronizes checks.
    /// </summary>
    public class CheckManager : ICheckManager
    {
        private readonly OptionsManager _optionsManager;
        private readonly IRepository _installedChecksRepository;
        private readonly IRepository _localRepository;
        private IRepository _remoteRepository = null;
        private static readonly string SyncStatusFileName =
            $"{Directory.GetCurrentDirectory()}\\{MainConsts.TVP_FOLDER_NAME}\\{MainConsts.LAST_SYNC_FILE_NAME}";

        /// <summary>
        /// Default constructor for CheckManager
        /// </summary>
        public CheckManager(OptionsManager optionsManager)
        {
            _optionsManager = optionsManager;
            _installedChecksRepository = new LocalRepository(Path.Combine(Directory.GetCurrentDirectory(),
                MainConsts.INSTALLED_CHECK_FOLDER_NAME));
            _localRepository = new LocalRepository(Path.Combine(Directory.GetCurrentDirectory(),
                MainConsts.LOCAL_CHECK_FOLDER_NAME));
            SetupRemoteRepository();
        }

        /// <summary>
        /// Keeps track of whether a sync has run during this ParaText session.
        /// </summary>
        public static bool HasSyncRun
        {
            get
            {
                var foundText = File.Exists(SyncStatusFileName) ? File.ReadAllLines(SyncStatusFileName)[0] : "";

                return string.Equals(foundText,
                    System.Diagnostics.Process.GetProcessesByName("Paratext")[0].Id.ToString());
            }
            set
            {
                var output = value
                    ? new[]
                    {
                        System.Diagnostics.Process.GetProcessesByName("Paratext")[0].Id.ToString(),
                        DateTime.Now.ToString(CultureInfo.InvariantCulture)
                    }
                    : new string[] { };
                File.WriteAllLines(SyncStatusFileName, output);
            }
        }

        /// <summary>
        /// The last time synchronization completed.
        /// </summary>
        public static DateTime? LastSyncTime
        {
            get
            {
                if (!File.Exists(SyncStatusFileName))
                {
                    return null;
                }
                var syncStatus = File.ReadAllLines(SyncStatusFileName);
                if (syncStatus.Length < 2) return null;
                DateTime.TryParse(syncStatus[1], out var lastRunDate);

                return lastRunDate;
            }
        }

        /// <summary>
        /// Sets up the remote S3 repository based on settings in the TVP options file.  
        /// </summary>
        public virtual void SetupRemoteRepository()
        {
            var options = _optionsManager.LoadOptions();
            if (options.SharedRepositories.Count > 0)
            {
                _remoteRepository = new S3Repository(options.SharedRepositories[0]);
                if (!_remoteRepository.Verified && _remoteRepository.Enabled) 
                {
                    _remoteRepository.Verify();
                    options.SharedRepositories[0].Verified = _remoteRepository.Verified;
                    if (!_remoteRepository.Verified && _remoteRepository.Enabled)
                    {
                        _remoteRepository.Enabled = false;
                        options.SharedRepositories[0].Enabled = false;
                        _installedChecksRepository.Clear();
                    }
                    _optionsManager.SaveOptions(options);
                }
                // If no permissions file exists for this repository, make the current user the admin. 
                if (_remoteRepository.Enabled && _remoteRepository.Admins.Count < 1)
                {
                    _remoteRepository.SetAdmin(HostUtil.Instance.CurrentUser);
                }
            }
            else
            {
                _remoteRepository = null;
            }
        }

        /// <summary>
        /// Indicates if the remote repository settings have been verified.
        /// </summary>
        /// <param name="error">If not verified, a message explaining why the verification failed.</param>
        /// <returns>true if the remote repository is verified.</returns>
        public bool RemoteRepositoryIsVerified(out string error)
        {
            error = _remoteRepository.VerificationError;
            return _remoteRepository.Verified;
        }

        /// <summary>
        /// Method to determine if the user is an administrator of the remote repository. 
        /// Relies on a list of admins hosted on the remote repository.
        /// </summary>
        /// <returns>True, if the current user is an admin.</returns>
        public bool IsCurrentUserRemoteAdmin()
        {
            return _remoteRepository.IsAdmin(HostUtil.Instance.CurrentUser);
        }

        /// <summary>
        /// Indicates if the remote repository should be synchronized on startup.
        /// </summary>
        /// <returns>true if sync on startup is enabled.</returns>
        public bool SyncOnStartup()
        {
            if (RemoteRepositoryIsEnabled() && _remoteRepository.SyncOnStartup)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// This method synchronizes installed <c>CheckAndFixItem</c>s with the remote repository.
        /// </summary>
        /// <param name="dryRun">(optional) If true, returns the result of the operation without applying it.</param>
        /// <returns>A key/value pair mapping of the result, with <c>CheckAndFixItem</c>s grouped by enumerated values.</returns>
        public virtual Dictionary<SynchronizationResultType, List<CheckAndFixItem>> SynchronizeInstalledChecks(
            bool dryRun = false)
        {
            var availableChecks = GetAvailableCheckAndFixItems();
            var installedChecks = GetInstalledCheckAndFixItems();

            var newCheckAndFixItems = GetNewCheckAndFixItems(availableChecks, installedChecks);
            var outdatedCheckAndFixItems = GetOutdatedCheckAndFixItems(availableChecks, installedChecks);
            var deprecatedCheckAndFixItems = GetDeprecatedCheckAndFixItems(availableChecks, installedChecks);

            if (dryRun)
            {
                return new Dictionary<SynchronizationResultType, List<CheckAndFixItem>>
                {
                    [SynchronizationResultType.New] = newCheckAndFixItems,
                    [SynchronizationResultType.Outdated] = outdatedCheckAndFixItems.Keys.ToList(),
                    [SynchronizationResultType.Updated] = outdatedCheckAndFixItems.Values.ToList(),
                    [SynchronizationResultType.Deprecated] = deprecatedCheckAndFixItems
                };
            }

            foreach (var check in deprecatedCheckAndFixItems)
            {
                UninstallCheckAndFixItem(check);
            }

            // Handle any situation where a newer check was removed from remote, but an older version still exists.
            newCheckAndFixItems = GetNewCheckAndFixItems(availableChecks, installedChecks);

            foreach (var check in newCheckAndFixItems)
            {
                InstallCheckAndFixItem(check);
            }

            foreach (var check in outdatedCheckAndFixItems.Keys)
            {
                UninstallCheckAndFixItem(check);
            }

            foreach (var check in outdatedCheckAndFixItems.Values)
            {
                InstallCheckAndFixItem(check);
            }

            return new Dictionary<SynchronizationResultType, List<CheckAndFixItem>>
            {
                [SynchronizationResultType.New] = newCheckAndFixItems,
                [SynchronizationResultType.Outdated] = outdatedCheckAndFixItems.Keys.ToList(),
                [SynchronizationResultType.Updated] = outdatedCheckAndFixItems.Values.ToList(),
                [SynchronizationResultType.Deprecated] = deprecatedCheckAndFixItems
            };
        }

        /// <summary>
        /// Indicates if a remote repository exists and is enabled.
        /// </summary>
        /// <returns>true if a remote repo is enabled.</returns>
        public bool RemoteRepositoryIsEnabled()
        {
            return !(_remoteRepository is null) && _remoteRepository.Enabled;
        }

        /// <summary>
        /// This method retrieves any checks which exist in the remote repository, but for which no version has been installed.
        /// </summary>
        /// <returns>A list of new <c>CheckAndFixItem</c>s which can be installed.</returns>
        internal virtual List<CheckAndFixItem> GetNewCheckAndFixItems()
        {
            var availableChecks = GetAvailableCheckAndFixItems();
            var installedChecks = GetInstalledCheckAndFixItems();

            return GetNewCheckAndFixItems(availableChecks, installedChecks);
        }

        /// <summary>
        /// This method retrieves any checks which exist in the remote repository, but for which no version has been installed.
        /// </summary>
        /// <param name="availableChecks">A list of available <c>CheckAndFixItem</c>s.</param>
        /// <param name="installedChecks">A list of installed <c>CheckAndFixItem</c>s.</param>
        /// <returns>A list of new <c>CheckAndFixItem</c>s which can be installed.</returns>
        private List<CheckAndFixItem> GetNewCheckAndFixItems(List<CheckAndFixItem> availableChecks, List<CheckAndFixItem> installedChecks)
        {
            var installedCheckIds = from installedCheck in installedChecks
                                    select new
                                    {
                                        installedCheck.Id
                                    };
            var availableChecksSorted = from availableCheck in availableChecks
                                        orderby availableCheck.Id ascending, availableCheck.Version descending
                                        select availableCheck;

            var newChecks = new List<CheckAndFixItem>();
            foreach (var availableCheck in availableChecksSorted )
            {
                var val1 = installedCheckIds.Contains(new { availableCheck.Id });
                if (!installedCheckIds.Contains(new { availableCheck.Id }) && 
                    !newChecks.Exists(newCheck => newCheck.Id == availableCheck.Id))
                {
                    newChecks.Add(availableCheck);
                }
            }

            return newChecks;
        }

        /// <summary>
        /// This method determines which <c>CheckAndFixItem</c>s have been installed, but are no longer available in the remote repository.
        /// </summary>
        /// <returns>A list of deprecated <c>CheckAndFixItem</c>s.</returns>
        internal virtual List<CheckAndFixItem> GetDeprecatedCheckAndFixItems()
        {
            var availableChecks = GetAvailableCheckAndFixItems();
            var installedChecks = GetInstalledCheckAndFixItems();

            return GetDeprecatedCheckAndFixItems(availableChecks, installedChecks);
        }

        /// <summary>
        /// This method determines which <c>CheckAndFixItem</c>s have been installed, but are no longer available in the remote repository.
        /// </summary>
        /// <param name="availableChecks">A list of available <c>CheckAndFixItem</c>s.</param>
        /// <param name="installedChecks">A list of installed <c>CheckAndFixItem</c>s.</param>
        /// <returns>A list of deprecated <c>CheckAndFixItem</c>s.</returns>
        private List<CheckAndFixItem> GetDeprecatedCheckAndFixItems(List<CheckAndFixItem> availableChecks, List<CheckAndFixItem> installedChecks)
        {
            var availableCheckIds = from availableCheck in availableChecks
                                    select new
                                    {
                                        availableCheck.Id
                                    };
            var deprecated = installedChecks.Where(installedCheck =>
            {
                var installedCheckId = new { installedCheck.Id };
                return !availableCheckIds.Contains(installedCheckId);
            }).ToList();
            return deprecated;
        }

        /// <summary>
        /// This method locally installs a <c>CheckAndFixItem</c> from a remote repository.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to be installed.</param>
        internal virtual void InstallCheckAndFixItem(CheckAndFixItem item)
        {
            var filename = String.IsNullOrWhiteSpace(item.FileName) ? GetCheckAndFixItemFilename(item) : item.FileName;
            _installedChecksRepository.AddCheckAndFixItem(filename, item);
        }

        /// <summary>
        /// This method saves a new, or modified, locally-developed <c>CheckAndFixItem</c>.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to save locally.</param>
        public virtual void SaveCheckAndFixItem(CheckAndFixItem item)
        {
            var filename = GetCheckAndFixItemFilename(item);

            // Remove previous versions of the item before saving a new one.
            foreach (var check in GetSavedCheckAndFixItems()
                .Where(check => check.Id == item.Id).ToList())
                DeleteCheckAndFixItem(check);
            _localRepository.AddCheckAndFixItem(filename, item);
        }

        /// <summary>
        /// This method uninstalls a <c>CheckAndFixItem</c> that was installed from a remote repository.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to uninstall.</param>
        internal virtual void UninstallCheckAndFixItem(CheckAndFixItem item)
        {
            var filename = GetCheckAndFixItemFilename(item);
            _installedChecksRepository.RemoveCheckAndFixItem(filename);
        }

        /// <summary>
        /// This method deletes a locally-developed <c>CheckAndFixItem</c>.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to delete locally.</param>
        public virtual void DeleteCheckAndFixItem(CheckAndFixItem item)
        {
            var filename = GetCheckAndFixItemFilename(item);
            _localRepository.RemoveCheckAndFixItem(filename);
        }

        /// <summary>
        /// This method publishes a locally-developed <c>CheckAndFixItem</c> to a remote repository.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to publish to the remote repository.</param>
        public void PublishCheckAndFixItem(CheckAndFixItem item)
        {
            var filename = GetCheckAndFixItemFilename(item);
            _remoteRepository.AddCheckAndFixItem(filename, item);
        }

        /// <summary>
        /// This method removes a <c>CheckAndFixItem</c> from the remote repository and uninstalls it locally.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to unpublish and uninstall.</param>
        public void UnpublishAndUninstallCheckAndFixItem(CheckAndFixItem item)
        {
            // TODO: Add check for failed S3 removal
            string fileName = GetCheckAndFixItemFilename(item);
            _remoteRepository.RemoveCheckAndFixItem(fileName);
            _installedChecksRepository.RemoveCheckAndFixItem(fileName);
        }

        /// <summary>
        /// This method gets the <c>CheckAndFixItem</c>s available in the remote repository.
        /// </summary>
        /// <returns>The <c>CheckAndFixItem</c>s which are available in the remote repository.</returns>
        public virtual List<CheckAndFixItem> GetAvailableCheckAndFixItems()
        {
            return _remoteRepository.GetCheckAndFixItems();
        }

        /// <summary>
        /// This method returns a list of <c>CheckAndFixItem</c>s that have been installed from a remote repository.
        /// </summary>
        /// <returns>A list of saved <c>CheckAndFixItem</c>s.</returns>
        public virtual List<CheckAndFixItem> GetInstalledCheckAndFixItems()
        {
            return _installedChecksRepository.GetCheckAndFixItems();
        }

        /// <summary>
        /// This method returns a list of locally-developed and saved <c>CheckAndFixItem</c>s.
        /// </summary>
        /// <returns>A list of saved <c>CheckAndFixItem</c>s.</returns>
        public virtual List<CheckAndFixItem> GetSavedCheckAndFixItems()
        {
            return _localRepository.GetCheckAndFixItems();
        }

        /// <summary>
        /// This method find the <c>CheckAndFixItem</c>s that have an updated version available in the remote repository.
        /// </summary>
        /// <returns>A dictionary comprising of {KEY: the current <c>CheckAndFixItem</c>} and {VALUE: the updated <c>CheckAndFixItem</c> available in the remote repository}.</returns>
        internal virtual Dictionary<CheckAndFixItem, CheckAndFixItem> GetOutdatedCheckAndFixItems()
        {
            var availableChecks = GetAvailableCheckAndFixItems();
            var installedChecks = GetInstalledCheckAndFixItems();

            return GetOutdatedCheckAndFixItems(availableChecks, installedChecks);
        }

        /// <summary>
        /// This method find the <c>CheckAndFixItem</c>s that have an updated version available in the remote repository.
        /// </summary>
        /// <param name="availableChecks">A list of available <c>CheckAndFixItem</c>s.</param>
        /// <param name="installedChecks">A list of installed <c>CheckAndFixItem</c>s.</param>
        /// <returns>A dictionary comprising of {KEY: the current <c>CheckAndFixItem</c>} and {VALUE: the updated <c>CheckAndFixItem</c> available in the remote repository}.</returns>
        internal virtual Dictionary<CheckAndFixItem, CheckAndFixItem> GetOutdatedCheckAndFixItems(
            List<CheckAndFixItem> availableChecks, List<CheckAndFixItem> installedChecks)
        {
            var availableChecksSorted = availableChecks.ToList();
            availableChecksSorted.Sort((x, y) =>
                new Version(y.Version).CompareTo(new Version(x.Version)));
            var outdatedCheckAndFixItems = new Dictionary<CheckAndFixItem, CheckAndFixItem>();

            installedChecks.ForEach(installedCheck =>
            {
                var availableUpdate = availableChecksSorted.Find(availableCheck =>
                    IsNewVersion(installedCheck, availableCheck));
                if (availableUpdate is { }) outdatedCheckAndFixItems.Add(installedCheck, availableUpdate);
            });

            return outdatedCheckAndFixItems;
        }

        /// <summary>
        /// This method creates a filename for the provided <c>CheckAndFixItem</c>. 
        /// </summary>
        /// <param name="id">The <c>CheckAndFixItem</c> guid</param>
        /// <param name="version">The <c>CheckAndFixItem</c> version</param>
        /// <param name="name">The <c>CheckAndFixItem</c> name</param>
        /// <returns>The filename produced for the provided <c>CheckAndFixItem</c>.</returns>
        private string GetCheckAndFixItemFilename(string id, string version, string name)
        {
            return $"{name.ConvertToFileName()}_{version.Trim()}_{id}.{MainConsts.CHECK_FILE_EXTENSION}";
        }

        /// <summary>
        /// This method creates a filename for the provided <c>CheckAndFixItem</c>.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> for which to produce a filename.</param>
        /// <returns>The filename produced for the provided <c>CheckAndFixItem</c>.</returns>
        public string GetCheckAndFixItemFilename(CheckAndFixItem item)
        {
            string fileName;
            if (String.IsNullOrWhiteSpace(item.FileName))
            {
                fileName = GetCheckAndFixItemFilename(item.Id, item.Version, item.Name);
            }
            else
            {
                fileName = item.FileName;
            }

            return fileName;
        }

        /// <summary>
        /// This method compares two checks and determines whether the candidate is an updated version of the original.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="candidate"></param>
        /// <returns>True if this candidate is greater than the original</returns>
        internal virtual bool IsNewVersion(CheckAndFixItem original, CheckAndFixItem candidate)
        {
            return string.Equals(candidate.Id, original.Id) &&
                   Version.Parse(candidate.Version) > Version.Parse(original.Version);
        }

        /// <summary>
        /// Get the local check folder path as a string for the editor to open files there.
        /// </summary>
        /// <returns>The local check folder path as a string for the editor to open files there</returns>
        public string GetLocalRepoDirectory()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), MainConsts.LOCAL_CHECK_FOLDER_NAME);
        }

        /// <summary>
        /// Get the path to the folder where remote checks are installed.
        /// </summary>
        /// <returns>The path to the folder where remote checks are installed.</returns>
        public string GetInstalledChecksDirectory()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), MainConsts.INSTALLED_CHECK_FOLDER_NAME);
        }
    }
}