using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ScraperRepositories.Migrations
{
    public partial class Onmap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "DataOnmap",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
            //        AdItemId = table.Column<string>(nullable: true),
            //        DateCreate = table.Column<string>(nullable: true),
            //        DateUpdate = table.Column<string>(nullable: true),
            //        EnCity = table.Column<string>(nullable: true),
            //        EnHouseNumber = table.Column<string>(nullable: true),
            //        EnNeighborhood = table.Column<string>(nullable: true),
            //        EnStreetName = table.Column<string>(nullable: true),
            //        HeCity = table.Column<string>(nullable: true),
            //        HeHouseNumber = table.Column<string>(nullable: true),
            //        HeNeighborhood = table.Column<string>(nullable: true),
            //        HeStreetName = table.Column<string>(nullable: true),
            //        Latitude = table.Column<string>(nullable: true),
            //        Longitude = table.Column<string>(nullable: true),
            //        AriaBase = table.Column<string>(nullable: true),
            //        Balconies = table.Column<string>(nullable: true),
            //        Bathrooms = table.Column<string>(nullable: true),
            //        Elevators = table.Column<string>(nullable: true),
            //        FloorOn = table.Column<string>(nullable: true),
            //        FloorOf = table.Column<string>(nullable: true),
            //        Rooms = table.Column<string>(nullable: true),
            //        Toilets = table.Column<string>(nullable: true),
            //        ContactEmail = table.Column<string>(nullable: true),
            //        ContactName = table.Column<string>(nullable: true),
            //        ContactPhone = table.Column<string>(nullable: true),
            //        Description = table.Column<string>(nullable: true),
            //        Price = table.Column<string>(nullable: true),
            //        PropertyType = table.Column<string>(nullable: true),
            //        Section = table.Column<string>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_DataOnmap", x => x.Id);
            //    });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataOnmap");
        }
    }
}
