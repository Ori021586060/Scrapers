using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.KomoDto
{
    public class StreetsDataDtoModel
    {
        public bool Scraped { get; set; } = false;
        public string num { get; set; }
        public string name { get; set; }
        public string counter { get; set; }
    }
}
