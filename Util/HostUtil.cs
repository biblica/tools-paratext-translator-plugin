/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using AddInSideViews;
using Newtonsoft.Json;
using Paratext.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using PtxUtils;
using TvpMain.CheckManagement;
using TvpMain.Project;
using TvpMain.Result;
using TvpMain.Text;

namespace TvpMain.Util
{
    /// <summary>
    /// Process-wide error utilities.
    /// </summary>
    public class HostUtil
    {
        /// <summary>
        /// Thread-safe singleton accessor.
        /// </summary>
        public static HostUtil Instance { get; } = new HostUtil();

        private const string ADMIN_ROLE = "Administrator";

        /// <summary>
        /// Indicates whether ParatextData has been initialized.
        ///
        /// Note: Uses int because Interlocked.CompareExchange doesn't work with bool.
        /// </summary>
        private int _isParatextDataInit = 0;

        /// <summary>
        /// Event used to track paratext data initialization complete.
        /// </summary>
        private readonly CountdownEvent _paratextDataSetupEvent = new CountdownEvent(1);

        /// <summary>
        /// Global reference to plugin, to route logging.
        /// </summary>
        private TranslationValidationPlugin _translationValidationPlugin;

        /// <summary>
        /// Global reference to host interface, providing Paratext services including logging.
        /// </summary>
        private IHost _host;

        /// <summary>
        /// Property for assignment from plugin entry method.
        /// </summary>
        public TranslationValidationPlugin TranslationValidationPlugin
        {
            set => _translationValidationPlugin = value;
        }

        /// <summary>
        /// Property for assignment from plugin entry method.
        /// </summary>
        public IHost Host
        {
            set => _host = value;
        }

        /// <summary>
        /// Reports exception to log and message box w/o prefix text.
        /// </summary>
        /// <param name="ex"></param>
        public void ReportError(Exception ex)
        {
            ReportError(null, ex);
        }

        /// <summary>
        /// Reports exception to log and message box w/prefix text.
        /// </summary>
        /// <param name="prefixText">Prefix text (optional, may be null; default used when null).</param>
        /// <param name="ex">Exception (required).</param>
        public void ReportError(string prefixText, Exception ex)
        {
            ReportError(prefixText, true, ex);
        }

