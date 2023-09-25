﻿/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using TvpMain.Check;
using TvpMain.CheckManagement;
using TvpMain.Project;
using TvpMain.Util;

namespace TvpTest
{
    [TestClass()]
    public class CheckManagerTests
    {
        Mock<CheckManager> checkManager;

        static readonly string TestOptionsManagerConfigDirectory = @"Resources\TestOptionsManagerConfig";


        [TestInitialize]
        public void Setup()
        {
            OptionsManager optionsManager = new OptionsManager(TestOptionsManagerConfigDirectory);

            checkManager = new Mock<CheckManager>(optionsManager) { CallBase = true };
            // make SetupRemoteRepository do nothing
            checkManager.Setup(cm => cm.SetupRemoteRepository()).Callback(() => { });
        }

        /// <summary>
        /// This test verifies that <c>GetOutdatedItems</c> correctly matches checks that were locally installed with their remote counterparts, and returns them if there is an updated version available.
        /// </summary>
        [TestMethod()]
        public void GetOutdatedItems_returns_a_dictionary_with_any_checks_that_match()
        {
            // Create two basic checks--one of which is an update of the other.
            string testName = "check";
            CheckAndFixItem installedItem = new CheckAndFixItem
            {
                Name = testName,
                Version = "1.0.0.0"
            };
            CheckAndFixItem availableItem = new CheckAndFixItem
            {
                Name = testName,
                Version = "1.0.0.1"
            };

            // Return the basic checks and the logic to compare them.
            checkManager.Setup(cm => cm.GetInstalledItems()).Returns(() => new List<IRunnable>
            {
                installedItem
            });
            checkManager.Setup(cm => cm.GetAvailableItems()).Returns(() => new List<IRunnable>
            {
               availableItem
            });

            // Verify that the check and its match are returned.
            checkManager.Setup(cm => cm.IsNewVersion(It.IsAny<IRunnable>(), It.IsAny<IRunnable>())).Returns(true);
            checkManager.Setup(cm => cm.GetOutdatedItems()).CallBase();
            var outdatedItems = checkManager.Object.GetOutdatedItems();

            Assert.IsTrue(outdatedItems.ContainsKey(installedItem));
            Assert.IsTrue(outdatedItems[installedItem].Equals(availableItem));
        }

        /// <summary>
        /// This test verifies that <c>GetOutdatedItems</c> does not return items that do not have an upstream match.
        /// </summary>
        [TestMethod()]
        public void GetOutdatedItems_returns_a_dictionary_excluding_checks_that_do_not_have_a_match()
        {
            // Create one basic check which will not have a match.
            string checkName1 = "check1";
            CheckAndFixItem installedCheckA = new CheckAndFixItem
            {
                Name = checkName1,
                Version = "1.0.0.0"
            };
            // Create two basic checks--one of which is an update of the other.
            string checkName2 = "check2";
            CheckAndFixItem installedCheckB = new CheckAndFixItem
            {
                Name = checkName2,
                Version = "1.0.0.0"
            };
            CheckAndFixItem availableCheckB = new CheckAndFixItem
            {
                Name = checkName2,
                Version = "1.0.0.1"
            };

            // Return the basic checks and the logic to compare them.
            checkManager.Setup(cm => cm.GetInstalledItems()).Returns(() => new List<IRunnable>
            {
                installedCheckA,
                installedCheckB
            });
            checkManager.Setup(cm => cm.GetAvailableItems()).Returns(() => new List<IRunnable>
            {
               availableCheckB
            });

            // Verify that the check with no match is not included in the dictionary.
            checkManager.Setup(cm => cm.IsNewVersion(installedCheckA, availableCheckB)).Returns(false);
            checkManager.Setup(cm => cm.IsNewVersion(installedCheckB, availableCheckB)).Returns(true);
            checkManager.Setup(cm => cm.GetOutdatedItems()).CallBase();
            var outdatedItems = checkManager.Object.GetOutdatedItems();

            Assert.IsFalse(outdatedItems.ContainsKey(installedCheckA));
            Assert.IsTrue(outdatedItems.ContainsKey(installedCheckB));
            Assert.IsTrue(outdatedItems[installedCheckB].Equals(availableCheckB));
        }

