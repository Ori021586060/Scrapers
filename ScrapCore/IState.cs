using ScrapModels.Models;
using System;

namespace ScraperCore
{
    public interface IState
    {
        bool IsNew { get; set; }
        string RootPath { get; }
        string PagesPath { get; }
        string ItemsPath { get; }
        EnumScrapers TypeScraper { get; set; }
        string WorkPhase { get; set; }
        string LogFilename { get; }
        string LogStatFilename { get; }
        string ExcelFilename { get; }
        string ConfigFilename { get; }
        string StatusFilename { get; }
        int UsedSelenoidService { get; set; }
        TimeSpan SpentTime { get; }
    }
}