using ScraperCore;
using ScraperModels.Models;
using ScrapModels.Models;
using System;

namespace ScraperServices.Models.HomeLess
{
    public  class ScraperHomeLessStateModel: ISpenTimeState, IState, IStateExcelService
    {
        public bool IsNew { get; set; } = false;
        public EnumScrapers TypeScraper { get; set; } = EnumScrapers.HomeLess;
        public string RootPath { get; } = "scraper-data-home-less";
        public string PagesPath { get => $"{RootPath}/pages"; }
        public string ItemsPath { get => $"{RootPath}/items"; }
        public string ListPagesFilename { get => $"{RootPath}/list-pages.json"; }
        public string ListItemsFilename { get => $"{RootPath}/list-items.json"; }
        public string ListItemsDublicatesFilename { get => $"{RootPath}/list-items-dublicates.json"; }
        public string ConfigFilename { get; } = "scraper-config-home-less.json";
        public string ExcelFilename { get => $"{RootPath}/home-less-scrape-all-cities.xlsx"; }
        public string LogFilename { get => $"{RootPath}/scraper-home-less.log"; }
        public string LogStatFilename { get => $"{RootPath}/scraper-home-less.state"; }
        public string StatusFilename { get => $"{RootPath}/status.json"; }
        public string WorkPhase { get; set; }
        public int UsedSelenoidService { get; set; }
        public DateTime DateStart { get; }
        public TimeSpan SpentTime { get => DateTime.UtcNow - DateStart; }
        public ScraperHomeLessStateModel()
        {
            DateStart = DateTime.UtcNow;
        }
    }
}