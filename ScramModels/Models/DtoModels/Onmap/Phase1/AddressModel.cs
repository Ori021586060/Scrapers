using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.OnmapDto
{
    public class AddressModel
    {
        public AddressLocationModel location { get; set; }
        public string google_place_id { get; set; }
        public string place_id { get; set; }
        public List<string> tags { get; set; }
        public AddressLangModel fr { get; set; }
        public AddressLangModel ru { get; set; }
        public AddressLangModel he { get; set; }
        public AddressLangModel en { get; set; }
    }
}