        /// <summary>
        /// This test verifies that <c>GetOutdatedItems</c> handles duplicate items and only returns the latest version.
        /// </summary>
        [TestMethod()]
        public void GetOutdatedItems_handles_duplicates()
        {
            // Create three basic checks--one of which is the latest update of another
            string testName = "check";

            CheckAndFixItem installedCheckA = new CheckAndFixItem
            {
                Name = testName,
                Version = "1.0.0.0"
            };
            CheckAndFixItem availableCheckA_Old = new CheckAndFixItem
            {
                Id = installedCheckA.Id,
                Name = testName,
                Version = "1.0.0.1"
            };
            CheckAndFixItem availableCheckA_New = new CheckAndFixItem
            {
                Id = installedCheckA.Id,
                Name = testName,
                Version = "1.0.0.2"
            };

            // Return the basic checks and the logic to compare them.
            checkManager.Setup(cm => cm.GetInstalledItems()).Returns(() => new List<IRunnable>
            {
                installedCheckA
            });
            checkManager.Setup(cm => cm.GetAvailableItems()).Returns(() => new List<IRunnable>
            {
               availableCheckA_Old,
               availableCheckA_New
            });

            // Verify that only the latest version is considered.
            checkManager.Setup(cm => cm.IsNewVersion(It.IsAny<IRunnable>(), It.IsAny<IRunnable>())).Returns(true);
            var outdatedItems = checkManager.Object.GetOutdatedItems();

            Assert.IsTrue(outdatedItems.ContainsKey(installedCheckA));
            Assert.IsTrue(outdatedItems[installedCheckA].Equals(availableCheckA_New));
        }

        /// <summary>
        /// The test verifies that <c>IsNewVersionTest</c> correctly identifes when two items are the same, with one being a newer version.
        /// </summary>
        [TestMethod()]
        public void IsNewVersionTest()
        {
            checkManager.Setup(cm => cm.IsNewVersion(It.IsAny<CheckAndFixItem>(), It.IsAny<CheckAndFixItem>())).CallBase();

            // Set up our base check that others will be compared to.
            CheckAndFixItem check = new CheckAndFixItem
            {
                Name = "test",
                Version = "1.0.0.1"
            };

            // Set up the checks to compare against the base check.
            CheckAndFixItem sameVersion = new CheckAndFixItem
            {
                Id = check.Id,
                Name = check.Name,
                Version = check.Version
            };
            CheckAndFixItem differentCheck = new CheckAndFixItem
            {
                Name = "text",
                Version = "1.0.0.1"
            };
            CheckAndFixItem oldVersion = new CheckAndFixItem
            {
                Id = check.Id,
                Name = check.Name,
                Version = "1.0.0.0"
            };
            CheckAndFixItem newVersion = new CheckAndFixItem
            {
                Id = check.Id,
                Name = check.Name,
                Version = "1.0.0.2"
            };

            // Test that only the new version of the same check will work.
            Assert.IsFalse(checkManager.Object.IsNewVersion(check, sameVersion));
            Assert.IsFalse(checkManager.Object.IsNewVersion(check, differentCheck));
            Assert.IsFalse(checkManager.Object.IsNewVersion(check, oldVersion));
            Assert.IsTrue(checkManager.Object.IsNewVersion(check, newVersion));
        }

