using Microsoft.EntityFrameworkCore.Migrations;

namespace Superbrands.Selection.Infrastructure.Migrations
{
    public partial class newFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CooperationId",
                table: "Procurements",
                newName: "CooperationTypeId");

            migrationBuilder.AddColumn<long>(
                name: "CooperationKindId",
                table: "Procurements",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CooperationPartnerCategoryId",
                table: "Procurements",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CooperationKindId",
                table: "Procurements");

            migrationBuilder.DropColumn(
                name: "CooperationPartnerCategoryId",
                table: "Procurements");

            migrationBuilder.RenameColumn(
                name: "CooperationTypeId",
                table: "Procurements",
                newName: "CooperationId");
        }
    }
}