        /// <summary>
        /// Set up the ParatextData libraries for project input/output.
        ///
        /// Will block until initialization complete, which takes at least a few seconds
        /// on typical systems and may scale per the number of projects.
        /// 
        /// Thread safe, may be called repeatedly
        /// </summary>
        /// <param name="isToBlock">True to block until initialization complete, false otherwise.</param>
        public void InitParatextData(bool isToBlock)
        {
            if (Interlocked.CompareExchange(ref _isParatextDataInit, 1, 0) == 0)
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        var executingAssembly = Assembly.GetExecutingAssembly();
                        var assemblyPath = Path.GetDirectoryName(executingAssembly.Location);
                        if (assemblyPath == null)
                        {
                            throw new InvalidOperationException(
                                $"plugin assembly in unexpected location: {executingAssembly.Location}");
                        }

                        var assemblyDir = new DirectoryInfo(assemblyPath);
                        if (assemblyDir.Parent?.Parent == null)
                        {
                            throw new InvalidOperationException(
                                $"plugin directory in unexpected location: {assemblyDir.FullName}");
                        }

                        // fall back on plugin working dir, if paratext.exe not found
                        var paratextDir = assemblyDir.Parent.Parent;
                        PtxUtils.Platform.BaseDirectory =
                            File.Exists(Path.Combine(paratextDir.FullName, "Paratext.exe"))
                                ? paratextDir.FullName
                                : assemblyPath;
                        ParatextData.Initialize();

#if DEBUG
                        ReportNonFatalParatextDataErrors();
#endif
                    }
                    catch (Exception ex)
                    {
                        ReportError("Can't initialize ParatextData", true, ex);
                    }
                    finally
                    {
                        _paratextDataSetupEvent.Signal();
                    }
                });
            }

            if (isToBlock)
            {
                _paratextDataSetupEvent.Wait();
            }
        }

        /// <summary>
        /// Reports non-fatal ParatextData initialization errors.
        /// </summary>
        public void ReportNonFatalParatextDataErrors()
        {
            var errorText = string.Join(Environment.NewLine,
                ScrTextCollection.ErrorMessages.Select(messageItem =>
                    $"Project: {messageItem.ProjectName}, type: {messageItem.ProjecType}, reason: {messageItem.Reason}, exception: {messageItem.Exception}."));
            if (!string.IsNullOrWhiteSpace(errorText))
            {
                ReportError("There were non-fatal initialization errors (performance may be impacted)."
                            + Environment.NewLine + Environment.NewLine
                            + errorText, false, null);
            }
        }

        /// <summary>
        /// Reports exception to log and message box w/prefix text.
        /// </summary>
        /// <param name="prefixText">Prefix text (optional, may be null; default used when null).</param>
        /// <param name="includeStackTrace">True to include stack trace, false otherwise.</param>
        /// <param name="ex">Exception (optional, may be null).</param>
        public void ReportError(string prefixText, bool includeStackTrace, Exception ex)
        {
            string messageText = null;
            if (String.IsNullOrWhiteSpace(prefixText))
            {
                prefixText = "Error: Please contact support/admin";
            }

            // Every time an error is reported it is also logged.
            LogLine(prefixText + ex.ToString(), true);

            if (ex == null)
            {
                messageText = prefixText;
            }
            else
            {
                if (includeStackTrace)
                {
                    messageText = prefixText
                                  + Environment.NewLine + Environment.NewLine
                                  + "Details: " + ex.ToString() + Environment.NewLine;
                }
                else
                {
                    messageText = prefixText
                                  + $" (Details: {ex.Message}).";
                }
            }

            MessageBox.Show(messageText, "Notice...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            LogLine(messageText, true);
        }

        /// <summary>
        /// Log text to Paratext's app log and the console.
        /// </summary>
        /// <param name="inputText">Input text (required).</param>
        /// <param name="isError">Error flag.</param>
        public void LogLine(string inputText, bool isError)
        {
            (isError ? Console.Error : Console.Out).WriteLine(inputText);
            _host?.WriteLineToLog(_translationValidationPlugin, inputText);
        }

        /// <summary>
        /// Loads the <c>ProjectCheckSettings</c> for the specified project.
        /// </summary>
        /// <param name="projectName">The project name.</param>
        /// <returns>The <c>ProjectCheckSettings</c>, or empty settings if none could be loaded.</returns>
        public ProjectCheckSettings GetProjectCheckSettings(string projectName)
        {
            ProjectCheckSettings settings = new ProjectCheckSettings();

            if (projectName == null || projectName.Length < 1)
            {
                throw new ArgumentNullException(nameof(projectName));
            }

            var inputData =
                _host.GetPlugInData(_translationValidationPlugin, projectName,
                    MainConsts.CHECK_SETTINGS_DATA_ID);
            if (inputData != null)
            {
                settings = ProjectCheckSettings.LoadFromXmlContent(inputData);
            }

            return settings;
        }

        /// <summary>
        /// Saves the project check settings for the given project
        /// </summary>
        /// <param name="projectName">The name of the project to save the settings to</param>
        /// <param name="settings">The check/fix settings to save. Right now, the default checks.</param>
        public void PutProjectCheckSettings(string projectName, ProjectCheckSettings settings)
        {
            if (projectName == null || projectName.Length < 1)
            {
                throw new ArgumentNullException(nameof(projectName));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _host.PutPlugInData(_translationValidationPlugin, projectName,
                MainConsts.CHECK_SETTINGS_DATA_ID,
                settings.WriteToXmlString());
        }

        /// <summary>
        /// Gets the list of <c>CheckResultItem</c>s that have been denied for the project.
        /// </summary>
        /// <param name="projectName">The name of the project to get the list for</param>
        /// <returns>A list of hashes representing <c>CheckResultItem</c>s that have been denied.</returns>
        public List<int> GetProjectDeniedResults(string projectName)
        {
            List<int> deniedResults = new List<int>();

            if (projectName == null || projectName.Length < 1)
            {
                throw new ArgumentNullException(nameof(projectName));
            }

            var inputData =
                _host.GetPlugInData(_translationValidationPlugin, projectName,
                    MainConsts.DENIED_RESULTS_DATA_ID);
            if (inputData != null)
            {
                var serializer = new XmlSerializer(typeof(List<int>));
                using TextReader reader = new StringReader(inputData);
                deniedResults = (List<int>) serializer.Deserialize(reader);
            }

            return deniedResults;
        }

        /// <summary>
        /// Saves a list of hashes representing <c>CheckResultItem</c>s that have been denied for the project.
        /// </summary>
        /// <param name="projectName">The name of the project to save the list for</param>
        /// <param name="deniedResults">A list of hashes representing <c>CheckResultItem</c>s that have been denied.</param>
        public void PutProjectDeniedResults(string projectName, List<int> deniedResults)
        {
            if (projectName == null || projectName.Length < 1)
            {
                throw new ArgumentNullException(nameof(projectName));
            }

            if (deniedResults == null)
            {
                throw new ArgumentNullException(nameof(deniedResults));
            }

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<int>));
            using StringWriter textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, deniedResults);

            _host.PutPlugInData(_translationValidationPlugin, projectName,
                MainConsts.DENIED_RESULTS_DATA_ID,
                textWriter.ToString());
        }

        /// <summary>
        /// Method to determine if the current user is an administrator or not. This loads the ProjectUserAccess.xml file from 
        /// the project and compares the users there against the current user name from IHost.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns>True, if the current user is an Admin for the given project</returns>
        public bool isCurrentUserProjectAdmin(string projectName)
        {
            if (projectName == null || projectName.Length < 1)
            {
                throw new ArgumentNullException(nameof(projectName));
            }

            FileManager fileManager = new FileManager(_host, projectName);

            try
            {
                using Stream reader =
                    new FileStream(Path.Combine(fileManager.ProjectDir.FullName, "ProjectUserAccess.xml"),
                        FileMode.Open);
                ProjectUserAccess projectUserAccess = ProjectUserAccess.LoadFromXML(reader);

                foreach (User user in projectUserAccess.Users)
                {
                    if (user.UserName.Equals(_host.UserName) && user.Role.Equals(ADMIN_ROLE))
                    {
                        // Bail as soon as we find a match
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\nContinuing as non-admin.", "Notice...", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            return false;
        }

        /// <summary>
        /// Attempts to connect to the internet.
        /// </summary>
        /// <returns>Whether the connection was successful.</returns>
        public bool TryGoOnline()
        {
            const string host = @"https://aws.amazon.com";
            var result = false;
            var request = (HttpWebRequest) WebRequest.Create(host);
            request.Method = "HEAD"; // Don't expect a body.
            try
            {
                var response = (HttpWebResponse) request.GetResponse();
                result = (int) response.StatusCode < 400;
            }
            catch (WebException ex)
            {
                Instance.LogLine("There was a problem reaching AWS: " + ex, false);
            }

            IsOnline = result;
            return result;
        }

        /// <summary>
        /// Whether or not the system is online.
        /// </summary>
        public bool IsOnline;

        /// <summary>
        /// The name of the current Paratext user.
        /// </summary>
        public string CurrentUser { get { return _host.UserName; } }

        /// <summary>
        /// This function navigates to a specific project's BCV in the Paratext GUI.
        /// 
        /// <para>
        /// Note: This isn't released yet for the stable Paratext client versions. Per Tim Steenwyk (Paratext software developer) 
        /// "it will be a while before most users have the changes". For the meantime, the code will be commented out until it becomes
        /// Mainstream.
        /// </para><
        /// </summary>
        /// <param name="projectName">The Paratext project shortname. EG: "spaNVI15"</param>
        /// <param name="book">The project's book 1-based index.</param>
        /// <param name="chapter">The project's chapter 1-based index.</param>
        /// <param name="verse">The project's verse 1-based index.</param>
        public void GotoBcvInGui(string projectName, int book, int chapter, int verse)
        {
            var versificationName = _host.GetProjectVersificationName(projectName);
            var bbbcccvvvReference = BookUtil.BcvToRef(book, chapter, verse);
            // Not yet available in latest version
            _host.GotoReference(bbbcccvvvReference, versificationName, projectName);
        }

        /// <summary>
        /// Stores the ignore list to the host's plugin data storage.
        /// </summary>
        /// <param name="projectName">Active project name (required).</param>
        /// <param name="bookId"></param>
        /// <param name="outputItems">Ignore list.</param>
        public void PutResultItems(string projectName, string bookId, IEnumerable<ResultItem> outputItems)
        {
            if (projectName == null || projectName.Length < 1)
            {
                Util.HostUtil.Instance.LogLine("Project name is invalid, can't operate so throwing exception.", true);
                throw new ArgumentNullException(nameof(projectName));
            }

            if (bookId == null || bookId.Length < 1)
            {
                Util.HostUtil.Instance.LogLine("Book id is invalid, can't operate so throwing exception.", true);
                throw new ArgumentNullException(nameof(bookId));
            }

            _host.PutPlugInData(_translationValidationPlugin, projectName,
                string.Format(MainConsts.RESULT_ITEMS_DATA_ID_FORMAT, bookId),
                JsonConvert.SerializeObject(outputItems));
        }

        /// <summary>
        /// Retrieve the ignore list from the host's plugin data storage.
        /// </summary>
        /// <param name="projectName">Active project name (required).</param>
        /// <param name="bookId"></param>
        /// <returns>Ignore list.</returns>
        public IList<ResultItem> GetResultItems(string projectName, string bookId)
        {
            if (projectName == null || projectName.Length < 1)
            {
                throw new ArgumentNullException(nameof(projectName));
            }

            if (bookId == null || bookId.Length < 1)
            {
                throw new ArgumentNullException(nameof(bookId));
            }

            var inputData =
                _host.GetPlugInData(_translationValidationPlugin, projectName,
                    string.Format(MainConsts.RESULT_ITEMS_DATA_ID_FORMAT, bookId));
            return inputData == null
                ? Enumerable.Empty<ResultItem>().ToList()
                : JsonConvert.DeserializeObject<List<ResultItem>>(inputData);
        }
    }
}