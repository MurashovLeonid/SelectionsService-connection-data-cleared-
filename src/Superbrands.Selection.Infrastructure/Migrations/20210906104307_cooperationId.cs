using Microsoft.EntityFrameworkCore.Migrations;

namespace Superbrands.Selection.Infrastructure.Migrations
{
    public partial class cooperationId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CooperationId",
                table: "Procurements",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CooperationId",
                table: "Procurements");
        }
    }
}
