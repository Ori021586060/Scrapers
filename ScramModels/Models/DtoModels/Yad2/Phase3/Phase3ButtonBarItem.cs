using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.Yad2Dto
{
    public class Phase3ButtonBarItem
    {
        public string key { get; set; }
        public string is_menu { get; set; }
        public List<Phase3ButtonBarItemMenuItem> menu_items { get; set; }
        public string @event { get; set; }
        public string link { get; set; }
        public string title { get; set; }
    }
}
