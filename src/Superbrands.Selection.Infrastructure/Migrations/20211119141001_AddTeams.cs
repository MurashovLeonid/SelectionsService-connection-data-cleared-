using Microsoft.EntityFrameworkCore.Migrations;

namespace Superbrands.Selection.Infrastructure.Migrations
{
    public partial class AddTeams : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PartnerTeamMembersIds",
                table: "Procurements",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SbsTeamMembersIds",
                table: "Procurements",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartnerTeamMembersIds",
                table: "Procurements");

            migrationBuilder.DropColumn(
                name: "SbsTeamMembersIds",
                table: "Procurements");
        }
    }
}
