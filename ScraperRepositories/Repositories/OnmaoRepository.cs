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
    public class OnmapRepository : RepositoryBase
    {
        public bool UpdateData(DataDomainModel data)
        {
            var result = true;

            Truncate();

            var items = (List<AdItemOnmapDomainModel>)data.Data;

            foreach(var item in items)
            {
                var itemDb = new AdItemOnmapDbModel().FromDomain(item);
                //_context.DataOnmap.Add(itemDb);
            }
            _context.SaveChanges();

            return result;
        }

        public bool Truncate()
        {
            var result = Truncate("DataOnmap");

            return result;
        }
    }
}
