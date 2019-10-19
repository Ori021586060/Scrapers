using ScraperModels.Models;
using System;

namespace ScraperModels.Models
{
    public interface IStateExcelService
    {
        EnumScrapers TypeScraper { get; set; }
        string ExcelFilename { get; }
        string WorkPhase { get; set; }
        TimeSpan SpentTime { get; }
    }
}