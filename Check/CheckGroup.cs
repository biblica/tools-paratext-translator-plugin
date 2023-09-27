/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Amazon.S3.Model;
using Paratext.Data.BibleModule;
using Paratext.Data.ProjectProgress;
using SIL.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using TvpMain.Util;
using static TvpMain.Check.CheckAndFixItem;

namespace TvpMain.Check
{
    /// <summary>
    /// A class that represents an ordered list of checks that can be run as a group.
    /// </summary>
    public class CheckGroup : IRunnable, ICloneable
    {
        /// <summary>
        /// The group's default constructor. It will ensure that a unique ID is generated.
        /// </summary>
        public CheckGroup()
        {
            _id = Guid.NewGuid();
        }

        /// <summary>
        /// Alternative constructor that allows for a fixed GUID and other parameters
        /// </summary>
        /// <param name="guid">The guid to use instead of the default created one</param>
        /// <param name="name">The name of the check group</param>
        /// <param name="description">The description of the check group</param>
        /// <param name="version">The version of the check group</param>
        /// <param name="scope">The scope for the check group</param>
        public CheckGroup(string guid, string name, string description, string version, CheckScope scope)
        {
            Id = guid;
            Name = name;
            Description = description;
            Version = version;
            Scope = scope;
        }

        /// <summary>
        /// Construct a CheckGroup from an existing one.
        /// </summary>
        /// <param name="group">The group to copy.</param>
        public CheckGroup(CheckGroup group)
            : this(group.getDTO())
        {
        }

