using ScraperModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models
{
    public class ScraperDomainModel
    {
        public EnumScrapers Id { get; set; }
        public bool AvailableScrapeByCity { get; set; } = false;
        public string Name { get; set; }
        public string Description { get; set; }
        public int ArchiveOrder { get; set; } = 1;
        public Credentials Credentials { get; set; }
    }
}
