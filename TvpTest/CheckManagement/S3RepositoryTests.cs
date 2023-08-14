/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TvpMain.Check;
using TvpMain.CheckManagement;
using TVPTest;

namespace TvpTest
{
    [TestCategory("IgnoreOnBuild")]
    [TestClass()]
    public class S3RepositoryTests
    {
        readonly S3Repository s3Repository = new S3Repository(TestAWSCredentials.AWS_TVP_TEST_ACCESS_KEY_ID,
                  TestAWSCredentials.AWS_TVP_TEST_ACCESS_KEY_SECRET,
                  TestAWSCredentials.AWS_TVP_TEST_REGION,
                  TestAWSCredentials.AWS_TVP_TEST_BUCKET_NAME);
        const string filename1 = "samplecheck-1.0.0.xml";
        const string filename2 = "sampleasynccheck-1.0.0.xml";

        /// <summary>
        /// This test verifies that the <c>S3Repository</c> class can add <c>CheckAndFixItem</c>s to an S3 bucket.
        /// </summary>
        [TestMethod()]
        public void AddCheckAndFixItem()
        {
            CheckAndFixItem check = new CheckAndFixItem
            {
                Name = "A Sample Check",
                Version = "1.0.1",
                Description = "A check pushed by the AddCheckAndFixItem test",
                CheckRegex = "*.",
                CheckScript = "return null;"
            };
            s3Repository.AddCheckAndFixItem(filename1, check);

            List<CheckAndFixItem> checkAndFixItems = s3Repository.GetCheckAndFixItems();

            Assert.IsTrue(checkAndFixItems.Contains(check));
        }

        /// <summary>
        /// This test verifies that the <c>S3Repository</c> class can asynchronously add <c>CheckAndFixItem</c>s to an S3-based repository.
        /// </summary>
        [TestMethod()]
        public void AddCheckAndFixItemAsync()
        {
            CheckAndFixItem check = new CheckAndFixItem
            {
                Name = "A Sample Async Check",
                Version = "1.0.0",
                Description = "A check pushed by the AddCheckAndFixItemAsync test",
                CheckRegex = "*.",
                CheckScript = "return null;"
            };
            s3Repository.AddCheckAndFixItemAsync(filename2, check).Wait();

            List<CheckAndFixItem> checkAndFixItems = s3Repository.GetCheckAndFixItems();

            Assert.IsTrue(checkAndFixItems.Contains(check));
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            s3Repository.RemoveCheckAndFixItem(filename1);
            s3Repository.RemoveCheckAndFixItem(filename2);
        }
    }
}