        /// <summary>
        /// Construct a new CheckGroup from a <c>CheckGroupDTO</c> object.
        /// </summary>
        /// <param name="dto">The CheckGroupDTO object.</param>
        public CheckGroup(CheckGroupDTO dto)
        {
            Id = dto.Id;
            Name = dto.Name;
            Description = dto.Description;
            DefaultDescription = dto.DefaultDescription;
            Version = dto.Version;
            Scope = dto.Scope;
            foreach (var checkId in dto.CheckIds)
            {
                Checks.Add(new KeyValuePair<string, CheckAndFixItem>(checkId, null));
            }
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
        /// The GUID for this check group.
        /// </summary>
        public string Id
        {
            get => _id.ToString();
            set => _id = Guid.Parse(value);
        }

        /// <summary>
        /// The name of this check group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of this check group.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The version of this check group.
        /// </summary>
        public string Version
        {
            get => _version == null ? "0.0.0.0" : _version.ToString();
            set => _version = new Version(value);
        }

        /// <summary>
        /// The scope of the check group.
        /// </summary>
        public CheckScope Scope { get; set; }

        /// <summary>
        /// Used for a default value in the resulting check groups if there isn't a specific one already provided
        /// </summary>
        public string DefaultDescription { get; set; }

        /// <summary>
        /// Set of languages this check group applies to. The languages for a group
        /// are determined by the checks that it references. A group only supports a 
        /// language if all of its checks support it.
        /// empty = all languages, null = incompatible languages
        /// Use standard ISO language  codes ( https://www.andiamo.co.uk/resources/iso-language-codes/)
        /// </summary>
        [XmlIgnore]
        public string[] Languages
        {
            get
            {
                List<string> languages = new List<string>();
                List<string> languagesToRemove = new List<string>();
                bool languagesAdded = false;
                foreach (var checkKvp in Checks)
                {
                    if (checkKvp.Value is null) continue;
                    CheckAndFixItem check = checkKvp.Value;
                    if (languages.Count == 0 && languagesAdded == false && check.Languages.Count() > 0)
                    {
                        languages.AddRange(check.Languages);
                        languagesAdded = true;
                    }
                    else if (languages.Count > 0 && check.Languages.Count() > 0)
                    {
                        languagesToRemove.Clear();
                        foreach (string language in languages)
                        {
                            if (!check.Languages.Contains(language))
                            {
                                languagesToRemove.Add(language);
                            }
                        }
                        foreach (string languageToRemove in languagesToRemove)
                        {
                            languages.Remove(languageToRemove);
                        }
                    }
                }

                // There are no common languages between all checks in this group.
                if (languagesAdded && languages.Count == 0) return null;

                return languages.ToArray();
            }
        }

        /// <summary>
        /// Set of tags that define the restrictions on project matching for this group. The tags
        /// for a group are determined by the tags of the checks it references.
        /// empty = no tags, null = incompatible tags
        /// Examples: RTL, LTR
        /// </summary>
        [XmlIgnore]
        public string[] Tags
        {
            get
            {
                List<string> tags = new List<string>();
                List<string> tagsToRemove = new List<string>();
                bool tagsAdded = false;
                foreach (var checkKvp in Checks)
                {
                    if (checkKvp.Value is null) continue;
                    CheckAndFixItem check = checkKvp.Value;
                    if (tags.Count == 0 && tagsAdded == false && check.Tags.Count() > 0)
                    {
                        tags.AddRange(check.Tags);
                        tagsAdded = true;
                    }
                    else if (tags.Count > 0 && check.Tags.Count() > 0)
                    {
                        tagsToRemove.Clear();
                        foreach (string tag in tags)
                        {
                            if (!check.Tags.Contains(tag))
                            {
                                tagsToRemove.Add(tag);
                            }
                        }
                        foreach (string tagToRemove in tagsToRemove)
                        {
                            tags.Remove(tagToRemove);
                        }
                    }
                }

                // The checks in this group contain incompatible tags.
                if (tagsAdded && tags.Count == 0) return null;

                return tags.ToArray();
            }
        }

        /// <summary>
        /// Ordered list of the checks in this check group. Each KeyValuePair contains
        /// the Id of a check and the corresponding CheckAndFixItem object. The object
        /// value may be null if the check was deleted. 
        /// </summary>
        public List<KeyValuePair<string, CheckAndFixItem>> Checks { get; set; } = new List<KeyValuePair<string, CheckAndFixItem>>();

        /// <summary>
        /// Adds a check to this group.
        /// </summary>
        /// <param name="check">The check to add</param>
        public void AddCheck(CheckAndFixItem check)
        {
            Checks.Add(new KeyValuePair<string, CheckAndFixItem>(check.Id, check));
        }

        /// <summary>
        /// Adds a KeyValue pair representing a check to this group.
        /// </summary>
        /// <param name="check">The check to add</param>
        public void AddCheck(KeyValuePair<string, CheckAndFixItem> checkKvp)
        {
            Checks.Add(new KeyValuePair<string, CheckAndFixItem>(checkKvp.Key, checkKvp.Value));
        }

        /// <summary>
        /// The name of the file that the group was loaded from, if applicable.
        /// </summary>
        [XmlIgnore]
        public string FileName { get; set; }

        /// <summary>
        /// Update this group to match the values of another group.
        /// </summary>
        /// <param name="group">The group to copy values from.</param>
        public void Update(CheckGroup group)
        {
            Name = group.Name;
            Description = group.Description;
            Version = group.Version;
            Scope = group.Scope;
            DefaultDescription = group.DefaultDescription;
            FileName = group.FileName;
            var checks = new List<KeyValuePair<string, CheckAndFixItem>>();
            foreach (var checkKvp in group.Checks)
            {
                checks.Add(new KeyValuePair<string, CheckAndFixItem>(checkKvp.Key, checkKvp.Value));
            }
            Checks = checks;
        }

        /// <summary>
        /// The file extension used when a check group is saved to a file.
        /// </summary>
        public static string FileExtension { get; } = "group.xml";

        /// <summary>
        /// Creates a CheckGroupDTO object from this group. 
        /// </summary>
        /// <returns></returns>
        private CheckGroupDTO getDTO()
        {
            var dto = new CheckGroupDTO();
            dto.Id = Id;
            dto.Name = Name;
            dto.Description = Description;
            dto.DefaultDescription = DefaultDescription;
            dto.Version = Version;
            dto.Languages = Languages;
            dto.Tags = Tags;
            dto.Scope = Scope;
            var checkIds = from check in Checks select check.Key;
            dto.CheckIds = checkIds.ToArray();
            return dto;
        }

        //////////////// Serialization and Deserialization functions ///////////////////////

        /// <summary>
        /// Deserialize a <c>CheckGroup</c> XML file into a corresponding object.
        /// </summary>
        /// <param name="xmlFilePath">The absolute path of the <c>CheckGroup</c> XML file. (required)</param>
        /// <returns>Corresponding <c>CheckGroup</c> object.</returns>
        public static CheckGroup LoadFromXmlFile(string xmlFilePath)
        {
            // validate input
            _ = xmlFilePath ?? throw new ArgumentNullException(nameof(xmlFilePath));

            // deserialize the file into an object
            var serializer = new XmlSerializer(typeof(CheckGroupDTO));
            using var xmlReader = new XmlTextReader(xmlFilePath);
            var checkGroupDto = (CheckGroupDTO)serializer.Deserialize(xmlReader);

            return new CheckGroup(checkGroupDto);
        }

        /// <summary>
        /// Deserialize <c>CheckGroup</c> XML content into a corresponding object.
        /// </summary>
        /// <param name="xmlContent">A <c>Stream</c> representing a <c>CheckGroup</c>. (required)</param>
        /// <returns>Corresponding <c>CheckGroup</c> object.</returns>
        public static CheckGroup LoadFromXmlContent(Stream xmlContent)
        {
            // validate input
            _ = xmlContent ?? throw new ArgumentNullException(nameof(xmlContent));

            // deserialize the file into an object
            var serializer = new XmlSerializer(typeof(CheckGroupDTO));
            var checkGroupDto = (CheckGroupDTO)serializer.Deserialize(xmlContent);

            return new CheckGroup(checkGroupDto);
        }

        /// <summary>
        /// Deserialize <c>CheckGroup</c> XML content into a corresponding object.
        /// </summary>
        /// <param name="xmlContent">A string representing a <c>CheckGroup</c>. (required)</param>
        /// <returns>Corresponding <c>CheckGroup</c> object.</returns>
        public static CheckGroup LoadFromXmlContent(string xmlContent)
        {
            // validate input
            _ = xmlContent ?? throw new ArgumentNullException(nameof(xmlContent));

            // deserialize the file into an object
            var serializer = new XmlSerializer(typeof(CheckGroupDTO));
            using TextReader reader = new StringReader(xmlContent);
            var checkGroupDto = (CheckGroupDTO)serializer.Deserialize(reader);

            return new CheckGroup(checkGroupDto);
        }

        /// <summary>
        /// Serialize the current <c>CheckGroup</c> object into an XML file.
        /// </summary>
        /// <param name="xmlFilePath">The absolute path of the <c>CheckGroup</c> XML file. (required)</param>
        public void SaveToXmlFile(string xmlFilePath)
        {
            // validate input
            _ = xmlFilePath ?? throw new ArgumentNullException(nameof(xmlFilePath));

            CheckGroupDTO dto = getDTO();
            var writer = new XmlSerializer(dto.GetType());
            using var file = File.Create(xmlFilePath);
            writer.Serialize(file, dto);
        }

        /// <summary>
        /// Serialize the current <c>CheckGroup</c> object into an XML <c>Stream</c>.
        /// </summary>
        /// <returns>Corresponding <c>CheckGroup</c> object as an XML <c>Stream</c>.</returns>
        public Stream WriteToXmlStream()
        {
            CheckGroupDTO dto = getDTO();
            var xmlSerializer = new XmlSerializer(dto.GetType());
            var stream = new MemoryStream();
            xmlSerializer.Serialize(stream, dto);

            return (Stream)stream;
        }

        /// <summary>
        /// Serialize the current <c>CheckGroup</c> object into an XML string.
        /// </summary>
        /// <returns>Corresponding <c>CheckGroup</c> object as an XML string.</returns>
        public string WriteToXmlString()
        {
            CheckGroupDTO dto = getDTO();
            var xmlSerializer = new XmlSerializer(dto.GetType());
            using var textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, dto);

            return textWriter.ToString();
        }

