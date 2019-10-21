using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ScraperDAL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initialization dbContext");

            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");

            var config = builder.Build();
            string connectionString = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<ScrapersContext>();
            var options = optionsBuilder
                .UseNpgsql(connectionString)
                .Options;

            using (var context = new ScrapersContext())
            {
                var item = context.DataYad2.FirstOrDefaultAsync();
                context.DataYad2.Add(new ScraperModels.Models.Db.AdItemYad2DbModel());
                context.SaveChanges();
            }
        }
    }
}