        /// <summary>
        /// The test verifies that the <c>CheckManager</c> class can manage both installed (from remote) and saved (locally-developed) <c>CheckAndFixItem</c>s.
        /// </summary>
        [TestMethod()]
        public void It_can_manage_both_installed_and_locally_developed_checks()
        {
            CheckAndFixItem remoteCheck = new CheckAndFixItem
            {
                Name = "Remote Check",
                Version = "1.0.0.0",
                Description = "A check that has been installed from a remote source."
            };
            CheckAndFixItem locallyDevelopedCheck = new CheckAndFixItem
            {
                Name = "Local Check",
                Version = "1.0.0.0",
                Description = "A check that was developed locally."
            };

            // Expose the methods under test.
            checkManager.Setup(cm => cm.InstallItem(It.IsAny<IRunnable>())).CallBase();
            checkManager.Setup(cm => cm.UninstallItem(It.IsAny<IRunnable>())).CallBase();
            checkManager.Setup(cm => cm.SaveItem(It.IsAny<IRunnable>(), It.IsAny<string>())).CallBase();
            checkManager.Setup(cm => cm.DeleteItem(It.IsAny<IRunnable>())).CallBase();
            checkManager.Setup(cm => cm.GetInstalledItems()).CallBase();
            checkManager.Setup(cm => cm.GetItems(It.IsAny<string>())).CallBase();

            // Add the checks to the repo using their respective methods.
            checkManager.Object.InstallItem(remoteCheck);
            checkManager.Object.SaveItem(locallyDevelopedCheck, MainConsts.LOCAL_REPO_NAME);

            // Fetch the checks using their respective methods.
            List<IRunnable> installedChecks = checkManager.Object.GetInstalledItems();
            List<IRunnable> locallyDevelopedChecks = checkManager.Object.GetItems(MainConsts.LOCAL_REPO_NAME);

            // Ensure that the repos are separate and that there is no cross-pollution.
            Assert.IsTrue(installedChecks.Contains(remoteCheck));
            Assert.IsFalse(installedChecks.Contains(locallyDevelopedCheck));
            Assert.IsTrue(locallyDevelopedChecks.Contains(locallyDevelopedCheck));
            Assert.IsFalse(locallyDevelopedChecks.Contains(remoteCheck));

            // Remove the checks from their respective repos.
            checkManager.Object.UninstallItem(remoteCheck);
            checkManager.Object.DeleteItem(locallyDevelopedCheck);

            // Fetch the checks using their respective methods.
            Assert.IsTrue(checkManager.Object.GetInstalledItems().Count == 0);
            Assert.IsTrue(checkManager.Object.GetItems(MainConsts.LOCAL_REPO_NAME).Count == 0);
        }

        /// <summary>
        /// This test verifies that the <c>CheckManager</c> class can synchronize the installed items with a remote repository.
        /// </summary>
        [TestMethod()]
        public void It_can_synchronize_the_installed_checks_with_a_remote_repository()
        {
            // This check is identical and nothing will change.
            CheckAndFixItem remoteCheckAlpha = new CheckAndFixItem
            {
                Name = "Alpha",
                Version = "1.0.0.0",
                Description = "Remote Alpha"
            };

            // This check is new and will be installed.
            CheckAndFixItem remoteCheckBeta = new CheckAndFixItem
            {
                Name = "Beta",
                Version = "1.0.0.0",
                Description = "Remote Beta"
            };

            // This check is an update and will be installed.
            CheckAndFixItem remoteCheckGamma = new CheckAndFixItem
            {
                Name = "Gamma",
                Version = "1.0.0.1",
                Description = "Remote Gamma"
            };

            // This check is identical and nothing will change.
            CheckAndFixItem localCheckAlpha = new CheckAndFixItem
            {
                Id = remoteCheckAlpha.Id,
                Name = remoteCheckAlpha.Name,
                Version = "1.0.0.0",
                Description = "Local Alpha"
            };

            // This check is outdated and will be uninstalled.
            CheckAndFixItem localCheckGamma = new CheckAndFixItem
            {
                Id = remoteCheckGamma.Id,
                Name = remoteCheckGamma.Name,
                Version = "1.0.0.0",
                Description = "Local Gamma"
            };

            // This check is deprecated (no longer exists upstream) and will be uninstalled.
            CheckAndFixItem localCheckDelta = new CheckAndFixItem
            {
                Name = "Delta",
                Version = "1.0.0.0",
                Description = "Local Delta"
            };

            checkManager.Setup(cm => cm.GetAvailableItems()).Returns(() => new List<IRunnable>()
            {
                remoteCheckAlpha,
                remoteCheckBeta,
                remoteCheckGamma
            });

            // Install the checks that exist at the beginning of the test.
            checkManager.Object.InstallItem(localCheckAlpha);
            checkManager.Object.InstallItem(localCheckGamma);
            checkManager.Object.InstallItem(localCheckDelta);

            // Inspect the repository.
            List<IRunnable> installedChecks = checkManager.Object.GetInstalledItems();

            // Ensure that all checks are installed as expected.
            Assert.IsTrue(installedChecks.Count == 3);
            Assert.IsTrue(installedChecks.Contains(localCheckAlpha));
            Assert.IsTrue(installedChecks.Contains(localCheckGamma));
            Assert.IsTrue(installedChecks.Contains(localCheckDelta));

            //Synchronize the repository.
            checkManager.Object.SynchronizeInstalledChecks();

            // Re-inspect the repository.
            installedChecks = checkManager.Object.GetInstalledItems();

            // Ensure that checks have been updated and uninstalled as expected.
            Assert.AreEqual(3, installedChecks.Count);
            Assert.IsTrue(installedChecks.Contains(localCheckAlpha));
            Assert.IsTrue(installedChecks.Contains(remoteCheckBeta));
            Assert.IsTrue(installedChecks.Contains(remoteCheckGamma));
        }

