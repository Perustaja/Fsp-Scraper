using Microsoft.EntityFrameworkCore.Migrations;

namespace FspScraper.WebAPI.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Times",
                columns: table => new
                {
                    RegistrationNum = table.Column<string>(nullable: false),
                    Hobbs = table.Column<decimal>(nullable: true),
                    AirTime = table.Column<int>(nullable: true),
                    Tach1 = table.Column<decimal>(nullable: true),
                    Tach2 = table.Column<decimal>(nullable: true),
                    Prop1 = table.Column<decimal>(nullable: true),
                    Prop2 = table.Column<decimal>(nullable: true),
                    AircraftTotal = table.Column<decimal>(nullable: false),
                    Engine1Total = table.Column<decimal>(nullable: true),
                    Engine2Total = table.Column<decimal>(nullable: true),
                    Cycles = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Times", x => x.RegistrationNum);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Times");
        }
    }
}
