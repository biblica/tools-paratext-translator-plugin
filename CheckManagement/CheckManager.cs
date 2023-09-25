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
        private Dictionary<string, IRepository> _repositories = new Dictionary<string, IRepository>();
        private readonly IRepository _localRepository;
        private IRepository _remoteRepository = null;
        /// <summary>
        /// The names of all enabled repositories. 
        /// </summary>
        public string[] Repositories
        {
            get
            {
                return _repositories.Where(repo => repo.Value.Enabled).Select(repo => repo.Key).ToArray();
            }
        }
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
            _repositories.Add(_localRepository.Name, _localRepository);
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
                _repositories.Add(_remoteRepository.Name, _remoteRepository);
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
        /// This method synchronizes installed runnable items with the remote repository.
        /// </summary>
        /// <param name="dryRun">(optional) If true, returns the result of the operation without applying it.</param>
        /// <returns>A key/value pair mapping of the result, with items grouped by enumerated values.</returns>
        public virtual Dictionary<SynchronizationResultType, List<IRunnable>> SynchronizeInstalledChecks(
            bool dryRun = false)
        {
            var availableItems = GetAvailableItems();
            var installedItems = GetInstalledItems();

            var newItems = GetNewItems(availableItems, installedItems);
            var outdatedItems = GetOutdatedItems(availableItems, installedItems);
            var deprecatedItems = GetDeprecatedItems(availableItems, installedItems);

            if (dryRun)
            {
                return new Dictionary<SynchronizationResultType, List<IRunnable>>
                {
                    [SynchronizationResultType.New] = newItems,
                    [SynchronizationResultType.Outdated] = outdatedItems.Keys.ToList(),
                    [SynchronizationResultType.Updated] = outdatedItems.Values.ToList(),
                    [SynchronizationResultType.Deprecated] = deprecatedItems
                };
            }

            foreach (var check in deprecatedItems)
            {
                UninstallItem(check);
            }

            // Handle any situation where a newer check or group was removed from remote, but an older version still exists.
            newItems = GetNewItems(availableItems, installedItems);

            foreach (var item in newItems)
            {
                InstallItem(item);
            }

            foreach (var item in outdatedItems.Keys)
            {
                UninstallItem(item);
            }

            foreach (var item in outdatedItems.Values)
            {
                InstallItem(item);
            }

            return new Dictionary<SynchronizationResultType, List<IRunnable>>
            {
                [SynchronizationResultType.New] = newItems,
                [SynchronizationResultType.Outdated] = outdatedItems.Keys.ToList(),
                [SynchronizationResultType.Updated] = outdatedItems.Values.ToList(),
                [SynchronizationResultType.Deprecated] = deprecatedItems
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
        /// This method retrieves any checks or groups which exist in the remote repository, 
        /// but for which no version has been installed.
        /// </summary>
        /// <returns>A list of new items which can be installed.</returns>
        internal virtual List<IRunnable> GetNewItems()
        {
            var availableItems = GetAvailableItems();
            var installedItems = GetInstalledItems();

            return GetNewItems(availableItems, installedItems);
        }

        /// <summary>
        /// This method retrieves any runnable items which exist in the remote repository,
        /// but for which no version has been installed.
        /// </summary>
        /// <param name="availableItems">A list of available items.</param>
        /// <param name="installedItems">A list of installed items.</param>
        /// <returns>A list of new items which can be installed.</returns>
        private List<IRunnable> GetNewItems(List<IRunnable> availableItems, List<IRunnable> installedItems)
        {
            var installedItemIds = from installedCheck in installedItems
                                    select new
                                    {
                                        installedCheck.Id
                                    };
            var availableItemsSorted = from availableItem in availableItems
                                        orderby availableItem.Id ascending, availableItem.Version descending
                                        select availableItem;

            var newItems = new List<IRunnable>();
            foreach (var availableItem in availableItemsSorted )
            {
                var val1 = installedItemIds.Contains(new { availableItem.Id });
                if (!installedItemIds.Contains(new { availableItem.Id }) && 
                    !newItems.Exists(newCheck => newCheck.Id == availableItem.Id))
                {
                    newItems.Add(availableItem);
                }
            }

            return newItems;
        }

        /// <summary>
        /// This method determines which items have been installed, but are no longer available in the remote repository.
        /// </summary>
        /// <returns>A list of deprecated items.</returns>
        internal virtual List<IRunnable> GetDeprecatedItems()
        {
            var availableItems = GetAvailableItems();
            var installedItems = GetInstalledItems();

            return GetDeprecatedItems(availableItems, installedItems);
        }

        /// <summary>
        /// This method determines which items have been installed, but are no longer available in the remote repository.
        /// </summary>
        /// <param name="availableItems">A list of available items.</param>
        /// <param name="installedItems">A list of installed items.</param>
        /// <returns>A list of deprecated items.</returns>
        private List<IRunnable> GetDeprecatedItems(List<IRunnable> availableItems, List<IRunnable> installedItems)
        {
            var availableItemIds = from availableItem in availableItems
                                    select new
                                    {
                                        availableItem.Id
                                    };
            var deprecatedItems = installedItems.Where(installedItem =>
            {
                var installedItemId = new { installedItem.Id };
                return !availableItemIds.Contains(installedItemId);
            }).ToList();

            return deprecatedItems;
        }

        /// <summary>
        /// This method locally installs an item from a remote repository.
        /// </summary>
        /// <param name="item">The item to be installed.</param>
        internal virtual void InstallItem(IRunnable item)
        {
            var filename = String.IsNullOrWhiteSpace(item.FileName) ? GetItemFilename(item) : item.FileName;
            _installedChecksRepository.AddItem(filename, item);
        }

        /// <summary>
        /// Saves an item to the specified repository.
        /// </summary>
        /// <param name="item">The item to save.</param>
        /// <param name="repositoryName">Name of the repository to save to.</param>
        /// <returns>true if successful, false otherwise</returns>
        public virtual bool SaveItem(IRunnable item, string repositoryName)
        {
            if (!_repositories.ContainsKey(repositoryName)) return false;

            IRepository repository = _repositories[repositoryName];
            List<IRunnable> repositoryItems = repository.GetItems();
            try
            {
                // Remove previous versions of the item.
                foreach (var repositoryItem in repositoryItems.Where(i => i.Id == item.Id).ToList())
                {
                    string removeFileName = GetItemFilename(repositoryItem);
                    repository.RemoveItem(removeFileName);
                }
                string saveFilename = GetItemFilename(item);
                repository.AddItem(saveFilename, item);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// This method uninstalls a item that was installed from a remote repository.
        /// </summary>
        /// <param name="item">The item to uninstall.</param>
        internal virtual void UninstallItem(IRunnable item)
        {
            var filename = GetItemFilename(item);
            _installedChecksRepository.RemoveItem(filename);
        }

        /// <summary>
        /// This method deletes a locally-developed check or group.
        /// </summary>
        /// <param name="item">The check or group to delete locally.</param>
        public virtual void DeleteItem(IRunnable item)
        {
            var filename = GetItemFilename(item);
            _localRepository.RemoveItem(filename);
        }

        /// <summary>
        /// This method publishes a locally developed item to the remote repository.
        /// </summary>
        /// <param name="item">The item to publish to the remote repository.</param>
        /// <returns>true if the item was published. false if the publish failed.</returns>
        public bool PublishItem(IRunnable item)
        {
            var filename = GetItemFilename(item, true);
            try
            {
                _remoteRepository.AddItem(filename, item);
            }
            catch (Exception)
            {
                return false;
            }
            // Remove previous versions of the item before saving a new one.
            foreach (var installedItem in GetInstalledItems().Where(thisItem => thisItem.Id == item.Id).ToList())
            {
                _installedChecksRepository.RemoveItem(GetItemFilename(installedItem));
            }
            _installedChecksRepository.AddItem(filename, item);

            return true;
        }

        /// <summary>
        /// This method removes a item from the remote repository and uninstalls it locally.
        /// </summary>
        /// <param name="item">The item to unpublish and uninstall.</param>
        public void UnpublishAndUninstallItem(IRunnable item)
        {
            // TODO: Add check for failed S3 removal
            string fileName = GetItemFilename(item);
            _remoteRepository.RemoveItem(fileName);
            _installedChecksRepository.RemoveItem(fileName);
        }

        /// <summary>
        /// This method gets the items available in the remote repository.
        /// </summary>
        /// <returns>The items which are available in the remote repository.</returns>
        public virtual List<IRunnable> GetAvailableItems()
        {
            return _remoteRepository.GetItems();
        }

        /// <summary>
        /// This method returns a list of checks that have been installed from a remote repository.
        /// </summary>
        /// <returns>A list of installed checks.</returns>
        public virtual List<CheckAndFixItem> GetInstalledChecks()
        {
            return _installedChecksRepository.GetChecks();
        }

        /// <summary>
        /// Returns a list of checks stored in the specified repository.
        /// </summary>
        /// <param name="repositoryName">The name of the repository.</param>
        /// <returns></returns>
        public virtual List<CheckAndFixItem> GetChecks(string repositoryName)
        {
            if (_repositories.ContainsKey(repositoryName))
            {
                return _repositories[repositoryName].GetChecks();
            }
            else
            {
                return new List<CheckAndFixItem>();
            }
        }

        /// <summary>
        /// Returns a list of runnable items (both checks and groups) stored 
        /// in the specified repository.
        /// </summary>
        /// <param name="repositoryName">The name of the repository.</param>
        /// <returns></returns>
        public virtual List<IRunnable> GetItems(string repositoryName)
        {
            if (_repositories.ContainsKey(repositoryName))
            {
                return _repositories[repositoryName].GetItems();
            }
            else
            {
                return new List<IRunnable>();
            }
        }

        /// <summary>
        /// This method returns a list of runnable items that have been installed 
        /// from a remote repository.
        /// </summary>
        /// <returns>A list of installed items.</returns>
        public virtual List<IRunnable> GetInstalledItems()
        {
            return _installedChecksRepository.GetItems();
        }

        /// <summary>
        /// This method find the items that have an updated version available in the remote repository.
        /// </summary>
        /// <returns>A dictionary comprising of {KEY: the current item} and {VALUE: the updated item available in the remote repository}.</returns>
        internal virtual Dictionary<IRunnable, IRunnable> GetOutdatedItems()
        {
            var availableItems = GetAvailableItems();
            var installedItems = GetInstalledItems();

            return GetOutdatedItems(availableItems, installedItems);
        }

        /// <summary>
        /// This method find the items that have an updated version available in the remote repository.
        /// </summary>
        /// <param name="availableItems">A list of available items.</param>
        /// <param name="installedItems">A list of installed items.</param>
        /// <returns>A dictionary comprising of {KEY: the current item} and {VALUE: the updated item available in the remote repository}.</returns>
        internal virtual Dictionary<IRunnable, IRunnable> GetOutdatedItems(
            List<IRunnable> availableItems, List<IRunnable> installedItems)
        {
            var availableItemsSorted = availableItems.ToList();
            availableItemsSorted.Sort((x, y) =>
                new Version(y.Version).CompareTo(new Version(x.Version)));
            var outdatedItems = new Dictionary<IRunnable, IRunnable>();

            installedItems.ForEach(installedItem =>
            {
                var availableUpdate = availableItemsSorted.Find(availableItem =>
                    IsNewVersion(installedItem, availableItem));
                if (availableUpdate is { }) outdatedItems.Add(installedItem, availableUpdate);
            });

            return outdatedItems;
        }

        /// <summary>
        /// This method returns the filename that the item was loaded from or creates 
        /// a new filename if the old file name is unknown.
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="ignoreExistingFilename">Always create a new filename.</param>
        /// <returns>The filename produced for the provided item.</returns>
        public string GetItemFilename(IRunnable item, bool ignoreExistingFilename = false)
        {
            string fileName;
            if (String.IsNullOrWhiteSpace(item.FileName) || ignoreExistingFilename)
            {
                string extension = "xml";
                extension = item is CheckAndFixItem ? CheckAndFixItem.FileExtension : extension;
                extension = item is CheckGroup ? CheckGroup.FileExtension : extension;
                string id = item.Id;
                string name = item.Name.ConvertToFileName();
                string version = item.Version.Trim();
                fileName = $"{name}_{version}_{id}.{extension}";
            }
            else
            {
                fileName = item.FileName;
            }

            return fileName;
        }

        /// <summary>
        /// This method compares two items and determines whether the candidate is an updated version of the original.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="candidate"></param>
        /// <returns>true if this item is greater than the original</returns>
        internal virtual bool IsNewVersion(IRunnable original, IRunnable candidate)
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