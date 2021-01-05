﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TvpMain.Check;

namespace TvpMain.CheckManagement
{
    /// <summary>
    /// This interface defines a class that manages a repository of checks.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// This method gets <c>CheckAndFixItem</c>s from a repository.
        /// </summary>
        /// <returns>A list of check and fix items that are available in the repository.</returns>
        public List<CheckAndFixItem> GetCheckAndFixItems();

        /// <summary>
        /// This method asynchronously gets <c>CheckAndFixItem</c>s from a repository.
        /// </summary>
        /// <returns>A task representing a list of check and fix items that are available in the repository.</returns>
        public Task<List<CheckAndFixItem>> GetCheckAndFixItemsAsync();

        /// <summary>
        /// This method adds a <c>CheckAndFixItem</c> to a repository.
        /// </summary>
        /// <param name="filename">The filename to use for the item in the repository.</param>
        /// <param name="item">The check and fix item to add to the repository.</param>
        public void AddCheckAndFixItem(string filename, CheckAndFixItem item);

        /// <summary>
        /// This method asynchronously adds a <c>CheckAndFixItem</c> to a repository.
        /// </summary>
        /// <param name="filename">The filename to use for the item in the repository.</param>
        /// <param name="item">The check and fix item to add to the repository.</param>
        /// <returns>A task representing the result of the operation.</returns>
        public Task AddCheckAndFixItemAsync(string filename, CheckAndFixItem item);

        /// <summary>
        /// This method removes a <c>CheckAndFixItem</c> from a repository.
        /// </summary>
        /// <param name="filename">The <c>CheckAndFixItem</c> to remove.</param>
        public void RemoveCheckAndFixItem(string filename);

        /// <summary>
        /// This method asynchronously removes a <c>CheckAndFixItem</c> from a repository.
        /// </summary>
        /// <param name="filename">The <c>CheckAndFixItem</c> to remove.</param>
        /// <returns>A task representing the result of the operation.</returns>
        public Task RemoveCheckAndFixItemAsync(string filename);
    }
}