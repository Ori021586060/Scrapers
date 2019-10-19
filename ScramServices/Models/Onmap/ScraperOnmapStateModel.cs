using ScraperCore;
using ScraperModels.Models;
using ScrapModels.Models;
using System;

namespace ScraperServices.Models.Onmap
{
    public  class ScraperOnmapStateModel: IState, ISpenTimeState, IStateExcelService
    {
        public bool IsNew { get; set; } = false;
        public EnumScrapers TypeScraper { get; set; } = EnumScrapers.Onmap;
        public string RootPath { get; } = "scraper-data-onmap";
        public string PagesPath { get => $"{RootPath}/pages"; }
        public string ItemsPath { get => $"{RootPath}/items"; }
        public string Phase1Filename { get => $"{RootPath}/onmap-phase1.json"; }
        public string Phase2Filename { get => $"{RootPath}/onmap-phase2.json"; }
        //public string ListItemsFilename { get => $"{RootPath}/list-items.json"; }
        public string ConfigFilename { get; } = "scraper-config-onmap.json";
        public string ExcelFilename { get => $"{RootPath}/onmap-scrape-all-cities.xlsx"; }
        public string WorkPhase { get; set; }
        public string PathListItems { get => $"{RootPath}/list-items.json"; }
        public string LogFilename { get => $"{RootPath}/scraper-onmap.log"; }
        public string LogStatFilename { get => $"{RootPath}/scraper-onmap-state.log"; }
        public string LogErrorFilename { get => $"{RootPath}/scraper-onmap-error.log"; }
        public string StatusFilename { get => $"{RootPath}/status.json"; }
        public int UsedSelenoidService { get; set; }
        public DateTime DateStart { get; }
        public TimeSpan SpentTime { get => DateTime.UtcNow - DateStart; }
        // ext-test
        public int Bad { get; set; }

        public ScraperOnmapStateModel()
        {
            DateStart = DateTime.UtcNow;
        }
    }
}