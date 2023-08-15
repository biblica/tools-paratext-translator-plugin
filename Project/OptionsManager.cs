using Paratext.Data.BibleModule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TvpMain.CheckManagement;
using TvpMain.Util;

namespace TvpMain.Project
{
    internal class OptionsManager
    {
        /// <summary>
        /// The name of the file where plugin options are stored.
        /// </summary>
        public const string OPTIONS_FILE_NAME = "Options.xml";

        /// <summary>
        /// The full path to the file where TVP options are stored.
        /// </summary>
        private static string _optionsFilePath = Path.Combine(Directory.GetCurrentDirectory(),
                MainConsts.TVP_FOLDER_NAME, MainConsts.CONFIGURATION_FOLDER, OPTIONS_FILE_NAME);

        /// <summary>
        /// Loads TVP options from the options file.
        /// </summary>
        public static TvpOptions LoadOptions()
        {
            FileStream optionsFile;
            var serializer = new XmlSerializer(typeof(TvpOptions));
            try
            {
                optionsFile = File.Open(_optionsFilePath, FileMode.Open);
            }
            catch (FileNotFoundException)
            {
                optionsFile = File.Create(_optionsFilePath);
                var newOptions = new TvpOptions();
                serializer.Serialize(optionsFile, newOptions);
                optionsFile.Seek(0, SeekOrigin.Begin);
            }
            var options = (TvpOptions)serializer.Deserialize(optionsFile);
            optionsFile.Close();

            return options;
        }

        /// <summary>
        /// Saves TVP options to an xml file.
        /// </summary>
        /// <param name="options">The options to save</param>
        public static void SaveOptions(TvpOptions options)
        {
            var serializer = new XmlSerializer(options.GetType());
            using var file = File.Create(_optionsFilePath);
            serializer.Serialize(file, options);
        }
    }
}
