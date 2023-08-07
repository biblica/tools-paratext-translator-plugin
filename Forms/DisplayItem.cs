using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvpMain.Check;

namespace TvpMain.Forms
{
    /// <summary>
    /// Used for displaying the check/fix items
    /// This is used so that we can remember if the item is
    /// selected or not during search/filtering
    /// </summary>
    public class DisplayItem
    {
        public bool Selected { get; set; }
        public string Location { get; set; }
        public string Name { get; }
        public string Description { get; }
        public string Version { get; }
        public string Languages { get; }
        public string Tags { get; }
        public string Id { get; }
        public bool Active { get; }
        public string Tooltip { get; }
        public CheckAndFixItem Item { get; }

        public DisplayItem(bool selected, string location, string name, string description, string version, 
            string languages, string tags, string id, bool active, string tooltip, CheckAndFixItem item)
        {
            Selected = selected;
            Location = location;
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
