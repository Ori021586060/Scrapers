using ScraperServices.Models.WinWin;
using ScraperServices.Scrapers;
using ScraperServices.Services;
using System;
using System.Globalization;
using System.Threading;

namespace ParseWinWin
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var state = new ScraperWinWinStateModel() { IsNew = false, };

            var scraper = new ScraperWinWin(state);

            //UpdateRepository(scraper);

            //Scrape(scraper);

            //GetExcelFile(scraper);

            //PrintSaveStatus(scraper);

            var itemDto = scraper.GetItemDtoAsync("4389448").Result;
        }

        static void UpdateRepository(ScraperWinWin scraper)
        {
            var repository = scraper.GetRepository();

            var model = scraper.GetDomainModel();

            var isOk = repository.UpdateData(model);
        }

        static void Scrape(ScraperWinWin scraper)
        {
            var isOk = scraper.Scrape();
        }

        static void GetExcelFile(ScraperWinWin scraper)
        {
            var model = scraper.GetDomainModel();

            var excelService = scraper.GetExcelService();

            var excelData = excelService.CreateExcel(model);

            var filename = excelService.SaveToFile(excelData);
        }

        static void PrintSaveStatus(ScraperWinWin scraper)
        {
            var status = scraper.StatusWorkspace();

            scraper.PrintStatus(status);

            scraper.SaveStatus(status);
        }
    }
}
