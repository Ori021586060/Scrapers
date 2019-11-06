using ScraperServices.Models.HomeLess;
using ScraperServices.Models.Komo;
using ScraperServices.Models.Onmap;
using ScraperServices.Models.WinWin;
using ScraperServices.Models.Yad2;
using ScraperServices.Scrapers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Update
{
    public class UpdaterKomo: IUpdater
    {
        public void Update()
        {
            var scraper = new ScraperKomo(new ScraperKomoStateModel());

            var repository = scraper.GetRepository();
            var model = scraper.GetDomainModel();
            var isOk = repository.UpdateData(model);
        }
    }
}
