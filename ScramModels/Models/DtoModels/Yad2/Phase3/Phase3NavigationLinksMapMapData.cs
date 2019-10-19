using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.Yad2Dto
{
    public class Phase3NavigationLinksMapMapData
    {
        public string view_type { get; set; }
        public Phase3NavigationLinksMapMapDataHeaderInfo header_info { get; set; }
        public Phase3NavigationLinksMapMapDataCoordinates coordinates { get; set; }
        public string zoom { get; set; }
    }
}
