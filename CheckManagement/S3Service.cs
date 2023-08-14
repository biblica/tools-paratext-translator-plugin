/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using SIL.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TVPMain.Util;

namespace TvpMain.CheckManagement
{
    /// <summary>
    /// This class communicates with an S3-based remote repository.
    /// </summary>
    public class S3Service : IRemoteService
    {
        // AWS configuration parameters.
        private string _accessKey;
        private string _secretKey;
        private RegionEndpoint _region;
        public virtual string BucketName { get; set; }

        /// <summary>
        /// The client used to communicate with S3.
        /// </summary>
        public virtual AmazonS3Client S3Client { get; set; }

        public S3Service(string accessKey, string secretKey, string region, string bucket)
        {
            _accessKey = accessKey;
            _secretKey = secretKey;
            _region = RegionEndpoint.GetBySystemName(region) ?? RegionEndpoint.USEast1;
            BucketName = bucket;
            S3Client = new AmazonS3Client(_accessKey, _secretKey, _region);
        }

        public List<string> ListAllFiles()
        {
            List<string> checkFileNames = new List<string>();
            ListObjectsV2Request request = new ListObjectsV2Request
            {
                BucketName = BucketName,
                MaxKeys = 10
            };
            ListObjectsV2Response response;
            do
            {
                response = S3Client.ListObjectsV2(request);

                // Process the response.
                foreach (S3Object entry in response.S3Objects)
                {
                    checkFileNames.Add(entry.Key);
                }
                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);

            return checkFileNames;
        }

        public Stream GetFileStream(string file)
        {
            GetObjectRequest getObjectRequest = new GetObjectRequest
            {
                BucketName = BucketName,
                Key = file
            };

            GetObjectResponse getObjectResponse = S3Client.GetObject(getObjectRequest);

            return getObjectResponse.ResponseStream;
        }

        public HttpStatusCode PutFileStream(string filename, Stream file)
        {
            PutObjectRequest putObjectRequest = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = filename,
                InputStream = file
            };

            PutObjectResponse putObjectResponse = S3Client.PutObject(putObjectRequest);

            return putObjectResponse.HttpStatusCode;
        }

        public async Task<HttpStatusCode> PutFileStreamAsync(string filename, Stream file)
        {
            PutObjectRequest putObjectRequest = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = filename,
                InputStream = file
            };

            PutObjectResponse putObjectResponse = await S3Client.PutObjectAsync(putObjectRequest);

            return putObjectResponse.HttpStatusCode;
        }

        public HttpStatusCode DeleteFile(string filename)
        {
            DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = BucketName,
                Key = filename
            };

            DeleteObjectResponse deleteObjectResponse = S3Client.DeleteObject(deleteObjectRequest);

            return deleteObjectResponse.HttpStatusCode;
        }

        /// <summary>
        /// Tests the settings for this service to ensure that a connection can be made and that
        /// the credentials have the needed permissions.
        /// </summary>
        /// <param name="error">Error message for the last test that failed.</param>
        /// <returns>true if all tests pass without errors. false otherwise.</returns>
        public bool Verify(out string error)
        {
            var testFile = Guid.NewGuid().ToString();

            if (_region.DisplayName == "Unknown")
            {
                error = "Unknown AWS region. Please enter a valid AWS region.";
                return false;
            }

            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(testFile);
                MemoryStream stream = new MemoryStream(byteArray); 
                var putResult = PutFileStream(testFile, stream);
            }
            catch (Exception ex)
            {
                error = $"Could not write file to bucket '{BucketName}'. {ex.Message}";
                return false;
            }

            try
            {
                var listResult = ListAllFiles();
            }
            catch (Exception ex)
            {
                error = $"Could list files in bucket '{BucketName}'. {ex.Message}";
                return false;
            }

            try
            {
                var getResult = GetFileStream(testFile);
            }
            catch (Exception ex)
            {
                error = $"Could not read file from bucket '{BucketName}'. {ex.Message}";
                return false;
            }

            try
            {
                var deleteResult = DeleteFile(testFile);
            }
            catch (Exception ex)
            {
                error = $"Could not delete file from bucket '{BucketName}'. {ex.Message}";
                return false;
            }

            error = "";
            return true;
        }

        /// <summary>
        /// Validates that a string is in the correct format for an AWS access key.
        /// It does not guarantee that it is a valid key that actually exists in AWS.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        public static bool ValidateAccessKey(string key)
        {
            Regex regex = new Regex(@"^[A-Z0-9]{20}$");
            if (regex.IsMatch(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Validates that a string is in the correct format for an AWS access key.
        /// It does not guarantee that it is a valid key that actually exists in AWS.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        public static bool ValidateSecretKey(string key)
        {
            Regex regex = new Regex(@"^[A-Za-z0-9/+=]{40}$");
            if (regex.IsMatch(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Validates that a string is in the correct format for an Amazon S3 bucket name.
        /// It does not guarantee that it is a valid S3 bucket that actually exists in AWS.
        /// reference: https://docs.aws.amazon.com/AmazonS3/latest/userguide/bucketnamingrules.html
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        public static bool ValidateS3BucketName(string bucket)
        {
            Regex regex = new Regex(@"^[0-9a-z][0-9a-z.-]{1,61}[0-9a-z]$");
            if (regex.IsMatch(bucket))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Validates that a string is in the correct format for an AWS region name based
        /// on existing regions. It does not guarantee that it is the name of a valid 
        /// AWS region that actually exists in AWS.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        public static bool ValidateAwsRegion(string region)
        {
            Regex regex = new Regex(@"^[a-z]{2}[-][a-z]{1,15}[-][1-9]$");
            if (regex.IsMatch(region))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
