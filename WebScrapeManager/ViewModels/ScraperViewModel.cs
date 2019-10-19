using ScraperModels.Models;
using ScraperModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebScraperManager.ViewModels
{
    public class ScraperViewModel
    {
        public ScraperViewModel(ScraperDomainModel model)
        {
            Id = (int)model.Id;
            Name = model.Name;
            Description = model.Description;
            Credentials = model.Credentials;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Credentials Credentials { get; set; }
    }
}
