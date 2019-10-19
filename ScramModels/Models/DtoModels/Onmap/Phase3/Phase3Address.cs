using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.OnmapDto
{
    public class Phase3Address
    {
        public Phase3AddressLang en { get; set; }
        public Phase3AddressLang fr { get; set; }
        public Phase3AddressLang he { get; set; }
        public Phase3AddressLocation location { get; set; }
        public string place_id { get; set; }
        public Phase3AddressLang ru { get; set; }
        public List<string> tags { get; set; }
    }
}
