﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ScraperModels.Models.Db;
using System;
using System.Collections.Generic;
using System.IO;
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
        public DbSet<AdItemAirdnaDbModel> DataAirdna { get; set; }

        public ScrapersContext()
        {
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");
            modelBuilder.Entity<AdItemHomeLessDbModel>()
                .Property(u => u.geom)
                .HasColumnType("geometry (point)");

            modelBuilder.Entity<AdItemKomoDbModel>().Property(u => u.geom).HasColumnType("geometry (point)");

            modelBuilder.Entity<AdItemOnmapDbModel>().Property(u => u.geom).HasColumnType("geometry (point)");

            modelBuilder.Entity<AdItemYad2DbModel>().Property(u => u.geom).HasColumnType("geometry (point)");

            modelBuilder.Entity<AdItemWinWinDbModel>().Property(u => u.geom).HasColumnType("geometry (point)");

            modelBuilder.Entity<AdItemAirdnaDbModel>()
                .Property(u => u.geom)
                .HasColumnType("geometry (point)");
                //.HasComputedColumnSql("ST_SetSRID(ST_MakePoint(public.\"DataAirdna.Longitude\", public.\"DataAirdna.Latitude\"), 4326)");

            modelBuilder.Entity<AdItemYad2DbModel>().ToTable("DataYad2", schema: "public");
            modelBuilder.Entity<AdItemWinWinDbModel>().ToTable("DataWinWin", schema: "public");
            modelBuilder.Entity<AdItemHomeLessDbModel>().ToTable("DataHomeLess", schema: "public");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");

            var config = builder.Build();
            string connectionString = config.GetConnectionString("DefaultConnection");

            optionsBuilder.UseNpgsql(connectionString, o => o.UseNetTopologySuite());
            //optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=dev1-sonar;Username=dev1-sonar;Password=Zx123456");
        }
    }
}
