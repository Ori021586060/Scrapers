using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using Flurl.Http;
using Newtonsoft.Json;
using ScraperServices.Models.Onmap;
using ScraperServices.Scrapers;
using ScraperServices.Services;

namespace ParseOnmap
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var state = new ScraperOnmapStateModel() { IsNew = false, };

            var scraper = new ScraperOnmap(state);

            //Scrape(scraper);

            GetExcelFile(scraper);

            PrintSaveStatus(scraper);
        }

        static void Scrape(ScraperOnmap scraper)
        {
            var isOk = scraper.Scrape();
        }

        static void GetExcelFile(ScraperOnmap scraper)
        {
            var model = scraper.GetDomainModel();

            var excelService = scraper.GetExcelService();

            var excelData = excelService.CreateExcel(model);

            var filename = excelService.SaveToFile(excelData);
        }

        static void PrintSaveStatus(ScraperOnmap scraper)
        {
            var status = scraper.StatusWorkspace();

            scraper.PrintStatus(status);

            scraper.SaveStatus(status);
        }
    }
}
