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
        public OnmapRepository()
        {
            _tableName = "DataOnmap";
        }

        public bool UpdateData(DataDomainModel data)
        {
            var result = true;

            Truncate();

            var items = (List<AdItemOnmapDomainModel>)data.Data;
            var index = 0;
            foreach(var item in items)
            {
                var itemDb = new AdItemOnmapDbModel().FromDomain(item);
                _context.DataOnmap.Add(itemDb);

                index++;
                if (index % 300 == 0)
                {
                    Console.WriteLine($"index={index}");
                    _context.SaveChanges();
                }
            }
            _context.SaveChanges();

            UpdateGeometry();

            return result;
        }

        //public bool Truncate()
        //{
        //    var result = Truncate("DataOnmap");

        //    return result;
        //}
    }
}
