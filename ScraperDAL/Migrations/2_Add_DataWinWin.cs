using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ScraperRepositories.Migrations
{
    public partial class WinWin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "DataWinWin",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
            //        AdItemId = table.Column<string>(nullable: true),
            //        DateUpdate = table.Column<string>(nullable: true),
            //        Longitude = table.Column<string>(nullable: true),
            //        Latitude = table.Column<string>(nullable: true),
            //        City = table.Column<string>(nullable: true),
            //        Area = table.Column<string>(nullable: true),
            //        StreetAddress = table.Column<string>(nullable: true),
            //        Rooms = table.Column<string>(nullable: true),
            //        Floor = table.Column<string>(nullable: true),
            //        State = table.Column<string>(nullable: true),
            //        DateEnter = table.Column<string>(nullable: true),
            //        Square = table.Column<string>(nullable: true),
            //        IsPartners = table.Column<string>(nullable: true),
            //        AmountPayment = table.Column<string>(nullable: true),
            //        Description = table.Column<string>(nullable: true),
            //        Price = table.Column<string>(nullable: true),
            //        IsAgent = table.Column<string>(nullable: true),
            //        ContactName = table.Column<string>(nullable: true),
            //        Phone1 = table.Column<string>(nullable: true),
            //        Phone2 = table.Column<string>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_DataWinWin", x => x.Id);
            //    });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataWinWin");
        }
    }
}