        /// <summary>
        /// An override of the equals capability to validate the content of two <c>CheckGroup</c>s are the same.
        /// </summary>
        /// <param name="obj">The <c>CheckGroup</c> to compare against. (required)</param>
        /// <returns>True: if both objects are equal, False: otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj is CheckGroup item &&
                   Id == item.Id &&
                   Name == item.Name &&
                   Description == item.Description &&
                   Version == item.Version &&
                   Scope == item.Scope &&
                   DefaultDescription == item.DefaultDescription &&
                   Checks == item.Checks &&
                   Tags == item.Tags &&
                   Languages == item.Languages;
        }

        /// <summary>
        /// This function will make a deep copy of our <c>CheckGroup</c>.
        /// </summary>
        /// <returns>A copy of the current <c>CheckGroup</c> artifact.</returns>
        public object Clone()
        {
            // deep clone the object by utilizing the serializing and deserializing functions.
            var newCheckGroup = CheckGroup.LoadFromXmlContent(this.WriteToXmlString());
            var newChecks = new List<KeyValuePair<string, CheckAndFixItem>>();
            foreach (var checkKvp in this.Checks)
            {
                newChecks.Add(new KeyValuePair<string, CheckAndFixItem>(checkKvp.Key, checkKvp.Value));
            }
            newCheckGroup.Checks = newChecks;

            return newCheckGroup;
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
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ (DefaultDescription != null ? DefaultDescription.GetHashCode() : 0);
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ (Checks != null ? Checks.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}