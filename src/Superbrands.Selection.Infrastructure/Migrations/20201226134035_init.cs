using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Superbrands.Selection.Infrastructure.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "colorModelSeq",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "EntityFrameworkHiLoSequence",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "LogSeq",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "NomenclatureTemplateSequence",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "ProcurementKeySetSequence",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "ProcurementSequence",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "SelectionPurchaseSalePointKeySeq",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "selectionSeq",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "NomenclatureTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SequenceHiLo),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Parameters = table.Column<string>(type: "jsonb", nullable: true),
                    EntityModificationInfo = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NomenclatureTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Procurements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SequenceHiLo),
                    SeasonCapsuleId = table.Column<string>(type: "text", nullable: true),
                    ExternalKeyFrom1C = table.Column<string>(type: "text", nullable: true),
                    PartnerId = table.Column<int>(type: "integer", nullable: false),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    Stage = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SalePoints = table.Column<string>(type: "jsonb", nullable: true),
                    BrandIds = table.Column<string>(type: "jsonb", nullable: true),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    EntityModificationInfo = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procurements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcurementKeySets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SequenceHiLo),
                    ProcurementId = table.Column<long>(type: "bigint", nullable: false),
                    BuyerId = table.Column<int>(type: "integer", nullable: false),
                    PurchaseKeyId = table.Column<int>(type: "integer", nullable: false),
                    FinancialPlaningCenterId = table.Column<int>(type: "integer", nullable: false),
                    EntityModificationInfo = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcurementKeySets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcurementKeySets_Procurements_ProcurementId",
                        column: x => x.ProcurementId,
                        principalTable: "Procurements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Selection",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SequenceHiLo),
                    Status = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    BuyerId = table.Column<long>(type: "bigint", nullable: false),
                    PartnerId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ProcurementId = table.Column<long>(type: "bigint", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    EntityModificationInfo = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Selection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Selection_Procurements_ProcurementId",
                        column: x => x.ProcurementId,
                        principalTable: "Procurements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ColorModelMetas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SequenceHiLo),
                    ModelId = table.Column<long>(type: "bigint", nullable: false),
                    SelectionId = table.Column<long>(type: "bigint", nullable: false),
                    ColorModelId = table.Column<int>(type: "integer", nullable: false),
                    Sizes = table.Column<string>(type: "jsonb", nullable: true),
                    SizeChartId = table.Column<int>(type: "integer", nullable: false),
                    ColorModelStatus = table.Column<int>(type: "integer", nullable: false),
                    ColorModelPriority = table.Column<int>(type: "integer", nullable: false),
                    SizeChartCount = table.Column<int>(type: "integer", nullable: false),
                    SalePointId = table.Column<int>(type: "integer", nullable: true),
                    PurchaseKeyId = table.Column<int>(type: "integer", nullable: true),
                    AssortmentGroupId = table.Column<int>(type: "integer", nullable: true),
                    BrandId = table.Column<int>(type: "integer", nullable: true),
                    ActivityId = table.Column<int>(type: "integer", nullable: true),
                    ActivityTypeId = table.Column<int>(type: "integer", nullable: true),
                    SelectionDalDtoId = table.Column<long>(type: "bigint", nullable: true),
                    EntityModificationInfo = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorModelMetas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColorModelMetas_Selection_SelectionDalDtoId",
                        column: x => x.SelectionDalDtoId,
                        principalTable: "Selection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SequenceHiLo),
                    SelectionId = table.Column<long>(type: "bigint", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    EntityModificationInfo = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logs_Selection_SelectionId",
                        column: x => x.SelectionId,
                        principalTable: "Selection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SelectionPurchaseSalePointKeys",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SequenceHiLo),
                    SelectionId = table.Column<long>(type: "bigint", nullable: false),
                    SalePointId = table.Column<int>(type: "integer", nullable: false),
                    PurchaseKeyId = table.Column<int>(type: "integer", nullable: false),
                    EntityModificationInfo = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectionPurchaseSalePointKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SelectionPurchaseSalePointKeys_Selection_SelectionId",
                        column: x => x.SelectionId,
                        principalTable: "Selection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ColorModelMetas_ColorModelId_SelectionId_ModelId",
                table: "ColorModelMetas",
                columns: new[] { "ColorModelId", "SelectionId", "ModelId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ColorModelMetas_SelectionDalDtoId",
                table: "ColorModelMetas",
                column: "SelectionDalDtoId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_SelectionId",
                table: "Logs",
                column: "SelectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementKeySets_BuyerId_FinancialPlaningCenterId_Procure~",
                table: "ProcurementKeySets",
                columns: new[] { "BuyerId", "FinancialPlaningCenterId", "ProcurementId", "PurchaseKeyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementKeySets_ProcurementId",
                table: "ProcurementKeySets",
                column: "ProcurementId");

            migrationBuilder.CreateIndex(
                name: "IX_Procurements_ExternalKeyFrom1C",
                table: "Procurements",
                column: "ExternalKeyFrom1C",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Selection_ProcurementId",
                table: "Selection",
                column: "ProcurementId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectionPurchaseSalePointKeys_PurchaseKeyId",
                table: "SelectionPurchaseSalePointKeys",
                column: "PurchaseKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectionPurchaseSalePointKeys_PurchaseKeyId_SelectionId_Sa~",
                table: "SelectionPurchaseSalePointKeys",
                columns: new[] { "PurchaseKeyId", "SelectionId", "SalePointId" });

            migrationBuilder.CreateIndex(
                name: "IX_SelectionPurchaseSalePointKeys_SalePointId",
                table: "SelectionPurchaseSalePointKeys",
                column: "SalePointId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectionPurchaseSalePointKeys_SelectionId",
                table: "SelectionPurchaseSalePointKeys",
                column: "SelectionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColorModelMetas");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "NomenclatureTemplates");

            migrationBuilder.DropTable(
                name: "ProcurementKeySets");

            migrationBuilder.DropTable(
                name: "SelectionPurchaseSalePointKeys");

            migrationBuilder.DropTable(
                name: "Selection");

            migrationBuilder.DropTable(
                name: "Procurements");

            migrationBuilder.DropSequence(
                name: "colorModelSeq");

            migrationBuilder.DropSequence(
                name: "EntityFrameworkHiLoSequence");

            migrationBuilder.DropSequence(
                name: "LogSeq");

            migrationBuilder.DropSequence(
                name: "NomenclatureTemplateSequence");

            migrationBuilder.DropSequence(
                name: "ProcurementKeySetSequence");

            migrationBuilder.DropSequence(
                name: "ProcurementSequence");

            migrationBuilder.DropSequence(
                name: "SelectionPurchaseSalePointKeySeq");

            migrationBuilder.DropSequence(
                name: "selectionSeq");
        }
    }
}
