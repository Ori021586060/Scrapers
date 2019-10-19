using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.OnmapDto
{
    public class Phase3ObjectDto
    {
        public string No { get => "-"; }
        public Phase3AdditionalInfo additional_info {get;set;}
        public Phase3Address address { get; set; }
        public string agency_id { get; set; }
        public List<string> commodities { get; set; }
        public Phase3Contacts contacts { get; set; }
        public string created_at { get; set; }
        public string description { get; set; }
        public string has_photos { get; set; }
        public string id { get; set; }
        public List<Phase3Image> images { get; set; }
        public string is_active { get; set; }
        public string is_autojump_enabled { get; set; }
        public string is_promoted { get; set; }
        public string is_suspended { get; set; }
        public string is_top_promoted { get; set; }
        public List<string> plans { get; set; }
        public string price { get; set; }
        public string property_type { get; set; }
        public string published { get; set; }
        public string realtor_property { get; set; }
        public string search_date { get; set; }
        public string search_option { get; set; }
        public string section { get; set; }
        public string share_date { get; set; }
        public string thumbnail { get; set; }
        public string updated_at { get; set; }
        public string user_id { get; set; }
        public Phase3Verification verification { get; set; }
        public List<Phase3Video> videos { get; set; }
        public string vr_link { get; set; }
    }
}
