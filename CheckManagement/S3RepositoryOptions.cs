using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvpMain.CheckManagement
{
    /// <summary>
    /// Represents all the user settings necessary to setup a remote repository 
    /// where files are stored in an Amazon S3 bucket.
    /// </summary>
    public class S3RepositoryOptions
    {
        /// <summary>
        /// A unique identifier for the repository.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// A display name for the repository.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Determines if the repository is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Have the repository settings been tested and verified to work?
        /// </summary>
        public bool Verified { get; set; }

        /// <summary>
        /// The AWS region where the Amazon S3 bucket is located.
        /// </summary>
        public string AwsRegion { get; set; }

        /// <summary>
        /// The name of the Amazon S3 bucket where files will be stored.
        /// </summary>
        public string S3Bucket { get; set; }

        /// <summary>
        /// The AWS API access key to use when accessing the S3 bucket. 
        /// </summary>
        public string AwsAccessKey { get; set; }

        /// <summary>
        /// The AWS API secret key to use when accessing the S3 bucket. 
        /// </summary>
        public string AwsSecretKey { get; set; }

        /// <summary>
        /// Determines if the repository will be synchronized when the TVP program is started.
        /// </summary>
        public bool SyncOnStartup { get; set; }
    }
}
