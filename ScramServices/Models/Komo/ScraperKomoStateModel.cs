using ScraperCore;
using ScraperModels.Models;
using ScrapModels.Models;
using System;

namespace ScraperServices.Models.Komo
{
    public  class ScraperKomoStateModel: IState, ISpenTimeState, IStateExcelService
    {
        public bool IsNew { get; set; } = false;
        public EnumScrapers TypeScraper { get; set; } = EnumScrapers.Komo;
        public string RootPath { get; } = "scraper-data-komo";
        public string CitiesPath { get => $"{RootPath}/cities"; }
        public string ItemsPath { get => $"{RootPath}/items"; }
        public string PagesPath { get => $"{RootPath}/pages"; }
        public string ListCitiesFilename { get => $"{RootPath}/list-cities.json"; }
        public string ListStreetsFilename { get => $"{RootPath}/list-streets.json"; }
        public string ListItemsFilename { get => $"{RootPath}/list-items.json"; }
        public string ConfigFilename { get; } = "scraper-config-komo.json";
        public string ExcelFilename { get => $"{RootPath}/komo-scrape-all-cities.xlsx"; }
        public string LogFilename { get => $"{RootPath}/scraper-komo.log"; }
        public string LogStatFilename { get => $"{RootPath}/scraper-komo-stat.log"; }
        public string StatusFilename { get => $"{RootPath}/status.json"; }
        public int TotalScrapedAds { get => _totalScrapesAds; set { lock (_lockTotalScrapedAds) { _totalScrapesAds = value; }; } }
        public string SessionSerial { get; set; }
        public string WorkPhase { get; set; }
        public DateTime DateStart { get; }
        public TimeSpan SpentTime { get => DateTime.UtcNow - DateStart; }
        public int UsedSelenoidService { get; set; }

        private static object _lockTotalScrapedAds { get; set; } = new object();
        public static object LockWriteToStatLog { get; set; } = new object();
        private int _totalScrapesAds { get; set; }

        public ScraperKomoStateModel()
        {
            DateStart = DateTime.UtcNow;
        }
    }
}