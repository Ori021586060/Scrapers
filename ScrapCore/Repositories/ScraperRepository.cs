using Newtonsoft.Json;
using ScraperModels.Models;
using ScrapModels.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScraperCore.Repositories
{
    public class ScraperRepository
    {
        private List<ScraperDomainModel> _srapers { get; set; }
        private readonly string storeFilename = @"scrapers.json";

        public ScraperRepository()
        {
            _initRepository();
        }

        public IEnumerable<ScraperDomainModel> Get()
        {
            return _srapers;
        }

        public ScraperDomainModel Get(EnumScrapers scraperId)
        {
            var scraper = Get().Where(x => x.Id == scraperId).Select(x => x).FirstOrDefault();

            return scraper;
        }
        
        private void _initRepository()
        {
            if (File.Exists(storeFilename))
            {
                _srapers = JsonConvert.DeserializeObject<List<ScraperDomainModel>>(File.ReadAllText(storeFilename));
            }
            else
            {
                _srapers = _initDefaultData();
                _saveChanges();
            }
        }
        private List<ScraperDomainModel> _initDefaultData()
        {
            _srapers = new List<ScraperDomainModel>() {
                    new ScraperDomainModel() {
                        Id = EnumScrapers.Airdna,
                        Name = "Airdna",
                        Description = "Collects data from a real estate site",
                        AvailableScrapeByCity = true,
                        ArchiveOrder = 30,
                        Credentials = new Credentials()
                        {
                            Username = "goodluckori@gmail.com",
                            Password = "021586060"
                        }
                    },
                    new ScraperDomainModel() {
                        Id = EnumScrapers.Onmap,
                        Name = "Onmap",
                        Description = "Collects data from a real estate site",
                        AvailableScrapeByCity = false,
                        ArchiveOrder = 1,
                        Credentials = new Credentials()
                        {
                            Username = "goodluckori@gmail.com",
                            Password = "021586060"
                        }
                    },
                    new ScraperDomainModel() {
                        Id = EnumScrapers.Yad2,
                        Name = "Yad2",
                        Description = "Collects data from a real estate site",
                        AvailableScrapeByCity = false,
                        ArchiveOrder = 2,
                        Credentials = new Credentials()
                        {
                            Username = "goodluckori@gmail.com",
                            Password = "021586060"
                        }
                    },
                    new ScraperDomainModel() {
                        Id = EnumScrapers.HomeLess,
                        Name = "HomeLess",
                        Description = "Collects data from a real estate site",
                        AvailableScrapeByCity = false,
                        ArchiveOrder = 3,
                        Credentials = new Credentials()
                        {
                            Username = "goodluckori@gmail.com",
                            Password = "021586060"
                        }
                    },
                    new ScraperDomainModel() {
                        Id = EnumScrapers.Komo,
                        Name = "Komo",
                        Description = "Collects data from a real estate site",
                        AvailableScrapeByCity = false,
                        ArchiveOrder = 4,
                        Credentials = new Credentials()
                        {
                            Username = "goodluckori@gmail.com",
                            Password = "021586060"
                        }
                    },
                    new ScraperDomainModel() {
                        Id = EnumScrapers.WinWin,
                        Name = "WinWin",
                        Description = "Collects data from a real estate site",
                        AvailableScrapeByCity = false,
                        ArchiveOrder = 3,
                        Credentials = new Credentials()
                        {
                            Username = "goodluckori@gmail.com",
                            Password = "021586060"
                        }
                    }
            };

            return _srapers;
        }
        private bool _saveChanges()
        {
            var result = true;

            File.WriteAllText(storeFilename, JsonConvert.SerializeObject(_srapers, Formatting.Indented));

            return result;
        }
    }
}
