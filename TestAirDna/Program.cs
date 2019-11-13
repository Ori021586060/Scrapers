using Npgsql;
using ScraperServices.Models.Airdna;
using ScraperServices.Models.HomeLess;
using ScraperServices.Scrapers;
using System;
using System.Globalization;
using System.Threading;
using Update;

namespace TestAirDna
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            NpgsqlConnection.GlobalTypeMapper.UseNetTopologySuite();

            var state = new ScraperAirdnaStateModel() { IsNew = true, };

            var scraper = new ScraperAirdna(state);

            UpdateRepository();

            //Scrape(scraper);

            //GetExcelFile(scraper);

            //PrintSaveStatus(scraper);
        }

        static void UpdateRepository()
        {
            IUpdater updater = new UpdaterAirdna();
            updater.Update();
        }

        static void Scrape(ScraperAirdna scraper)
        {
            var isOk = scraper.Scrape();
        }

        static void GetExcelFile(ScraperAirdna scraper)
        {
            var model = scraper.GetDomainModel();

            var excelService = scraper.GetExcelService();

            var excelData = excelService.CreateExcel(model);

            var filename = excelService.SaveToFile(excelData);
        }
    }
}
