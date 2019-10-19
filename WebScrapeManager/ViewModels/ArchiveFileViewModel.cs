using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebScraperManager.Models;

namespace WebScraperManager.ViewModels
{
    public class ArchiveFileViewModel
    {
        public string FileName { get; set; }
        public EnumStateDateFile StateDateFile { get; set; }
        public TimeSpan TimeLeft { get; set; }
    }
}
