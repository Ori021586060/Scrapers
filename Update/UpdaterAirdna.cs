using ScraperServices.Models.Airdna;
using ScraperServices.Scrapers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Update
{
    public class UpdaterAirdna: IUpdater
    {
        public void Update()
        {
            var scraper = new ScraperAirdna(new ScraperAirdnaStateModel());

            var repository = scraper.GetRepository();
            var model = scraper.GetDomainModel();
            var isOk = repository.UpdateData(model);
        }
    }
}
