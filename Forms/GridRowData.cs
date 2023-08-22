using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvpMain.Check;

namespace TvpMain.Forms
{
    /// <summary>
    /// Represents the data displayed in a row of the check items grid.
    /// </summary>
    public class GridRowData
    {
        public bool Selected { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string Name { get; }
        public string Description { get; }
        public string Version { get; }
        public string Languages { get; }
        public string Tags { get; }
        public string Id { get; }
        public bool Active { get; }
        public string Tooltip { get; }
        public IRunnable Item { get; }

        public GridRowData(bool selected, string location, string type, string name, string description, string version, 
            string languages, string tags, string id, bool active, string tooltip, IRunnable item)
        {
            Selected = selected;
            Location = location;
            Type = type;
            Name = name;
            Description = description;
            Version = version;
            Languages = languages;
            Tags = tags;
            Id = id;
            Active = active;
            Tooltip = tooltip;
            Item = item;
        }
    }
}
