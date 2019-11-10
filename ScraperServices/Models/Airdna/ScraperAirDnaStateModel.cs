using ScraperCore;
using ScraperModels.Models;
using System;

namespace ScraperServices.Models.Airdna
{
    public  class ScraperAirdnaStateModel : ISpenTimeState, IState, IStateExcelService
    {
        public bool IsNew { get; set; } = false;
        public EnumScrapers TypeScraper { get; set; } = EnumScrapers.Airdna;
        public string RootPath { get; } = "scraper-data-airdna";
        public string PagesPath { get => $"{RootPath}/pages"; }
        public string ItemsPath { get => $"{RootPath}/items"; }
        public string ListPagesFilename { get => $"{RootPath}/list-cities.json"; }
        public string ListItemsFilename { get => $"{RootPath}/list-items.json"; }
        public string ListItemsDublicatesFilename { get => $"{RootPath}/list-items-dublicates.json"; }
        public string ConfigFilename { get; } = "scraper-config-airdna.json";
        public string ExcelFilename { get => $"{RootPath}/airdna-scrape-all-cities.xlsx"; }
        public string LogFilename { get => $"{RootPath}/scraper-airdns.log"; }
        public string LogStatFilename { get => $"{RootPath}/scraper-airdna.state"; }
        public string StatusFilename { get => $"{RootPath}/status.json"; }
        public string WorkPhase { get; set; }
        public int UsedSelenoidService { get; set; }
        public DateTime DateStart { get; }
        public TimeSpan SpentTime { get => DateTime.UtcNow - DateStart; }
        public ScraperAirdnaStateModel()
        {
            DateStart = DateTime.UtcNow;
        }
    }
}