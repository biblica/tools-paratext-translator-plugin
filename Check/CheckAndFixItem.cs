/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using TvpMain.Util;

namespace TvpMain.Check
{
    /// <summary>
    /// This is a model class that represents a Translation Check and Fix.
    /// </summary>
    public class CheckAndFixItem : ICloneable
    {
        /// <summary>
        /// The item's default constructor. It will ensure that a unique ID is generated.
        /// </summary>
        public CheckAndFixItem()
        {
            _id = Guid.NewGuid();
        }

        /// <summary>
        /// Alternative constructor for the V1 TVP checks/fixes that allow for a fixed GUID and other paramters
        /// </summary>
        /// <param name="guid">The GUI to use instead of the default created one</param>
        /// <param name="name">The name of the check/fix</param>
        /// <param name="description">The description </param>
        /// <param name="version">The version of the C/F</param>
        /// <param name="scope">The scope for the C/F</param>
        public CheckAndFixItem(string guid, string name, string description, string version, CheckScope scope)
        {
            Id = guid;
            Name = name;
            Description = description;
            Version = version;
            Scope = scope;
        }

        /// <summary>
        /// The internal GUID object for the ID.
        /// </summary>
        private Guid _id;

        /// <summary>
        /// The internal version object.
        /// </summary>
        private Version _version;

        /// <summary>
        /// The GUID for this Check and Fix.
        /// </summary>
        public string Id
        {
            get => _id.ToString();
            set => _id = Guid.Parse(value);
        }

        /// <summary>
        /// The name of this Check and Fix item.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The description of this Check and Fix item.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The version of this Check and Fix item.
        /// </summary>
        public string Version
        {
            get => _version == null ? "0.0.0.0" : _version.ToString();
            set => _version = new Version(value);
        }
        /// <summary>
        /// Enumeration for the scope of the check
        /// </summary>
        public enum CheckScope : int
        {
            PROJECT,
            BOOK,
            CHAPTER,
            VERSE
        }
        /// <summary>
        /// The scope of the check
        /// </summary>
        public CheckScope Scope { get; set; }
        /// <summary>
        /// Used for a default value in the resulting check items if there isn't a specific one already provided
        /// </summary>
        public string DefaultItemDescription { get; set; }
        /// <summary>
        /// The Check's regular expression. The check regex will be evaluated before the check script.
        /// </summary>
        public string CheckRegex { get; set; }
        /// <summary>
        /// The Fix's regular expression. The fix regex will be evaluated before the fix script, if present.
        /// </summary>
        public string FixRegex { get; set; }
        /// <summary>
        /// The Check's javascript script content.
        /// </summary>
        public string CheckScript { get; set; }
        /// <summary>
        /// Set of Languages this check/fix applies to. Empty = All
        /// Use standard ISO language  codes ( https://www.andiamo.co.uk/resources/iso-language-codes/)
        /// </summary>
        [XmlArrayItem("Language", Type = typeof(string), IsNullable = false)]
        [XmlArray("Languages")]
        public string[] Languages { get; set; }
        /// <summary>
        /// Set of Tags that define the limitations or project matching for this check/fix.
        /// Examples: RTL, LTR
        /// </summary>
        [XmlArrayItem("Tag", Type = typeof(string), IsNullable = false)]
        [XmlArray("Tags")]
        public string[] Tags { get; set; }
        /// <summary>
        /// The name of the file that the check was loaded from, if applicable.
        /// </summary>
        [XmlIgnore]
        public string FileName { get; set; }

        //////////////// Serialization and Deserialization functions ///////////////////////

        /// <summary>
        /// Deserialize a <c>CheckAndFixItem</c> XML file into a corresponding object.
        /// </summary>
        /// <param name="xmlFilePath">The absolute path of the <c>CheckAndFixItem</c> XML file. (required)</param>
        /// <returns>Corresponding <c>CheckAndFixItem</c> object.</returns>
        public static CheckAndFixItem LoadFromXmlFile(string xmlFilePath)
        {
            // validate input
            _ = xmlFilePath ?? throw new ArgumentNullException(nameof(xmlFilePath));

            // deserialize the file into an object
            var serializer = new XmlSerializer(typeof(CheckAndFixItem));
            using var xmlReader = new XmlTextReader(xmlFilePath);
            var obj = (CheckAndFixItem)serializer.Deserialize(xmlReader);
            return obj;
        }

