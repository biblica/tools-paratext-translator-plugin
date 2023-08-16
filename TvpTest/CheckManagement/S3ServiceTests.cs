/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Amazon;
using Amazon.S3;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using TvpMain.Check;
using TvpMain.CheckManagement;
using TVPTest;

namespace TvpTest
{
    [TestCategory("IgnoreOnBuild")]
    [TestClass()]
    public class S3ServiceTests
    {
        const string filename = "test.xml";

        S3Service Service { get; set; } = new S3Service(
            TestAWSCredentials.AWS_TVP_TEST_ACCESS_KEY_ID,
            TestAWSCredentials.AWS_TVP_TEST_ACCESS_KEY_SECRET,
            TestAWSCredentials.AWS_TVP_TEST_REGION,
            TestAWSCredentials.AWS_TVP_TEST_BUCKET_NAME);

        [TestInitialize()]
        public void TestSetup()
        {
            CheckAndFixItem checkAndFix = new CheckAndFixItem
            {
                Name = "test",
                Version = "1.2.3.4",
                CheckRegex = "*.",
                CheckScript = "return null;"
            };
            Service.PutFileStream(filename, checkAndFix.WriteToXmlStream());
        }

        /// <summary>
        /// This test verifies that the <c>S3Service</c> class can add <c>CheckAndFixItem</c>s to an S3 bucket.
        /// </summary>
        [TestMethod()]
        public void PutFileStream()
        {
            CheckAndFixItem checkAndFix = new CheckAndFixItem
            {
                Name = "test",
                Version = "1.2.3.5",
                CheckRegex = "*.",
                CheckScript = "return null;"
            };
            Service.PutFileStream(filename, checkAndFix.WriteToXmlStream());

            var files = Service.ListAllFiles();
            Assert.IsTrue(files.Contains(filename));
        }

        /// <summary>
        /// This test verifies that the <c>S3Service</c> class read a list of files from an S3 bucket.
        /// </summary>
        [TestMethod()]
        public void ListFilesTest()
        {
            var files = Service.ListAllFiles();
            Assert.IsTrue(files.Contains(filename));
        }

        /// <summary>
        /// This test verifies that the <c>S3Service</c> class can get a <c>CheckAndFixItem</c>s from an S3 bucket.
        /// </summary>
        [TestMethod()]
        public void GetFileTest()
        {
            using Stream file = Service.GetFileStream(filename);
            Assert.IsTrue(CheckAndFixItem.LoadFromXmlContent(file) is CheckAndFixItem);
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            Service.DeleteFile(filename);
        }
    }
}
