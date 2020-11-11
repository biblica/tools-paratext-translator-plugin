﻿using PtxUtils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TvpMain.Check;
using TvpMain.Util;

namespace TvpMain.CheckManager
{
    public class CheckManager : ICheckManager
    {
        private readonly FolderRepository folderRepository;

        private readonly S3Repository s3Repository;

        public CheckManager()
        {
            folderRepository = new FolderRepository();
            s3Repository = new S3Repository();
        }

        public List<CheckAndFixItem> GetLocalCheckAndFixItems()
        {
            return folderRepository.GetCheckAndFixItems();
        }

        public List<CheckAndFixItem> GetRemoteCheckAndFixItems()
        {
            return s3Repository.GetCheckAndFixItems();
        }

        public void InstallCheckAndFixItem(CheckAndFixItem item)
        {
            string filename = GetCheckAndFixItemFilename(item);
            folderRepository.AddCheckAndFixItem(filename, item);
        }

        public Task InstallCheckAndFixItemAsync(CheckAndFixItem item)
        {
            string filename = GetCheckAndFixItemFilename(item);
            return folderRepository.AddCheckAndFixItemAsync(filename, item);
        }

        public void PublishCheckAndFixItem(CheckAndFixItem item)
        {
            string filename = GetCheckAndFixItemFilename(item);
            s3Repository.AddCheckAndFixItem(filename, item);
        }

        public Task PublishCheckAndFixItemAsync(CheckAndFixItem item)
        {
            string filename = GetCheckAndFixItemFilename(item);
            return s3Repository.AddCheckAndFixItemAsync(filename, item);
        }

        /// <summary>
        /// This method creates a filename for the provided <c>CheckAndFixItem</c>.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> for which to produce a filename.</param>
        /// <returns>The filename produced for the provided <c>CheckAndFixItem</c>.</returns>
        private string GetCheckAndFixItemFilename(CheckAndFixItem item)
        {
            return $"{item.Name.ConvertToTitleCase().Replace(" ", String.Empty)}-{item.Version.Trim()}.{MainConsts.CHECK_FILE_EXTENSION}";
        }
    }
}