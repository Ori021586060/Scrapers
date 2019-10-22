using Newtonsoft.Json;
using ScraperDAL;
using ScraperModels.Models;
using ScraperModels.Models.Db;
using ScraperModels.Models.Domain;
using System;
using System.Collections.Generic;
using System.IO;

namespace TestUpdateContext
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start update context");

            var model = LoadDomainModel();
            var listItems = model.Data;

            using (var context = new ScrapersContext())
            {
                foreach(var item in listItems)
                {
                    var itemDb = new AdItemYad2DbModel().FromDomain(item);
                    context.DataYad2.Add(itemDb);
                }

                context.SaveChanges();
            }

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
