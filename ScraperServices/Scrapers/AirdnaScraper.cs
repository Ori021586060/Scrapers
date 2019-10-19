using Flurl.Http;
using ScraperCore.Repositories;
using ScraperModels.Models;
using ScraperServices.Models;
using ScraperModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScraperServices.Scrapers
{
    public class AirdnaScraper
    {
        public Credentials Credentials { get; set; }
        private EnumScrapers _scraperId { get => EnumScrapers.Airdna; }
        private ScraperRepository _scraperRepository { get; set; } = new ScraperRepository();
        private CityRepository _cityRepository { get; set; } = new CityRepository();
        public AirdnaScraper()
        {
            _initScraper();
        }
        public DataScrapeModel Scrape(int cityId)
        {
            var loginUrl = "https://www.airdna.co/api/v1/account/login";
            var loginPost = loginUrl
                .PostUrlEncodedAsync(new { username = Credentials.Username, password = Credentials.Password, remember_me = "true" })
                .ReceiveJson<ResponseLogin>();

            var loginResult = loginPost.Result;

            if (loginResult.Status == null || loginResult.Status.ToLower() != "success")
            {
                //Console.WriteLine($"Error authorizations on airdna.co site.");
            }

            var token = loginResult.Token;
            var city = _cityRepository.Get().Where(x=>x.Id==cityId).FirstOrDefault();

            var url = $"https://api.airdna.co/v1/market/property_list?access_token={token}&city_id={city.Airdna.CityId}";

            var data = url.GetJsonAsync<AirdnaScrapeDataModel>().Result;

            var result = new DataScrapeModel() {
                Scraper = _scraperId,
                Data = data
            };

            return result;
        }
        private void _initScraper()
        {
            Credentials = _scraperRepository.Get().Where(x => x.Id == _scraperId).Select(x=>x.Credentials).FirstOrDefault();
        }
    }
}
