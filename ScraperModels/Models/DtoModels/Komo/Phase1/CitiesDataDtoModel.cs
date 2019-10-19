using System;

namespace ScraperModels.Models.KomoDto
{
    public class CitiesDataDtoModel
    {
        public bool Scraped { get; set; } = false;
        public string num { get; set; }
        public string name { get; set; }
        public string counter { get; set; }
    }
}