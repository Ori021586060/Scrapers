using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models
{
    public class DataDomainModel
    {
        public EnumScrapers Scraper { get; set; }
        public object Data { get; set; }
    }
}
