using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.OnmapDto
{
    public class DataRow
    {
        public string created_at { get; set; }
        public AdditionalInfoModel additional_info { get; set; }
        public string price { get; set; }
        public string search_option { get; set; }
        public string property_type { get; set; }
        public AddressModel address { get; set; }
        public string vr_link { get; set; }
        public string search_date { get; set; }
        public List<ImageModel> images { get; set; }
        public string is_top_promoted { get; set; }
        public string is_promoted { get; set; }
        public string id { get; set; }
        public string thumbnail { get; set; }
    }
}
