using ScraperCore;
using ScraperModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperServices.Models.Yad2
{
    public class ScraperYad2StateModel: ISpenTimeState, IState, IStateExcelService
    {
        public bool IsNew { get; set; } = false;
        public EnumScrapers TypeScraper { get; set; } = EnumScrapers.Yad2;
        public string RootPath { get; } = "scraper-data-yad2";
        public string ConfigFilename { get; } = "scraper-config-yad2.json";
        public string LogFilename { get => $"{RootPath}/scraper-yad2.log"; }
        // privates
        private string FolderPreLoads = "pre-loads";
        private string FolderItems = "items";
        private string FolderItemsContacts = "items-contacts";
        private string FilenameListPreLoads = "list-pre-loads.json";
        private string FilenameListItems = "list-items.json";
        private string FilenameListItemsContacts = "list-items-contacts.json";

        // privates
        public string PagesPath { get => PathPreLoads; }
        public string PathPreLoads { get => $"{RootPath}/{FolderPreLoads}"; }
        public string ItemsPath { get => $"{RootPath}/{FolderItems}"; }
        public string PathItemsContacts { get => $"{RootPath}/{FolderItemsContacts}"; }
        public string PathListPreLoads { get => $"{RootPath}/{FilenameListPreLoads}"; }
        public string PathListItems { get => $"{RootPath}/{FilenameListItems}"; }
        public string PathListItemsContacts { get => $"{RootPath}/{FilenameListItemsContacts}"; }

        public string ExcelFilename { get => $"{RootPath}/yad2-scrape-all-cities.xlsx"; }
        public string LogStatFilename { get; set; }
        public string StatusFilename { get => $"{RootPath}/status.json"; }
        public int UsedSelenoidService { get; set; }
        public string ListItemsContactsDublicatesFilename { get => $"{RootPath}/list-items-contacts-dublicates.json"; }
        public string ListItemsDublicatesFilename { get => $"{RootPath}/list-items-dublicates.json"; }
        public string WorkPhase { get; set; }
        public DateTime DateStart { get; }
        public TimeSpan SpentTime { get => DateTime.UtcNow - DateStart; }
        // Extended params
        public int CountScrapers { get; set; } = 100;
        public int CountWaitSecondForFailRequest { get; set; } = 2;
        public ScraperYad2StateModel()
        {
            DateStart = DateTime.UtcNow;
        }
    }
}
