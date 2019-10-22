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

            using (var context = new ScrapersContext())
            {
                context.Database.Migrate();

                Console.WriteLine($"Migration accepted");
            }

            Console.WriteLine($"Done");
        }
    }
}
