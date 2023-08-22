/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Paratext.Data.BibleModule;
using SIL.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using TvpMain.Util;
using static TvpMain.Check.CheckAndFixItem;

namespace TvpMain.Check
{
    /// <summary>
    /// Holds the information to be serialized when a CheckGroup object is saved to a file.
    /// </summary>
    public class CheckGroupDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public CheckScope Scope { get; set; }
        public string DefaultDescription { get; set; }
        [XmlArrayItem("Language", Type = typeof(string), IsNullable = false)]
        [XmlArray("Languages")]
        public string[] Languages { get; set; }
        [XmlArrayItem("Tag", Type = typeof(string), IsNullable = false)]
        [XmlArray("Tags")]
        public string[] Tags { get; set; }
        [XmlArrayItem("CheckId", Type = typeof(string), IsNullable = false)]
        [XmlArray("CheckIds")]
        public string[] CheckIds { get; set; }
    }
}