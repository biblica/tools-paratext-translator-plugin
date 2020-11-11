﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TvpMain.Check;

namespace TvpMain.CheckManager
{
    interface IRepository
    {
        /// <summary>
        /// This method gets check and fix items from a repository.
        /// </summary>
        /// <returns>A list of check and fix items that are available in the repository.</returns>
        public List<CheckAndFixItem> GetCheckAndFixItems();

        /// <summary>
        /// This method gets check and fix items from a repository.
        /// </summary>
        /// <returns>A task representing a list of check and fix items that are available in the repository.</returns>
        public Task<List<CheckAndFixItem>> GetCheckAndFixItemsAsync();

        /// <summary>
        /// This method adds a check and fix item to a repository.
        /// </summary>
        /// <param name="filename">The filename to use for the item in the repository.</param>
        /// <param name="item">The check and fix item to add to the repository.</param>
        public void AddCheckAndFixItem(string filename, CheckAndFixItem item);

        /// <summary>
        /// This method adds a check and fix item to a repository.
        /// </summary>
        /// <param name="filename">The filename to use for the item in the repository.</param>
        /// <param name="item">The check and fix item to add to the repository.</param>
        /// <returns>A task representing the result of the operation.</returns>
        public Task AddCheckAndFixItemAsync(string filename, CheckAndFixItem item);
    }
}