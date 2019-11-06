using ScraperServices.Models.WinWin;
using ScraperServices.Models.Yad2;
using ScraperServices.Scrapers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Update
{
    public class UpdaterYad2: IUpdater
    {
        public void Update()
        {
            var scraper = new ScraperYad2(new ScraperYad2StateModel());

            var repository = scraper.GetRepository();
            var model = scraper.GetDomainModel();
            var isOk = repository.UpdateData(model);
        }
    }
}
