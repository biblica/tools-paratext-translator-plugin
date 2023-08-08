using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvpMain.CheckManagement;

namespace TvpMain.Project
{
    /// <summary>
    /// User configured settings for the TVP program.
    /// </summary>
    public class TvpOptions
    {
        /// <summary>
        /// Settings for the remote repository used for sharing checks among TVP users.
        /// </summary>
        public S3RepositoryOptions SharedRepo { get; set; }

        public TvpOptions()
        {
            SharedRepo = new S3RepositoryOptions();
        }
    }
}
