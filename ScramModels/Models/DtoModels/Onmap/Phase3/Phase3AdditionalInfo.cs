using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.OnmapDto
{
    public class Phase3AdditionalInfo
    {
        public Phase3AdditionalInfoArea area { get; set; }
        public string balconies { get; set; }
        public string bathrooms { get; set; }
        public string construction_year { get; set; }
        public string elevators { get; set; }
        public Phase3AdditionalInfoEntryDate entry_date { get; set; }
        public Phase3AdditionalInfoFloor floor { get; set; }
        public string hasBasement { get; set; }
        public Phase3AdditionalInfoLightDirection light_direction { get; set; }
        public string only_part { get; set; }
        public string rooms { get; set; }
        public string toilets { get; set; }
    }
}
