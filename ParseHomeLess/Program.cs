using ScraperServices.Models.HomeLess;
using ScraperServices.Scrapers;
using ScraperServices.Services;
using System;
using System.Globalization;
using System.Threading;

namespace ParseHomeLess
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var state = new ScraperHomeLessStateModel() { IsNew= false, };

            var scraper = new ScraperHomeLess(state);

            Scrape(scraper);

            GetExcelFile(scraper);

            PrintSaveStatus(scraper);
        }

        static void Scrape(ScraperHomeLess scraper)
        {
            var isOk = scraper.Scrape().Result;
        }

        static void GetExcelFile(ScraperHomeLess scraper)
        {
            var dataOfScrape = scraper.GetDomainModel();

            var excelService = scraper.GetExcelService();

            var excelData = excelService.CreateExcel(dataOfScrape);

            var filename = excelService.SaveToFile(excelData);
        }

        static void PrintSaveStatus(ScraperHomeLess scraper)
        {
            var status = scraper.StatusWorkspace();

            scraper.PrintStatus(status);

            scraper.SaveStatus(status);
        }
    }
}
