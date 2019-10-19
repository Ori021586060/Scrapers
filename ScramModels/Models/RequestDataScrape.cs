using System;
using System.Collections.Generic;
using System.Text;

namespace ScrapModels.Models
{
    public class RequestDataScrape
    {
        public EnumScrapers ScrapeId { get; set; }
        public int CityId { get; set; }
    }
}
