using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.WinWinDto
{
    public class ItemWinWinDtoModel
    {
        public string ItemId { get; set; }
        public string DateUpdate { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string StreetAddress { get; set; }
        public string Rooms { get; set; }
        public string Floor { get; set; }
        public string State { get; set; }
        public string DateEnter { get; set; }
        public string Square { get; set; }
        public string IsPartners { get; set; }
        public string AmountPayment { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string ContactName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string IsAgent { get; set; }
        public List<string> Images { get; set; }
    }
}
