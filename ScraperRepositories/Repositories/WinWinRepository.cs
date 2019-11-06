using ScraperModels.Models;
using ScraperModels.Models.Db;
using ScraperModels.Models.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperRepositories.Repositories
{
    public class WinWinRepository: RepositoryBase
    {
        public bool UpdateData(DataDomainModel data)
        {
            var result = true;

            Truncate();

            var items = (List<AdItemWinWinDomainModel>)data.Data;
            var index = 0;
            foreach (var item in items)
            {
                var itemDb = new AdItemWinWinDbModel().FromDomain(item);
                _context.DataWinWin.Add(itemDb);
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
            var result = Truncate("DataWinWin");

            return result;
        }
    }
}
