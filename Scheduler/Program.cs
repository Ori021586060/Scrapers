using Scheduler.Models;
using ScraperServices.Services;
using ScrapModels.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Scheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var config = _parseParams(args);

            if (config.IsValid)
            {
                var schedulerService = new SchedulerService();
                switch (config.Scraper)
                {
                    case EnumScrapers.Onmap:
                        switch (config.Job)
                        {
                            case EnumJobs.ScrapeNewThenSaveStore:
                                schedulerService.OnmapScrapeNewThenSaveStore();
                                break;

                            case EnumJobs.ScrapeContinueThenSaveStore:
                                schedulerService.OnmapScrapeContinueThenSaveStore();
                                break;

                            case EnumJobs.GenerateExcelThenSaveStore:
                                schedulerService.OnmapGenerateExcelThenSaveStore();
                                break;

                            case EnumJobs.StatusWorkspace:
                            default:
                                schedulerService.OnmapStatusWorkspace();
                                break;
                        }
                        break;

                    case EnumScrapers.Yad2:
                        switch (config.Job)
                        {
                            case EnumJobs.ScrapeNewThenSaveStore:
                                schedulerService.Yad2ScrapeNewThenSaveStore();
                                break;

                            case EnumJobs.ScrapeContinueThenSaveStore:
                                schedulerService.Yad2ScrapeContinueThenSaveStore();
                                break;

                            case EnumJobs.GenerateExcelThenSaveStore:
                                schedulerService.Yad2GenerateExcelThenSaveStore();
                                break;

                            case EnumJobs.StatusWorkspace:
                            default:
                                schedulerService.Yad2StatusWorkspace();
                                break;
                        }
                        break;

                    case EnumScrapers.HomeLess:
                        switch (config.Job)
                        {
                            case EnumJobs.ScrapeNewThenSaveStore:
                                schedulerService.HomeLessScrapeNewThenSaveStore();
                                break;

                            case EnumJobs.ScrapeContinueThenSaveStore:
                                schedulerService.HomeLessScrapeContinueThenSaveStore();
                                break;

                            case EnumJobs.GenerateExcelThenSaveStore:
                                schedulerService.HomeLessGenerateExcelThenSaveStore();
                                break;

                            case EnumJobs.StatusWorkspace:
                            default:
                                schedulerService.HomeLessStatusWorkspace();
                                break;
                        }
                        break;

                    case EnumScrapers.Komo:
                        switch (config.Job)
                        {
                            case EnumJobs.ScrapeNewThenSaveStore:
                                schedulerService.KomoScrapeNewThenSaveStore();
                                break;

                            case EnumJobs.ScrapeContinueThenSaveStore:
                                schedulerService.KomoScrapeContinueThenSaveStore();
                                break;

                            case EnumJobs.GenerateExcelThenSaveStore:
                                schedulerService.KomoGenerateExcelThenSaveStore();
                                break;

                            case EnumJobs.StatusWorkspace:
                            default:
                                schedulerService.KomoStatusWorkspace();
                                break;
                        }
                        break;

                    case EnumScrapers.WinWin:
                        switch (config.Job)
                        {
                            case EnumJobs.ScrapeNewThenSaveStore:
                                schedulerService.WinWinScrapeNewThenSaveStore();
                                break;

                            case EnumJobs.ScrapeContinueThenSaveStore:
                                schedulerService.WinWinScrapeContinueThenSaveStore();
                                break;

                            case EnumJobs.GenerateExcelThenSaveStore:
                                schedulerService.WinWinGenerateExcelThenSaveStore();
                                break;

                            case EnumJobs.StatusWorkspace:
                            default:
                                schedulerService.WinWinStatusWorkspace();
                                break;
                        }
                        break;

                    default:
                        Console.WriteLine($"Unknown method");
                        break;
                }
            }
            else
                _showHelp();
        }

        private static void _showHelp()
        {
            Console.WriteLine($"Use application:");
            Console.WriteLine($"scheduler -scraper scraper_name [-job job_name]");
            Console.WriteLine();
            Console.WriteLine($"scrapers:");

            foreach(var scraper in Enum.GetValues(typeof(EnumScrapers)))
                Console.WriteLine($"\t{scraper}");

            Console.WriteLine($"\njobs:");

            var jobs = Enum.GetValues(typeof(EnumJobs));
            foreach (var job in jobs)
                Console.WriteLine($"\t{job}");

            Console.WriteLine($"\nDefault job: {jobs.GetValue(0)}");

            Console.WriteLine();
        }

        private static AppConfig _parseParams(string[] args)
        {
            var result = new AppConfig();
            var i = 1;
            var val = "";

            foreach(var arg in args)
            {
                switch (arg.ToLower())
                {
                    case "-scraper":
                        val = _getNextValue(i, args)?.ToLower();
                        if (!string.IsNullOrEmpty(val))
                        {
                            try
                            {
                                result.Scraper = (EnumScrapers)System.Enum.Parse(typeof(EnumScrapers), val, true);
                            }
                            catch { }
                        }
                        break;

                    case "-job":
                        val = _getNextValue(i, args)?.ToLower();
                        if (!string.IsNullOrEmpty(val))
                        {
                            try
                            {
                                result.Job = (EnumJobs)System.Enum.Parse(typeof(EnumJobs), val, true);
                            }
                            catch { }
                        }
                        break;
                }

                i++;
            }

            return result;
        }

        private static string _getNextValue(int i, string[] args)
        {
            var result = "";

            if (args.Length > i) result = args[i];

            return result;
        }
    }
}
