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

    public class DataDomainModel<T>
    {
        public EnumScrapers Scraper { get; set; }
        public List<T> Data { get; set; }
    }
}
