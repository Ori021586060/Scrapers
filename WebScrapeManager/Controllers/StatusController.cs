using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ScraperCore.Repositories;
using ScraperModels.Models;
using ScraperServices.Models;
using ScraperServices.Models.Yad2;
using ScrapModels.Models;
using WebScraperManager.ViewModels;

namespace WebScraperManager.Controllers
{
    public class StatusController : Controller
    {
        private ScraperRepository _scraperRepository { get; set; } = new ScraperRepository();
        private readonly ILogger<StatusController> _logger;
        private IConfiguration _configuration { get; set; }

        public StatusController(ILogger<StatusController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = new StatusViewModel();

            var scrapers = _scraperRepository.Get().OrderBy(x => x.ArchiveOrder).ToList();

            foreach (var scraper in scrapers)
            {
                var enumScraper = (EnumScrapers)scraper.Id;
                model.Statuses.Add(scraper, _getStatus(enumScraper));
            }

            _logger.LogInformation($"Amount scrapers: {scrapers.Count}");

            return View(model);
        }

        private IStatus _getStatus(EnumScrapers enumScraper)
        {
            //IStatus model = null;// new ScraperYad2StatusModel();
            var dataPath = _configuration.GetSection("WebScraperManager:ScraperDataPath").Value;
            var filename = _getFilenameStatusfile(dataPath, enumScraper);

            var model = _getStatusfile(filename);

            return model;
        }

        private IStatus _getStatusfile(string filename)
        {
            IStatus result = null;
            try
            {
                _logger.LogInformation($"Read status file: {filename}");
                var json = System.IO.File.ReadAllText(filename);
                result = JsonConvert.DeserializeObject<BaseStatusModel>(json);
            }
            catch(Exception exception) {
                ;
            }

            return result;
        }

        private string _getFilenameStatusfile(string dataPath, EnumScrapers enumScraper)
        {
            var scraper = enumScraper.ToString().ToLower();

            switch (enumScraper)
            {
                case EnumScrapers.HomeLess:
                    scraper = "home-less";
                    break;

                case EnumScrapers.WinWin:
                    scraper = "win-win";
                    break;
            }

            var result = dataPath.Replace("{scraper}", scraper);

            return result;
        }
    }
}