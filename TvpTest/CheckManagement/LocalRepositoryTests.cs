/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using TvpMain.Check;
using TvpMain.CheckManagement;

namespace TvpTest
{
    [TestClass()]
    public class LocalRepositoryTests
    {
        const string filename = "test.xml";
        private readonly string folderPath = Path.Combine(Path.GetTempPath(), "checks");

        private LocalRepository LocalRepository { get; set; }

        [TestInitialize()]
        public void TestSetup()
        {
            LocalRepository = new LocalRepository(folderPath);
        }

        /// <summary>
        /// This test verifies that the <c>LocalRepository</c> class can both add and remove items from a local repository.
        /// </summary>
        [TestMethod()]
        public void It_can_add_and_remove_checks()
        {
            CheckAndFixItem checkAndFixItem = new CheckAndFixItem
            {
                Version = "1.2.3.4"
            };

            //Throws an exception if Name is null.
            Assert.ThrowsException<ArgumentNullException>(() => LocalRepository.AddItem(null, checkAndFixItem));

            CheckAndFixItem checkAndFixItem2 = new CheckAndFixItem
            {
                Name = "Test Check",
                Version = "1.2.3.4",
                Description = "A test check",
                CheckRegex = "*.",
                CheckScript = "return null;"
            };

            LocalRepository.AddItem(filename, checkAndFixItem2);

            List<IRunnable> checkAndFixItems = LocalRepository.GetItems();
            Assert.IsTrue(checkAndFixItems.Count == 1);
            Assert.IsTrue(checkAndFixItems[0].Version == "1.2.3.4");
            LocalRepository.RemoveItem(filename);
            Assert.IsTrue(LocalRepository.GetItems().Count == 0);
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            Directory.Delete(folderPath);
        }
    }
}
