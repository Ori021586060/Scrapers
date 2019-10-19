using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.KomoDto
{
    public class DataPageDtoModel
    {
        public string Updated { get; set; }
        public string Price { get; set; }
        public string ContactName { get; set; }
        public string Minisite { get; set; }
        public string Description { get; set; }
        public List<string> ExtDescription { get; set; }
        public string Title { get; set; }
        public string Rooms { get; set; }
        public string Floor { get; set; }
        public string Square { get; set; }
        public string CheckHour { get; set; }
        public string GooglePlaceId { get; set; }
        public List<string> Images { get; set; }
    }
}
