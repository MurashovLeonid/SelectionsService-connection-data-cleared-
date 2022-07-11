using Microsoft.EntityFrameworkCore.Migrations;

namespace Superbrands.Selection.Infrastructure.Migrations
{
    public partial class ChangedColorModelMetaToFlatProductsStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ColorModelMetas_ColorModelId_SelectionId_ModelId",
                table: "ColorModelMetas");

            migrationBuilder.DropColumn(
                name: "ColorModelId",
                table: "ColorModelMetas");

            migrationBuilder.DropColumn(
                name: "ModelId",
                table: "ColorModelMetas");

            migrationBuilder.AddColumn<string>(
                name: "ColorModelVendorCodeSbs",
                table: "ColorModelMetas",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModelVendorCodeSbs",
                table: "ColorModelMetas",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ColorModelMetas_ColorModelVendorCodeSbs_SelectionId_ModelVe~",
                table: "ColorModelMetas",
                columns: new[] { "ColorModelVendorCodeSbs", "SelectionId", "ModelVendorCodeSbs" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ColorModelMetas_ColorModelVendorCodeSbs_SelectionId_ModelVe~",
                table: "ColorModelMetas");

            migrationBuilder.DropColumn(
                name: "ColorModelVendorCodeSbs",
                table: "ColorModelMetas");

            migrationBuilder.DropColumn(
                name: "ModelVendorCodeSbs",
                table: "ColorModelMetas");

            migrationBuilder.AddColumn<int>(
                name: "ColorModelId",
                table: "ColorModelMetas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "ModelId",
                table: "ColorModelMetas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ColorModelMetas_ColorModelId_SelectionId_ModelId",
                table: "ColorModelMetas",
                columns: new[] { "ColorModelId", "SelectionId", "ModelId" },
                unique: true);
        }
    }
}
