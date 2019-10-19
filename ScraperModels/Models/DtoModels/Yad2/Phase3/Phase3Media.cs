using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.Yad2Dto
{
    public class Phase3Media
    {
        public string zone { get; set; }
        public string layout { get; set; }
        public Phase3MediaParams @params { get; set; }
        public string artiMedia_iscroll_group { get; set; }
        public List<string> eratePopUnderPerIds { get; set; }
        public List<string> locations { get; set; }
        public string lightboxLocations { get; set; }
    }
}
