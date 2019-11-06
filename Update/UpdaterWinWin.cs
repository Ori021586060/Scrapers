using ScraperServices.Models.WinWin;
using ScraperServices.Scrapers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Update
{
    public class UpdaterWinWin: IUpdater
    {
        public void Update()
        {
            var scraper = new ScraperWinWin(new ScraperWinWinStateModel());

            var repository = scraper.GetRepository();
            var model = scraper.GetDomainModel();
            var isOk = repository.UpdateData(model);
        }
    }
}
