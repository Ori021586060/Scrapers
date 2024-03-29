﻿using Newtonsoft.Json;
using ScraperRepositories.Repositories;
using ScraperServices.Models.Yad2;
using ScraperServices.Scrapers;
using ScraperServices.Services;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Update;

namespace ParseYad2
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var state = new ScraperYad2StateModel() { IsNew = true, };

            var scraper = new ScraperYad2(state);

            UpdateRepository();

            //Scrape(scraper);

            //GetExcelFile(scraper);

            //PrintSaveStatus(scraper);

            //SaveDomainModel(scraper);

            //LoadDomainModelFromFile(scraper);
        }

        private static void LoadDomainModelFromFile(ScraperYad2 scraper)
        {
            var model = scraper.GetDomainModelFromFile();
        }

        static void SaveDomainModel(ScraperYad2 scraper)
        {
            var model = scraper.GetDomainModel();

            scraper.SaveDomainModel(model);
        }

        static void UpdateRepository()
        {
            IUpdater updater = new UpdaterYad2();
            updater.Update();
        }

        static void Scrape(ScraperYad2 scraper)
        {
            var isOk = scraper.Scrape();
        }

        static void GetExcelFile(ScraperYad2 scraper)
        {
            var model = scraper.GetDomainModel();

            var excelService = scraper.GetExcelService();

            var excelData = excelService.CreateExcel(model);

            var filename = excelService.SaveToFile(excelData);
        }

        static void PrintSaveStatus(ScraperYad2 scraper)
        {
            var status = scraper.StatusWorkspace();

            scraper.PrintStatus(status);

            scraper.SaveStatus(status);
        }

        private static void _parseParams(string[] args, ScraperYad2StateModel state)
        {
            foreach(var arg in args)
            {
                switch (arg.ToLower())
                {
                    case "-new":
                        state.IsNew = true;
                        break;
                }
            }
        }
    }
}
