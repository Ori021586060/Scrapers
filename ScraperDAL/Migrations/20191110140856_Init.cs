using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ScraperDAL.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "DataAirdna",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Geometry = table.Column<byte[]>(type: "geometry (point)", nullable: true),
                    AdItemId = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    Longitude = table.Column<double>(nullable: false),
                    Latitude = table.Column<double>(nullable: false),
                    Adr = table.Column<double>(nullable: true),
                    Rating = table.Column<double>(nullable: true),
                    Bathrooms = table.Column<float>(nullable: true),
                    Bedrooms = table.Column<float>(nullable: true),
                    Accommodates = table.Column<string>(nullable: true),
                    Revenue = table.Column<string>(nullable: true),
                    PropertyType = table.Column<string>(nullable: true),
                    Occ = table.Column<string>(nullable: true),
                    Reviews = table.Column<float>(nullable: true),
                    RoomType = table.Column<string>(nullable: true),
                    LinkToProfile = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataAirdna", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataKomo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdItemId = table.Column<string>(nullable: true),
                    Updated = table.Column<string>(nullable: true),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    ContactName = table.Column<string>(nullable: true),
                    Phone1 = table.Column<string>(nullable: true),
                    Phone2 = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<float>(nullable: true),
                    PropertyType = table.Column<string>(nullable: true),
                    Rooms = table.Column<int>(nullable: true),
                    Floor = table.Column<int>(nullable: true),
                    Square = table.Column<int>(nullable: true),
                    CheckHour = table.Column<string>(nullable: true),
                    Extras = table.Column<string>(nullable: true),
                    Images = table.Column<List<string>>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataKomo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataOnmap",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdItemId = table.Column<string>(nullable: true),
                    DateCreate = table.Column<string>(nullable: true),
                    DateUpdate = table.Column<string>(nullable: true),
                    EnCity = table.Column<string>(nullable: true),
                    EnHouseNumber = table.Column<string>(nullable: true),
                    EnNeighborhood = table.Column<string>(nullable: true),
                    EnStreetName = table.Column<string>(nullable: true),
                    HeCity = table.Column<string>(nullable: true),
                    HeHouseNumber = table.Column<string>(nullable: true),
                    HeNeighborhood = table.Column<string>(nullable: true),
                    HeStreetName = table.Column<string>(nullable: true),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    AriaBase = table.Column<float>(nullable: true),
                    Balconies = table.Column<int>(nullable: true),
                    Bathrooms = table.Column<string>(nullable: true),
                    Elevators = table.Column<string>(nullable: true),
                    FloorOn = table.Column<int>(nullable: true),
                    FloorOf = table.Column<int>(nullable: true),
                    Rooms = table.Column<int>(nullable: true),
                    Toilets = table.Column<string>(nullable: true),
                    ContactEmail = table.Column<string>(nullable: true),
                    ContactName = table.Column<string>(nullable: true),
                    ContactPhone = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<float>(nullable: true),
                    PropertyType = table.Column<string>(nullable: true),
                    Section = table.Column<string>(nullable: true),
                    Images = table.Column<List<string>>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataOnmap", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataHomeLess",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdItemId = table.Column<string>(nullable: true),
                    DateUpdated = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Region = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    Images = table.Column<List<string>>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    WindowBars = table.Column<string>(nullable: true),
                    RoomMatesAllow = table.Column<string>(nullable: true),
                    Furnitures = table.Column<string>(nullable: true),
                    Elevator = table.Column<string>(nullable: true),
                    Balcony = table.Column<string>(nullable: true),
                    Parking = table.Column<string>(nullable: true),
                    Conditioner = table.Column<string>(nullable: true),
                    Size = table.Column<float>(nullable: true),
                    Floor = table.Column<int>(nullable: true),
                    Contact = table.Column<string>(nullable: true),
                    Phone1 = table.Column<string>(nullable: true),
                    Phone2 = table.Column<string>(nullable: true),
                    AgencyName = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Price = table.Column<float>(nullable: true),
                    LinkToProfile = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataHomeLess", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataWinWin",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdItemId = table.Column<string>(nullable: true),
                    DateUpdate = table.Column<string>(nullable: true),
                    Longitude = table.Column<double>(nullable: true),
                    Latitude = table.Column<double>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Area = table.Column<string>(nullable: true),
                    StreetAddress = table.Column<string>(nullable: true),
                    Rooms = table.Column<float>(nullable: true),
                    Floor = table.Column<int>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    DateEnter = table.Column<string>(nullable: true),
                    Square = table.Column<string>(nullable: true),
                    IsPartners = table.Column<string>(nullable: true),
                    AmountPayment = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<float>(nullable: true),
                    IsAgent = table.Column<string>(nullable: true),
                    ContactName = table.Column<string>(nullable: true),
                    Phone1 = table.Column<string>(nullable: true),
                    Phone2 = table.Column<string>(nullable: true),
                    Images = table.Column<List<string>>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataWinWin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataYad2",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdItemId = table.Column<string>(nullable: true),
                    DateCreate = table.Column<string>(nullable: true),
                    DateUpdate = table.Column<string>(nullable: true),
                    HeCity = table.Column<string>(nullable: true),
                    HouseNumber = table.Column<int>(nullable: true),
                    Neighborhood = table.Column<string>(nullable: true),
                    StreetName = table.Column<string>(nullable: true),
                    Latitude = table.Column<double>(nullable: true),
                    Longitude = table.Column<double>(nullable: true),
                    AriaBase = table.Column<int>(nullable: true),
                    Balconies = table.Column<int>(nullable: true),
                    Pets = table.Column<string>(nullable: true),
                    Elevators = table.Column<string>(nullable: true),
                    FloorOn = table.Column<int>(nullable: true),
                    FloorOf = table.Column<int>(nullable: true),
                    Rooms = table.Column<float>(nullable: true),
                    Parking = table.Column<string>(nullable: true),
                    ContactName = table.Column<string>(nullable: true),
                    ContactPhone = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<float>(nullable: true),
                    PropertyType = table.Column<string>(nullable: true),
                    AirConditioner = table.Column<string>(nullable: true),
                    Images = table.Column<List<string>>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataYad2", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataAirdna");

            migrationBuilder.DropTable(
                name: "DataKomo");

            migrationBuilder.DropTable(
                name: "DataOnmap");

            migrationBuilder.DropTable(
                name: "DataHomeLess",
                schema: "public");

            migrationBuilder.DropTable(
                name: "DataWinWin",
                schema: "public");

            migrationBuilder.DropTable(
                name: "DataYad2",
                schema: "public");
        }
    }
}
