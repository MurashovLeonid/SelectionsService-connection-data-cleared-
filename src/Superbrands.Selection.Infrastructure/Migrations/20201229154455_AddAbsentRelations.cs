using Microsoft.EntityFrameworkCore.Migrations;

namespace Superbrands.Selection.Infrastructure.Migrations
{
    public partial class AddAbsentRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ColorModelMetas_Selection_SelectionDalDtoId",
                table: "ColorModelMetas");

            migrationBuilder.DropIndex(
                name: "IX_ColorModelMetas_SelectionDalDtoId",
                table: "ColorModelMetas");

            migrationBuilder.DropColumn(
                name: "SelectionDalDtoId",
                table: "ColorModelMetas");

            migrationBuilder.CreateIndex(
                name: "IX_ColorModelMetas_SelectionId",
                table: "ColorModelMetas",
                column: "SelectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ColorModelMetas_Selection_SelectionId",
                table: "ColorModelMetas",
                column: "SelectionId",
                principalTable: "Selection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ColorModelMetas_Selection_SelectionId",
                table: "ColorModelMetas");

            migrationBuilder.DropIndex(
                name: "IX_ColorModelMetas_SelectionId",
                table: "ColorModelMetas");

            migrationBuilder.AddColumn<long>(
                name: "SelectionDalDtoId",
                table: "ColorModelMetas",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ColorModelMetas_SelectionDalDtoId",
                table: "ColorModelMetas",
                column: "SelectionDalDtoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ColorModelMetas_Selection_SelectionDalDtoId",
                table: "ColorModelMetas",
                column: "SelectionDalDtoId",
                principalTable: "Selection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
