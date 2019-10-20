﻿using ScraperServices.Models.WinWin;
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

            var state = new ScraperWinWinStateModel() { IsNew = true, };

            var scraper = new ScraperWinWin(state);

            Scrape(scraper);

            GetExcelFile(scraper);

            PrintSaveStatus(scraper);
        }

        static void Scrape(ScraperWinWin scraper)
        {
            var isOk = scraper.Scrape().Result;
        }

        static void GetExcelFile(ScraperWinWin scraper)
        {
            var dataOfScrape = scraper.GetDomainModel();

            var excelService = scraper.GetExcelService();

            var excelData = excelService.CreateExcel(dataOfScrape);

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
