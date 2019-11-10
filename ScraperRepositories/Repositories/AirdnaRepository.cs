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
    public class AirdnaRepository : RepositoryBase
    {
        public bool UpdateData(DataDomainModel data)
        {
            var result = true;

            Truncate();

            var items = (List<AdItemAirdnaDomainModel>)data.Data;
            Console.WriteLine($"Need update items: {items.Count()}");
            var index = 0;
            foreach(var item in items)
            {
                if (item.IsValidForDbModel)
                {
                    var itemDb = new AdItemAirdnaDbModel().FromDomain(item);
                    _context.DataAirdna.Add(itemDb);

                    index++;
                    if (index % 300 == 0)
                    {
                        Console.WriteLine($"index={index}");
                        _context.SaveChanges();
                    }
                }
            }
            _context.SaveChanges();

            return result;
        }

        public bool Truncate()
        {
            var result = Truncate("DataAirdna");

            return result;
        }
    }
}
