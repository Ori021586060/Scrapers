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

            var state = new ScraperHomeLessStateModel() { IsNew= true, };

            var scraper = new ScraperHomeLess(state);

            //UpdateRepository(scraper);

            Scrape(scraper);

            //GetExcelFile(scraper);

            //PrintSaveStatus(scraper);
        }

        static void UpdateRepository(ScraperHomeLess scraper)
        {
            var repository = scraper.GetRepository();

            var model = scraper.GetDomainModel();

            var isOk = repository.UpdateData(model);
        }

        static void Scrape(ScraperHomeLess scraper)
        {
            var isOk = scraper.Scrape();
        }

        static void GetExcelFile(ScraperHomeLess scraper)
        {
            var model = scraper.GetDomainModel();

            var excelService = scraper.GetExcelService();

            var excelData = excelService.CreateExcel(model);

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
