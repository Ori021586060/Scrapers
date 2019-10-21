using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScraperCore.Repositories;
using ScraperModels.Models;
using ScraperServices.Services;
using ScraperModels.Models;
using WebScraperManager.DtoModels;

namespace WebScraperManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScraperController : Controller
    {
        private ScraperService _scraperService { get; set; } = new ScraperService();
        private ExcelService _excelService { get; set; } = new ExcelService();
        private CityRepository _cityRepository { get; set; } = new CityRepository();
        private ArchiveRepository _archiveRepository { get; set; } = new ArchiveRepository();

        [HttpPost]
        public JsonResult Scrape([FromBody] RequestScraperDto request)
        {
            var result = new ResponseStateModel();

            var list = new List<DataDomainModel>();

            foreach (var scraper in request.UseScrapers.Where(x=>x.Checked))
            {
                var data = _scraperService.GetDataFromScraper(new RequestDataScrape() { ScrapeId = scraper.Id, CityId = request.CityId });
                list.Add(data);
            }

            var excelData = _excelService.DataToExcel(list);
            var city = _cityRepository.Get().Where(x => x.Id == request.CityId).Select(x => x.Airdna.CityName.ToLower()).FirstOrDefault();
            var filename = $"airdna-scrape-city-{city}.xlsx";

            var pathFile = _excelService.SaveToFile(excelData, filename);

            _archiveRepository.Save(pathFile, EnumScrapers.Airdna);

            result.Payload = filename;

            return Json(result);
        }
    }
}