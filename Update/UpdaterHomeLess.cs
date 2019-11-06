using ScraperServices.Models.HomeLess;
using ScraperServices.Models.WinWin;
using ScraperServices.Models.Yad2;
using ScraperServices.Scrapers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Update
{
    public class UpdaterHomeLess: IUpdater
    {
        public void Update()
        {
            var scraper = new ScraperHomeLess(new ScraperHomeLessStateModel());

            var repository = scraper.GetRepository();
            var model = scraper.GetDomainModel();
            var isOk = repository.UpdateData(model);
        }
    }
}
