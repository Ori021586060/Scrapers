using Microsoft.EntityFrameworkCore;
using ScraperModels.Models.Db;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperDAL
{
    public class ScrapersContext : DbContext
    {
        public DbSet<AdItemHomeLessDbModel> DataHomeLess { get; set; }
        public DbSet<AdItemKomoDbModel> DataKomo { get; set; }
        public DbSet<AdItemOnmapDbModel> DataOnmap { get; set; }
        public DbSet<AdItemYad2DbModel> DataYad2 { get; set; }
        public DbSet<AdItemWinWinDbModel> DataWinWin { get; set; }

        public ScrapersContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<AdItemYad2DbModel>().ToTable("DataYad2", schema: "public");
            //modelBuilder.Entity<AdItemWinWinDbModel>().ToTable("DataWinWin", schema: "public");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=dev1-sonar;Username=dev1-sonar;Password=Zx123456");
        }
    }
}
