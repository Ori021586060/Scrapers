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

            foreach (var item in items)
            {
                var itemDb = new AdItemWinWinDbModel().FromDomain(item);
                //_context.DataWinWin.Add(itemDb);
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
