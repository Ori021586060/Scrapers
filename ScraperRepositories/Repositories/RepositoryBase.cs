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
            _context.Database.ExecuteSqlCommand($"TRUNCATE TABLE public.\"DataYad2\"");

            return true;
        }
    }
}
