using ScraperCore;
using ScraperModels.Models;
using ScraperServices.Services;
using ScrapModels.Models;
using System;

namespace ScraperServices.Models.WinWin
{
    public  class ScraperWinWinStateModel: IState, ISpenTimeState, IStateExcelService
    {
        public bool IsNew { get; set; } = false;
        public EnumScrapers TypeScraper { get; set; } = EnumScrapers.WinWin;
        public string RootPath { get; } = "scraper-data-win-win";
        //public string RegionsPath { get => $"{RootPath}/regions"; }
        public string PagesPath { get => $"{RootPath}/pages"; }
        public string ItemsPath { get => $"{RootPath}/items"; }
        public string ListRegionsFilename { get => $"{RootPath}/list-regions.json"; }
        public string ListPagesFilename { get => $"{RootPath}/list-pages.json"; }
        public string ListItemsFilename { get => $"{RootPath}/list-items.json"; }
        public string ListItemsDublicatesFilename { get => $"{RootPath}/list-items-dublicates.json"; }
        public string ConfigFilename { get; } = "scraper-config-win-win.json";
        public string ExcelFilename { get => $"{RootPath}/win-win-scrape-all-cities.xlsx"; }
        public string StatusFilename { get => $"{RootPath}/status.json"; }
        public string WorkPhase { get; set; }
        public string LogFilename { get => $"{RootPath}/scraper-win-win.log"; }
        public string LogStatFilename { get => $"{RootPath}/scraper-win-win-state.log"; }
        public string LogErrorFilename { get => $"{RootPath}/scraper-win-win-error.log"; }
        public int UsedSelenoidService { get; set; }
        public DateTime DateStart { get; }
        public TimeSpan SpentTime { get => DateTime.UtcNow - DateStart; }
        // ext-test
        public int Bad { get; set; }

        public ScraperWinWinStateModel()
        {
            DateStart = DateTime.UtcNow;
        }
    }
}