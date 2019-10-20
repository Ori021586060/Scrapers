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

            Scrape(scraper);

            GetExcelFile(scraper);

            PrintSaveStatus(scraper);
        }

        static void Scrape(ScraperKomo scraper)
        {
            var isOk = scraper.Scrape().Result;
        }

        static void GetExcelFile(ScraperKomo scraper)
        {
            var dataOfScrape = scraper.GetDomainModel();

            var excelService = scraper.GetExcelService();

            var excelData = excelService.CreateExcel(dataOfScrape);

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
