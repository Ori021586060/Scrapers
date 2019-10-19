using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScraperCore.Repositories;
using ScrapModels.Models;
using WebScraperManager.Models;
using WebScraperManager.ViewModels;

namespace WebScraperManager.Controllers
{
    public class ArchiveController : Controller
    {
        private ArchiveRepository _archiveRepository { get; set; } = new ArchiveRepository();
        private ScraperRepository _scraperRepository { get; set; } = new ScraperRepository();
        public IActionResult Index()
        {
            var model = new ArchiveViewModel();

            var scrapers = _scraperRepository.Get().OrderBy(x=>x.ArchiveOrder).ToList();
            
            Func<FileInfo, TimeSpan> timeLeft = (f) => (DateTime.UtcNow - f.CreationTimeUtc);
            Func<FileInfo, EnumStateDateFile> stateDateFile = (f) => timeLeft(f) < TimeSpan.FromDays(1) ? EnumStateDateFile.Success : EnumStateDateFile.Info;

            foreach (var scraper in scrapers)
            {
                var enumScraper = (EnumScrapers)scraper.Id;

                var list = _archiveRepository.GetFiles(enumScraper);
                if (list.Count() > 0)
                {
                    var scraperModel = _scraperRepository.Get(enumScraper);
                    model.Archive.Add(scraperModel, list.Select(x=>new ArchiveFileViewModel()
                            {
                                FileName = x.Name,
                                StateDateFile = stateDateFile(x),
                                TimeLeft = timeLeft(x),
                    }
                        )
                        .Where(x=>!x.FileName.Contains("-latest"))
                        .ToList());

                    model.LatestFiles.Add(scraperModel,list.Select(x => new ArchiveFileViewModel()
                            {
                                FileName = x.Name,
                                StateDateFile = stateDateFile(x),
                                TimeLeft = timeLeft(x),
                            }
                        )
                        .Where(x => x.FileName.Contains("-latest"))
                        .ToList());
                }
            }

            return View(model);
        }
    }
}