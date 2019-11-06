using Microsoft.EntityFrameworkCore;
using Scheduler.Models;
using ScraperCore;
using ScraperDAL;
using ScraperModels.Models;
using ScraperServices.Models.WinWin;
using ScraperServices.Models.Yad2;
using ScraperServices.Scrapers;
using System;
using System.Linq;

namespace Update
{
    class Program
    {
        static void Main(string[] args)
        {
            IUpdater updater = default;

            if (args.Count() > 0 || 1==1)
            {
                var parsedParam = args.First().ToLower();
                //var parsedParam = "migrate";

                if (parsedParam == "migrate")
                {
                    using(var ctx=new ScrapersContext())
                    {
                        ctx.Database.Migrate();
                    }
                }
                else
                {
                    var scraper = (EnumScrapers)System.Enum.Parse(typeof(EnumScrapers), parsedParam, true);
                    switch (scraper)
                    {
                        case EnumScrapers.WinWin:
                            updater = new UpdaterWinWin();
                            break;
                        case EnumScrapers.Yad2:
                            updater = new UpdaterYad2();
                            break;
                        case EnumScrapers.HomeLess:
                            updater = new UpdaterHomeLess();
                            break;
                        case EnumScrapers.Onmap:
                            updater = new UpdaterOnmap();
                            break;
                        case EnumScrapers.Komo:
                            updater = new UpdaterKomo();
                            break;
                    }
                }

                if (updater != null) updater.Update();
            }
            else Console.WriteLine($"No params");
        }
    }
}
