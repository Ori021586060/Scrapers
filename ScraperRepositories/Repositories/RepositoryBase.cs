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

        protected bool Truncate(string table)
        {
            var sql = $"TRUNCATE TABLE public.\"{table}\"";
            _context.Database.ExecuteSqlCommand(sql);

            return true;
        }
    }
}
