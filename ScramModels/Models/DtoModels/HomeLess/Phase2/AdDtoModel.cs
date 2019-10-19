using ScrapModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.HomeLess
{
    public class AdDtoModel
    {
        public EnumTypeItems TypeItem { get; set; }
        public string Id { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Price { get; set; }
        public string DateUpdated { get; set; }
        public bool DownloadedItem { get; set; } = false;
    }
}
