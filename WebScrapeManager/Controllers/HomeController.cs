using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScraperCore.Repositories;
using WebScraperManager.Models;
using WebScraperManager.ViewModels;

namespace WebScraperManager.Controllers
{
    public class HomeController : Controller
    {
        private CityRepository _cityRepository { get; set; } = new CityRepository();
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Configuration()
        {
            var model = new ConfigurationViewModel();
            model.Cities = _cityRepository.Get().ToList();

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
