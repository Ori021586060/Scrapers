using ScraperModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.Models
{
    public class AppConfig
    {
        public EnumScrapers? Scraper { get; set; } = null;
        public EnumJobs? Job { get; set; } = null;
        public bool IsValid { get => Scraper != null; }
    }
}
