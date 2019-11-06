using ScraperModels.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ScraperModels.Models.Db;
using ScraperModels.Models.Domain;
using System.Linq;

namespace ScraperRepositories.Repositories
{
    public class Yad2Repository: RepositoryBase
    {
        public bool UpdateData(DataDomainModel data)
        {
            var result = true;

            Truncate();

            var items = (List<AdItemYad2DomainModel>)data.Data;
            var index = 0;
            foreach(var item in items)
            {
                var itemDb = new AdItemYad2DbModel().FromDomain(item);
                _context.DataYad2.Add(itemDb);

                index++;
                if (index % 100 == 0)
                {
                    Console.WriteLine($"index={index}");
                    _context.SaveChanges();
                }
            }
            _context.SaveChanges();

            return result;
        }

        public bool Truncate()
        {
            var result = Truncate("DataYad2");

            return result;
        }
    }
}
