using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.Yad2Dto
{
    public class Phase3ObjectDtoAgencyInfo
    {
        public string agency_info_title { get; set; }
        public string agency_info_link_title { get; set; }
        public string agency_info_link_url { get; set; }
        public string agency_info_link_event { get; set; }
        public List<Phase3ObjectDtoAgencyInfoAgencyInfoItems> agency_info_items { get; set; }
        public string has_minisite { get; set; }

    }
}