        /// <summary>
        /// Deserialize <c>CheckAndFixItem</c> XML content into a corresponding object.
        /// </summary>
        /// <param name="xmlContent">A <c>Stream</c> representing a <c>CheckAndFixItem</c>. (required)</param>
        /// <returns>Corresponding <c>CheckAndFixItem</c> object.</returns>
        public static CheckAndFixItem LoadFromXmlContent(Stream xmlContent)
        {
            // validate input
            _ = xmlContent ?? throw new ArgumentNullException(nameof(xmlContent));

            // deserialize the file into an object
            var serializer = new XmlSerializer(typeof(CheckAndFixItem));
            var result = (CheckAndFixItem)serializer.Deserialize(xmlContent);
            return result;
        }

        /// <summary>
        /// Deserialize <c>CheckAndFixItem</c> XML content into a corresponding object.
        /// </summary>
        /// <param name="xmlContent">A string representing a <c>CheckAndFixItem</c>. (required)</param>
        /// <returns>Corresponding <c>CheckAndFixItem</c> object.</returns>
        public static CheckAndFixItem LoadFromXmlContent(string xmlContent)
        {
            // validate input
            _ = xmlContent ?? throw new ArgumentNullException(nameof(xmlContent));

            // deserialize the file into an object
            var serializer = new XmlSerializer(typeof(CheckAndFixItem));

            using TextReader reader = new StringReader(xmlContent);
            var result = (CheckAndFixItem)serializer.Deserialize(reader);
            return result;
        }

        /// <summary>
        /// Serialize the current <c>CheckAndFixItem</c> object into an XML file.
        /// </summary>
        /// <param name="xmlFilePath">The absolute path of the <c>CheckAndFixItem</c> XML file. (required)</param>
        public void SaveToXmlFile(string xmlFilePath)
        {
            // validate input
            _ = xmlFilePath ?? throw new ArgumentNullException(nameof(xmlFilePath));

            var writer = new XmlSerializer(this.GetType());
            using var file = File.Create(xmlFilePath);
            writer.Serialize(file, this);
        }

        /// <summary>
        /// Serialize the current <c>CheckAndFixItem</c> object into an XML <c>Stream</c>.
        /// </summary>
        /// <returns>Corresponding <c>CheckAndFixItem</c> object as an XML <c>Stream</c>.</returns>
        public Stream WriteToXmlStream()
        {
            var xmlSerializer = new XmlSerializer(this.GetType());

            var stream = new MemoryStream();
            xmlSerializer.Serialize(stream, this);
            return (Stream)stream;
        }

        /// <summary>
        /// Serialize the current <c>CheckAndFixItem</c> object into an XML string.
        /// </summary>
        /// <returns>Corresponding <c>CheckAndFixItem</c> object as an XML string.</returns>
        public string WriteToXmlString()
        {
            var xmlSerializer = new XmlSerializer(this.GetType());

            using var textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, this);
            return textWriter.ToString();
        }

        /// <summary>
        /// An override of the equals capability to validate the content of two <c>CheckAndFixItem</c>s are the same.
        /// </summary>
        /// <param name="obj">The <c>CheckAndFixItem</c> to compare against. (required)</param>
        /// <returns>True: if both objects are equal, False: otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj is CheckAndFixItem item &&
                   Id == item.Id &&
                   Name == item.Name &&
                   Description == item.Description &&
                   Version == item.Version &&
                   Scope == item.Scope &&
                   DefaultItemDescription == item.DefaultItemDescription &&
                   CheckRegex == item.CheckRegex &&
                   FixRegex == item.FixRegex &&
                   CheckScript == item.CheckScript &&
                   Tags == item.Tags &&
                   Languages == item.Languages;
        }

        /// <summary>
        /// This function will make a deep copy of our <c>CheckAndFixItem</c>.
        /// </summary>
        /// <returns>A copy of the current <c>CheckAndFixItem</c> artifact.</returns>
        public object Clone()
        {
            // deep clone the object by utilizing the serializing and deserializing functions.
            return CheckAndFixItem.LoadFromXmlContent(this.WriteToXmlString());
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (MainConsts.HASH_PRIME) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ (Version != null ? Version.GetHashCode() : 0);
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ Scope.GetHashCode();
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ (Tags != null ? Tags.GetHashCode() : 0);
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ (Languages != null ? Languages.GetHashCode() : 0);
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ (DefaultItemDescription != null ? DefaultItemDescription.GetHashCode() : 0);
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ (CheckRegex != null ? CheckRegex.GetHashCode() : 0);
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ (FixRegex != null ? FixRegex.GetHashCode() : 0);
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ (CheckScript != null ? CheckScript.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}