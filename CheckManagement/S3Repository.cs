/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TvpMain.Check;

namespace TvpMain.CheckManagement
{
    /// <summary>
    /// This class works with a remote, S3-based repository for checks and fixes.
    /// </summary>
    public class S3Repository : IRepository
    {
        /// <summary>
        /// The service which interacts with S3.
        /// </summary>
        protected virtual IRemoteService Service { get; } = S3ServiceProvider.Instance;

        public void AddCheckAndFixItem(string filename, CheckAndFixItem item)
        {
            if (String.IsNullOrEmpty(filename)) new ArgumentNullException(nameof(filename));

            Service.PutFileStream(filename, item.WriteToXmlStream());
        }

        public async Task AddCheckAndFixItemAsync(string filename, CheckAndFixItem item)
        {
            if (String.IsNullOrEmpty(filename)) new ArgumentNullException(nameof(filename));

            await Service.PutFileStreamAsync(filename, item.WriteToXmlStream());
        }

        public List<CheckAndFixItem> GetCheckAndFixItems()
        {
            var checkAndFixItems = new List<CheckAndFixItem>();

            // Whether the string represents an XML filename.
            // This is a simple way to guard against non-check files in the repository.
            static bool IsXmlFile(string filename) => filename.Trim().ToLowerInvariant().EndsWith(".xml");
            
            var filenames = Service.ListAllFiles().Where((Func<string,bool>) IsXmlFile).ToList();
            foreach (var file in filenames)
            {
                using var fileStream = Service.GetFileStream(file);
                var checkAndFixItem = ReadCheckAndFixItemFromStream(fileStream);
                checkAndFixItem.FileName = Path.GetFileName(file);
                if (checkAndFixItem != null) checkAndFixItems.Add(checkAndFixItem);
            }

            return checkAndFixItems;
        }

        /// <summary>
        /// This loads a <c>CheckAndFixItem</c> from a <c>Stream</c>, guarding against invalid files.
        /// </summary>
        /// <param name="stream">The <c>Stream</c> of a file representing a <c>CheckAndFixItem</c>.</param>
        /// <returns></returns>
        private CheckAndFixItem ReadCheckAndFixItemFromStream(Stream stream)
        {
            CheckAndFixItem checkAndFixItem = null;
            checkAndFixItem = CheckAndFixItem.LoadFromXmlContent(stream);

            return checkAndFixItem;
        }

        public void RemoveCheckAndFixItem(string filename)
        {
            Service.DeleteFile(filename);
        }
    }
}
