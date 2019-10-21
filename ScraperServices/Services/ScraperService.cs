using ScraperCore.Repositories;
using ScraperModels.Models;
using ScraperServices.Scrapers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScraperServices.Services
{
    public class ScraperService
    {
        private ScraperRepository _ScraperRepository { get; set; } = new ScraperRepository();
        public List<ScraperDomainModel> Get()
        {
            var scrapers = _ScraperRepository.Get().ToList();

            return scrapers;
        }

        public DataDomainModel GetDataFromScraper(RequestDataScrape request)
        {
            var result = new DataDomainModel();

            switch (request.ScrapeId)
            {
                case EnumScrapers.Airdna:
                    var scraper = new AirdnaScraper();

                    result = scraper.Scrape(request.CityId);

                    break;
            }

            return result;
        }
    }
}
