/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using PtxUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TvpMain.Check;

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

        public LocalRepository(string folderPath)
        {
            FolderPath = folderPath;
        }

        public void AddCheckAndFixItem(string filename, CheckAndFixItem item)
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

        public Task AddCheckAndFixItemAsync(string filename, CheckAndFixItem item)
        {
            return Task.Run(() => AddCheckAndFixItem(filename, item));
        }

        public void RemoveCheckAndFixItem(string filename)
        {
            var filePath = Path.Combine(FolderPath, filename);
            if (!File.Exists(filePath)) return;

            File.Delete(filePath);
        }

        public List<CheckAndFixItem> GetCheckAndFixItems()
        {
            var checkAndFixItems = new List<CheckAndFixItem>();
            VerifyFolderPath();

            var checkFiles = Directory.GetFiles(FolderPath, "*.xml");
            foreach (var checkFilePath in checkFiles)
            {
                try
                {
                    var checkAndFixItem = CheckAndFixItem.LoadFromXmlFile(checkFilePath);
                    checkAndFixItem.FileName = Path.GetFileName(checkFilePath);
                    checkAndFixItems.Add(checkAndFixItem);
                }
                catch (Exception e)
                {
                    throw new FileLoadException($"Unable to load '{checkFilePath}'.", e.InnerException);
                }
            }

            return checkAndFixItems;
        }

        /// <summary>
        /// This method ensures that the folder exists.
        /// </summary>
        private void VerifyFolderPath()
        {
            Directory.CreateDirectory(FolderPath);
        }
    }
}
