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
    public class HomeLessRepository : RepositoryBase
    {
        public bool UpdateData(DataScrapeModel data)
        {
            var result = true;

            Truncate();

            var items = (List<AdItemHomeLessDomainModel>)data.Data;

            foreach(var item in items)
            {
                var itemDb = new AdItemHomeLessDbModel().FromDomain(item);
                //_context.DataHomeLess.Add(itemDb);
            }
            _context.SaveChanges();

            return result;
        }

        public bool Truncate()
        {
            var result = Truncate("DataHomeLess");

            return result;
        }
    }
}
