using ScraperServices.Models.Komo;
using ScraperServices.Scrapers;
using ScraperServices.Services;
using System;
using System.Globalization;
using System.Threading;

namespace ParseKomo
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var state = new ScraperKomoStateModel() { IsNew = true, };

            var scraper = new ScraperKomo(state);

            //UpdateRepository(scraper);

            //Scrape(scraper);

            GetExcelFile(scraper);

            PrintSaveStatus(scraper);
        }

        static void UpdateRepository(ScraperKomo scraper)
        {
            var repository = scraper.GetRepository();

            var model = scraper.GetDomainModel();

            var isOk = repository.UpdateData(model);
        }

        static void Scrape(ScraperKomo scraper)
        {
            var isOk = scraper.Scrape();
        }

        static void GetExcelFile(ScraperKomo scraper)
        {
            var model = scraper.GetDomainModel();

            var excelService = scraper.GetExcelService();

            var excelData = excelService.CreateExcel(model);

            var filename = excelService.SaveToFile(excelData);
        }

        static void PrintSaveStatus(ScraperKomo scraper)
        {
            var status = scraper.StatusWorkspace();

            scraper.PrintStatus(status);

            scraper.SaveStatus(status);
        }
    }
}
