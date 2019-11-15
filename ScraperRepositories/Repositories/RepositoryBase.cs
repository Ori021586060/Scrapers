using Microsoft.EntityFrameworkCore;
using ScraperDAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperRepositories.Repositories
{
    public class RepositoryBase
    {
        protected ScrapersContext _context { get; set; } = new ScrapersContext();
        protected string _tableName { get; set; }

        protected void UpdateGeometry()
        {
            var sql = $"call updateloc('public.\"{_tableName}\"')";

            Console.WriteLine($"call sql:{sql}");

            _context.Database.ExecuteSqlCommand(sql);
        }

        protected bool Truncate()
        {
            var sql = $"TRUNCATE TABLE public.\"{_tableName}\"";

            Console.WriteLine($"call sql:{sql}");

            _context.Database.ExecuteSqlCommand(sql);

            return true;
        }
    }
}
