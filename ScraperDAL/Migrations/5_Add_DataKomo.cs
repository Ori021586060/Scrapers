using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ScraperRepositories.Migrations
{
    public partial class Komo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "DataKomo",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
            //        AdItemId = table.Column<string>(nullable: true),
            //        Updated = table.Column<string>(nullable: true),
            //        Latitude = table.Column<string>(nullable: true),
            //        Longitude = table.Column<string>(nullable: true),
            //        ContactName = table.Column<string>(nullable: true),
            //        Phone1 = table.Column<string>(nullable: true),
            //        Phone2 = table.Column<string>(nullable: true),
            //        Description = table.Column<string>(nullable: true),
            //        Price = table.Column<string>(nullable: true),
            //        PropertyType = table.Column<string>(nullable: true),
            //        Rooms = table.Column<string>(nullable: true),
            //        Floor = table.Column<string>(nullable: true),
            //        Square = table.Column<string>(nullable: true),
            //        CheckHour = table.Column<string>(nullable: true),
            //        Extras = table.Column<string>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_DataKomo", x => x.Id);
            //    });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataKomo");
        }
    }
}
