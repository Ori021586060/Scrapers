using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ScraperCore.Repositories;
using ScraperServices.Services;
using WebScraperManager.ViewModels;

namespace WebScraperManager.Controllers
{
    public class ScrapeController : Controller
    {
        private ScraperService _scraperServices { get; set; } = new ScraperService();
        private CityRepository _cityRepository { get; set; } = new CityRepository();
        public IActionResult Index()
        {
            var model = new ScrapersViewModel()
            {
                Scrapers = _scraperServices.Get().Where(x=>x.AvailableScrapeByCity).Select(x => new ScraperViewModel(x)).ToList(),
                Cities = _cityRepository.Get().ToDictionary(k => k.Id, v => v.CityOriginalName),
            };

            return View(model);
        }
    }
}