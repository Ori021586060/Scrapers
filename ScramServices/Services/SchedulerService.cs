using ScraperCore;
using ScraperCore.Repositories;
using ScraperServices.Models;
using ScraperServices.Models.HomeLess;
using ScraperServices.Models.Komo;
using ScraperServices.Models.Onmap;
using ScraperServices.Models.WinWin;
using ScraperServices.Models.Yad2;
using ScraperServices.Scrapers;
using ScrapModels.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScraperServices.Services
{
    public class SchedulerService
    {
        private SchedulerStateModel _schedulerStateModel { get; set; } = new SchedulerStateModel();

        public void OnmapScrapeNewThenSaveStore()
        {
            _onmapScrapeThenSaveStore(isNew: true);
        }

        public void OnmapScrapeContinueThenSaveStore()
        {
            _onmapScrapeThenSaveStore(isNew: false);
        }

        public void OnmapGenerateExcelThenSaveStore()
        {
            _onmapScrapeThenSaveStore(needScrape: false);
        }

        private void _onmapScrapeThenSaveStore(bool isNew=false, bool needScrape=true)
        {
            _log($"Start OnmapScrapeThenSaveStore (isNew={isNew})");

            var state = new ScraperOnmapStateModel() { IsNew = isNew, };

            var scraper = new ScraperOnmap(state);

            if (needScrape) scraper.Scrape();

            var dataOfScrape = scraper.GetDomainModel();

            var excelService = new ExcelOnmapService(state);

            var excelData = excelService.CreateExcel(dataOfScrape);

            var pathToFile = excelService.SaveToFile(excelData);

            var archive = new ArchiveRepository();

            archive.Save(pathToFile, state.TypeScraper);

            _log($"End OnmapScrapeThenSaveStore (isNew={isNew}), Spent time {_calcSpentTime2String(state)}");
        }

        public void OnmapStatusWorkspace()
        {
            var scraper = new ScraperOnmap();

            var status = scraper.StatusWorkspace();
            scraper.SaveStatus(status);
            scraper.PrintStatus(status);
        }

        public void Yad2ScrapeNewThenSaveStore()
        {
            _yad2ScrapeThenSaveStore(isNew: true);
        }

        public void Yad2ScrapeContinueThenSaveStore()
        {
            _yad2ScrapeThenSaveStore(isNew: false);
        }

        public void Yad2GenerateExcelThenSaveStore()
        {
            _yad2ScrapeThenSaveStore(needScrape: false);
        }

        private void _yad2ScrapeThenSaveStore(bool isNew = false, bool needScrape = true)
        {
            _log($"Start Yad2ScrapeThenSaveStore (isNew={isNew})");

            var state = new ScraperYad2StateModel() { IsNew = isNew, };

            var scraper = new ScraperYad2(state);

            if (needScrape) scraper.Scrape();

            var dataOfScrape = scraper.GetDomainModel();

            var excelService = new ExcelYad2Service(state);

            var excelData = excelService.CreateExcel(dataOfScrape);

            var pathToFile = excelService.SaveToFile(excelData);

            var archive = new ArchiveRepository();

            archive.Save(pathToFile, state.TypeScraper);

            _log($"End Yad2ScrapeThenSaveStore (isNew={isNew}), Spent time {_calcSpentTime2String(state)}");
        }

        public void Yad2StatusWorkspace()
        {
            var scraper = new ScraperYad2();

            var status = scraper.StatusWorkspace();
            scraper.SaveStatus(status);
            scraper.PrintStatus(status);
        }

        public void HomeLessScrapeNewThenSaveStore() {
            _homeLessScrapeThenSaveStore(isNew: true);
        }

        public void HomeLessStatusWorkspace()
        {
            var scraper = new ScraperHomeLess();

            var status = scraper.StatusWorkspace();
            scraper.SaveStatus(status);
            scraper.PrintStatus(status);
        }

        public void HomeLessScrapeContinueThenSaveStore() {
            _homeLessScrapeThenSaveStore(isNew: false);
        }

        public void HomeLessGenerateExcelThenSaveStore()
        {
            _homeLessScrapeThenSaveStore(needScrape: false);
        }

        private void _homeLessScrapeThenSaveStore(bool isNew=false, bool needScrape = true)
        {
            _log($"Start HomeLessScrapeThenSaveStore (isNew={isNew})");

            var state = new ScraperHomeLessStateModel() { IsNew = isNew, };

            var scraper = new ScraperHomeLess(state);

            if (needScrape) scraper.Scrape();

            var dataOfScrape = scraper.GetDomainModel();

            var excelService = new ExcelHomeLessService(state);

            var excelData = excelService.CreateExcel(dataOfScrape);

            var pathToFile = excelService.SaveToFile(excelData);

            var archive = new ArchiveRepository();

            archive.Save(pathToFile, state.TypeScraper);

            _log($"End HomeLessScrapeThenSaveStore (isNew={isNew}), Spent time {_calcSpentTime2String(state)}");
        }

        public void WinWinScrapeNewThenSaveStore()
        {
            _winWinScrapeThenSaveStore(isNew: true);
        }

        public void WinWinScrapeContinueThenSaveStore()
        {
            _winWinScrapeThenSaveStore(isNew: false);
        }

        public void WinWinGenerateExcelThenSaveStore()
        {
            _winWinScrapeThenSaveStore(needScrape: false);
        }

        private void _winWinScrapeThenSaveStore(bool isNew = false, bool needScrape = true)
        {
            _log($"Start WinWinScrapeThenSaveStore (isNew={isNew})");

            var state = new ScraperWinWinStateModel() { IsNew = isNew, };

            var scraper = new ScraperWinWin(state);

            if (needScrape) scraper.Scrape();

            var dataOfScrape = scraper.GetDomainModel();

            var excelService = new ExcelWinWinService(state);

            var excelData = excelService.CreateExcel(dataOfScrape);

            var pathToFile = excelService.SaveToFile(excelData);

            var archive = new ArchiveRepository();

            archive.Save(pathToFile, state.TypeScraper);

            _log($"End WinWinScrapeThenSaveStore (isNew={isNew}), Spent time {_calcSpentTime2String(state)}");
        }

        public void WinWinStatusWorkspace()
        {
            var scraper = new ScraperWinWin();

            var status = scraper.StatusWorkspace();
            scraper.SaveStatus(status);
            scraper.PrintStatus(status);
        }

        public void KomoScrapeContinueThenSaveStore()
        {
            _komoScrapeThenSaveStore(isNew: false);
        }

        public void KomoScrapeNewThenSaveStore()
        {
            _komoScrapeThenSaveStore(isNew: true);
        }

        public void KomoGenerateExcelThenSaveStore()
        {
            _komoScrapeThenSaveStore(needScrape: false);
        }

        private void _komoScrapeThenSaveStore(bool isNew = false, bool needScrape = true)
        {
            _log($"Start KomoScrapeThenSaveStore (isNew={isNew})");

            var state = new ScraperKomoStateModel() { IsNew = isNew, };

            var scraper = new ScraperKomo(state);

            if (needScrape) scraper.Scrape();

            var dataOfScrape = scraper.GetDomainModel();

            var excelService = new ExcelKomoService(state);

            var excelData = excelService.CreateExcel(dataOfScrape);

            var pathToFile = excelService.SaveToFile(excelData);

            var archive = new ArchiveRepository();

            archive.Save(pathToFile, state.TypeScraper);

            _log($"End KomoScrapeThenSaveStore (isNew={isNew}), Spent time {_calcSpentTime2String(state)}");
        }

        public void KomoStatusWorkspace()
        {
            var scraper = new ScraperKomo();

            var status = scraper.StatusWorkspace();
            scraper.SaveStatus(status);
            scraper.PrintStatus(status);
        }

        private string _calcSpentTime2String(ISpenTimeState state)
        {
            var result = "";

            var time = DateTime.UtcNow - state.DateStart;
            result = $"{time.TotalHours:#.##} Hours/ {time.TotalMinutes:#.##} Mins";

            return result;
        }

        private void _log(string message)
        {
            var formatMessage = Formats.LogRow(DateTime.UtcNow, "SchedulerService", message);
            Console.WriteLine(formatMessage);

            File.AppendAllText(_schedulerStateModel.LogFilename, $"{formatMessage}\r\n");
        }
    }
}
