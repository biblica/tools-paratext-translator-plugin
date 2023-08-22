using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TvpMain.Check
{
    /// <summary>
    /// Represents a check or group of checks
    /// </summary>
    public interface IRunnable
    {
        /// <summary>
        /// The internal GUID object for the runnable item.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The name of this runnable item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of this runnable item.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The version of this runnable item.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Set of Languages this runnable item applies to. empty = All, null = None
        /// Use standard ISO language  codes ( https://www.andiamo.co.uk/resources/iso-language-codes/)
        /// </summary>
        public string[] Languages { get; set; }

        /// <summary>
        /// Set of Tags that define the limitations or project matching for this runnable item.
        /// Examples: RTL, LTR
        /// </summary>
        public string[] Tags { get; set; }

        public List<CheckAndFixItem> Checks { get; }

        /// <summary>
        /// The name of the file that the runnable item was loaded from, if applicable.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Serialize the runnable item into an XML file.
        /// </summary>
        /// <param name="xmlFilePath">The absolute path of the XML file. (required)</param>
        public void SaveToXmlFile(string xmlFilePath);

        /// <summary>
        /// Serialize the current runnable item into an XML <c>Stream</c>.
        /// </summary>
        /// <returns>Corresponding item as an XML <c>Stream</c>.</returns>
        public Stream WriteToXmlStream();

    }
}
