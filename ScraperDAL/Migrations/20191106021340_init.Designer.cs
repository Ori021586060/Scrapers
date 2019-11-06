﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ScraperDAL;

namespace ScraperDAL.Migrations
{
    [DbContext(typeof(ScrapersContext))]
    [Migration("20191106021340_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("ScraperModels.Models.Db.AdItemHomeLessDbModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("AdItemId")
                        .HasColumnType("text");

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("AgencyName")
                        .HasColumnType("text");

                    b.Property<string>("Balcony")
                        .HasColumnType("text");

                    b.Property<string>("City")
                        .HasColumnType("text");

                    b.Property<string>("Conditioner")
                        .HasColumnType("text");

                    b.Property<string>("Contact")
                        .HasColumnType("text");

                    b.Property<string>("DateUpdated")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Elevator")
                        .HasColumnType("text");

                    b.Property<int?>("Floor")
                        .HasColumnType("integer");

                    b.Property<string>("Furnitures")
                        .HasColumnType("text");

                    b.Property<List<string>>("Images")
                        .HasColumnType("text[]");

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<string>("LinkToProfile")
                        .HasColumnType("text");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<string>("Parking")
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<string>("Phone1")
                        .HasColumnType("text");

                    b.Property<string>("Phone2")
                        .HasColumnType("text");

                    b.Property<float?>("Price")
                        .HasColumnType("real");

                    b.Property<string>("Region")
                        .HasColumnType("text");

                    b.Property<string>("RoomMatesAllow")
                        .HasColumnType("text");

                    b.Property<float?>("Size")
                        .HasColumnType("real");

                    b.Property<string>("WindowBars")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DataHomeLess","public");
                });

            modelBuilder.Entity("ScraperModels.Models.Db.AdItemKomoDbModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("AdItemId")
                        .HasColumnType("text");

                    b.Property<string>("CheckHour")
                        .HasColumnType("text");

                    b.Property<string>("ContactName")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Extras")
                        .HasColumnType("text");

                    b.Property<int?>("Floor")
                        .HasColumnType("integer");

                    b.Property<List<string>>("Images")
                        .HasColumnType("text[]");

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<string>("Phone1")
                        .HasColumnType("text");

                    b.Property<string>("Phone2")
                        .HasColumnType("text");

                    b.Property<float?>("Price")
                        .HasColumnType("real");

                    b.Property<string>("PropertyType")
                        .HasColumnType("text");

                    b.Property<int?>("Rooms")
                        .HasColumnType("integer");

                    b.Property<int?>("Square")
                        .HasColumnType("integer");

                    b.Property<string>("Updated")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DataKomo");
                });

            modelBuilder.Entity("ScraperModels.Models.Db.AdItemOnmapDbModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("AdItemId")
                        .HasColumnType("text");

                    b.Property<float?>("AriaBase")
                        .HasColumnType("real");

                    b.Property<int?>("Balconies")
                        .HasColumnType("integer");

                    b.Property<string>("Bathrooms")
                        .HasColumnType("text");

                    b.Property<string>("ContactEmail")
                        .HasColumnType("text");

                    b.Property<string>("ContactName")
                        .HasColumnType("text");

                    b.Property<string>("ContactPhone")
                        .HasColumnType("text");

                    b.Property<string>("DateCreate")
                        .HasColumnType("text");

                    b.Property<string>("DateUpdate")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Elevators")
                        .HasColumnType("text");

                    b.Property<string>("EnCity")
                        .HasColumnType("text");

                    b.Property<string>("EnHouseNumber")
                        .HasColumnType("text");

                    b.Property<string>("EnNeighborhood")
                        .HasColumnType("text");

                    b.Property<string>("EnStreetName")
                        .HasColumnType("text");

                    b.Property<int?>("FloorOf")
                        .HasColumnType("integer");

                    b.Property<int?>("FloorOn")
                        .HasColumnType("integer");

                    b.Property<string>("HeCity")
                        .HasColumnType("text");

                    b.Property<string>("HeHouseNumber")
                        .HasColumnType("text");

                    b.Property<string>("HeNeighborhood")
                        .HasColumnType("text");

                    b.Property<string>("HeStreetName")
                        .HasColumnType("text");

                    b.Property<List<string>>("Images")
                        .HasColumnType("text[]");

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<float?>("Price")
                        .HasColumnType("real");

                    b.Property<string>("PropertyType")
                        .HasColumnType("text");

                    b.Property<int?>("Rooms")
                        .HasColumnType("integer");

                    b.Property<string>("Section")
                        .HasColumnType("text");

                    b.Property<string>("Toilets")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DataOnmap");
                });

            modelBuilder.Entity("ScraperModels.Models.Db.AdItemWinWinDbModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("AdItemId")
                        .HasColumnType("text");

                    b.Property<string>("AmountPayment")
                        .HasColumnType("text");

                    b.Property<string>("Area")
                        .HasColumnType("text");

                    b.Property<string>("City")
                        .HasColumnType("text");

                    b.Property<string>("ContactName")
                        .HasColumnType("text");

                    b.Property<string>("DateEnter")
                        .HasColumnType("text");

                    b.Property<string>("DateUpdate")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int?>("Floor")
                        .HasColumnType("integer");

                    b.Property<List<string>>("Images")
                        .HasColumnType("text[]");

                    b.Property<string>("IsAgent")
                        .HasColumnType("text");

                    b.Property<string>("IsPartners")
                        .HasColumnType("text");

                    b.Property<double?>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double?>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<string>("Phone1")
                        .HasColumnType("text");

                    b.Property<string>("Phone2")
                        .HasColumnType("text");

                    b.Property<float?>("Price")
                        .HasColumnType("real");

                    b.Property<float?>("Rooms")
                        .HasColumnType("real");

                    b.Property<string>("Square")
                        .HasColumnType("text");

                    b.Property<string>("State")
                        .HasColumnType("text");

                    b.Property<string>("StreetAddress")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DataWinWin","public");
                });

            modelBuilder.Entity("ScraperModels.Models.Db.AdItemYad2DbModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("AdItemId")
                        .HasColumnType("text");

                    b.Property<string>("AirConditioner")
                        .HasColumnType("text");

                    b.Property<int?>("AriaBase")
                        .HasColumnType("integer");

                    b.Property<int?>("Balconies")
                        .HasColumnType("integer");

                    b.Property<string>("ContactName")
                        .HasColumnType("text");

                    b.Property<string>("ContactPhone")
                        .HasColumnType("text");

                    b.Property<string>("DateCreate")
                        .HasColumnType("text");

                    b.Property<string>("DateUpdate")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Elevators")
                        .HasColumnType("text");

                    b.Property<int?>("FloorOf")
                        .HasColumnType("integer");

                    b.Property<int?>("FloorOn")
                        .HasColumnType("integer");

                    b.Property<string>("HeCity")
                        .HasColumnType("text");

                    b.Property<int?>("HouseNumber")
                        .HasColumnType("integer");

                    b.Property<List<string>>("Images")
                        .HasColumnType("text[]");

                    b.Property<double?>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double?>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<string>("Neighborhood")
                        .HasColumnType("text");

                    b.Property<string>("Parking")
                        .HasColumnType("text");

                    b.Property<string>("Pets")
                        .HasColumnType("text");

                    b.Property<float?>("Price")
                        .HasColumnType("real");

                    b.Property<string>("PropertyType")
                        .HasColumnType("text");

                    b.Property<float?>("Rooms")
                        .HasColumnType("real");

                    b.Property<string>("StreetName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DataYad2","public");
                });
#pragma warning restore 612, 618
        }
    }
}
