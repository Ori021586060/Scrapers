using ScrapModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebScraperManager.DtoModels
{
    public class ScraperDto
    {
        public EnumScrapers Id { get; set; }
        public bool Checked { get; set; }
    }
}
