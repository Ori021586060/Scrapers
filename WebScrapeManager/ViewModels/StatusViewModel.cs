using ScraperModels.Models;
using ScraperServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebScraperManager.ViewModels
{
    public class StatusViewModel
    {
        public Dictionary<ScraperDomainModel, IStatus> Statuses { get; set; } = new Dictionary<ScraperDomainModel, IStatus>();
    }
}
