using Newtonsoft.Json;
using ScraperDAL;
using ScraperModels.Models;
using ScraperModels.Models.Db;
using ScraperModels.Models.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using Update;

namespace TestUpdateContext
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start update context");

            IUpdater updater;

            updater = new UpdaterYad2();
            updater.Update();

            updater = new UpdaterWinWin();
            updater.Update();

            updater = new UpdaterHomeLess();
            updater.Update();

            updater = new UpdaterOnmap();
            updater.Update();

            //updater = new UpdaterKomo();
            //updater.Update();

            Console.WriteLine($"Done");
        }

        private static DataDomainModel<AdItemYad2DomainModel> LoadDomainModel()
        {
            var filename = "domain-model.json";
            var model = JsonConvert.DeserializeObject<DataDomainModel<AdItemYad2DomainModel>>(File.ReadAllText(filename));

            return model;
        }
    }
}
