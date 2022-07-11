using Microsoft.EntityFrameworkCore.Migrations;

namespace Superbrands.Selection.Infrastructure.Migrations
{
    public partial class FixSeasonCapsuleId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeasonCapsuleId",
                table: "Procurements");

            migrationBuilder.AddColumn<long>(
                name: "SeasonCapsuleId",
                table: "Procurements",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeasonCapsuleId",
                table: "Procurements");

            migrationBuilder.AddColumn<long>(
                name: "SeasonCapsuleId",
                table: "Procurements",
                type: "text",
                nullable: true);
        }
    }
}