        /// <summary>
        /// This test verifies that when the <c>CheckManager</c> class saves a locally-developed item, it does not leave old versions on the filesystem.
        /// </summary>
        [TestMethod()]
        public void Saving_a_check_replaces_the_existing_version()
        {
            CheckAndFixItem firstVersion = new CheckAndFixItem
            {
                Name = "Sample Test",
                Version = "1.0.0.0",
                Description = "An original description"
            };
            // Has an updated description.
            CheckAndFixItem secondVersion = new CheckAndFixItem
            {
                Id = firstVersion.Id,
                Name = firstVersion.Name,
                Version = "1.0.0.0",
                Description = "A new description"
            };
            // Has an updated version number.
            CheckAndFixItem thirdVersion = new CheckAndFixItem
            {
                Id = firstVersion.Id,
                Name = firstVersion.Name,
                Version = "1.0.0.3",
                Description = "A new description"
            };

            checkManager.Setup(cm => cm.SaveItem(It.IsAny<CheckAndFixItem>(), It.IsAny<string>())).CallBase();
            checkManager.Setup(cm => cm.GetItems(It.IsAny<string>())).CallBase();
            checkManager.Setup(cm => cm.DeleteItem(It.IsAny<CheckAndFixItem>())).CallBase();

            checkManager.Object.SaveItem(firstVersion, MainConsts.LOCAL_REPO_NAME);
            List<IRunnable> savedChecks = checkManager.Object.GetItems(MainConsts.LOCAL_REPO_NAME);
            Assert.IsTrue(savedChecks.Count == 1);
            Assert.IsTrue(savedChecks.Contains(firstVersion));

            checkManager.Object.SaveItem(secondVersion, MainConsts.LOCAL_REPO_NAME);
            savedChecks = checkManager.Object.GetItems(MainConsts.LOCAL_REPO_NAME);
            Assert.IsTrue(savedChecks.Count == 1);
            Assert.IsTrue(savedChecks.Contains(secondVersion));

            checkManager.Object.SaveItem(thirdVersion, MainConsts.LOCAL_REPO_NAME);
            savedChecks = checkManager.Object.GetItems(MainConsts.LOCAL_REPO_NAME);
            Assert.IsTrue(savedChecks.Count == 1);
            Assert.IsTrue(savedChecks.Contains(thirdVersion));
        }

        /// <summary>
        /// Remove any <c>CheckAndFixItem</c> directories that may have been created as a part of this test.
        /// </summary>
        [TestCleanup()]
        public void TestCleanup()
        {
            string installedCheckRoot = Path.Combine(Directory.GetCurrentDirectory(), MainConsts.INSTALLED_CHECK_FOLDER_NAME.Split('\\')[0]);
            if (Directory.Exists(installedCheckRoot)) Directory.Delete(installedCheckRoot, true);

            string localCheckRoot = Path.Combine(Directory.GetCurrentDirectory(), MainConsts.INSTALLED_CHECK_FOLDER_NAME.Split('\\')[0]);
            if (Directory.Exists(localCheckRoot)) Directory.Delete(localCheckRoot, true);
        }
    }
}
