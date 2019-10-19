using ScraperModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebScraperManager.ViewModels
{
    public class ArchiveViewModel
    {
        public Dictionary<ScraperDomainModel,List<ArchiveFileViewModel>> LatestFiles { get; set; } = new Dictionary<ScraperDomainModel,List<ArchiveFileViewModel>>();
        public Dictionary<ScraperDomainModel, List<ArchiveFileViewModel>> Archive { get; set; } = new Dictionary<ScraperDomainModel, List<ArchiveFileViewModel>>();
    }
}
