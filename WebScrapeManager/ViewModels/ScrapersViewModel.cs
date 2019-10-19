using Microsoft.AspNetCore.Mvc.Rendering;
using ScraperModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebScraperManager.ViewModels
{
    public class ScrapersViewModel
    {
        public List<ScraperViewModel> Scrapers { get; set; } = new List<ScraperViewModel>();
        public Dictionary<int, string> Cities { get; set; } = new Dictionary<int, string>();
    }
}
