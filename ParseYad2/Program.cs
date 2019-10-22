using Newtonsoft.Json;
using ScraperRepositories.Repositories;
using ScraperServices.Models.Yad2;
using ScraperServices.Scrapers;
using ScraperServices.Services;
using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace ParseYad2
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var state = new ScraperYad2StateModel() { IsNew = true, };

            var scraper = new ScraperYad2(state);

            //UpdateRepository(scraper);

            //Scrape(scraper);

            GetExcelFile(scraper);

            PrintSaveStatus(scraper);

            //SaveDomainModel(scraper);
        }

        static void SaveDomainModel(ScraperYad2 scraper)
        {
            var model = scraper.GetDomainModel();

            var filename = "domain-model.json";
            File.WriteAllText(filename, JsonConvert.SerializeObject(model, Formatting.Indented));
        }

        static void UpdateRepository(ScraperYad2 scraper)
        {
            var repository = scraper.GetRepository();

            var model = scraper.GetDomainModel();

            var isOk = repository.UpdateData(model);
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
