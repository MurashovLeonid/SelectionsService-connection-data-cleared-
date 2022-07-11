﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Superbrands.Selection.Infrastructure;

namespace Superbrands.Selection.Infrastructure.Migrations
{
    [DbContext(typeof(SelectionDbContext))]
    [Migration("20220627152658_segmentNotNullable")]
    partial class segmentNotNullable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.HasSequence("colorModelSeq")
                .IncrementsBy(10);

            modelBuilder.HasSequence("EntityFrameworkHiLoSequence")
                .IncrementsBy(10);

            modelBuilder.HasSequence("LogSeq")
                .IncrementsBy(10);

            modelBuilder.HasSequence("NomenclatureTemplateSequence")
                .IncrementsBy(10);

            modelBuilder.HasSequence("ProcurementKeySetSequence")
                .IncrementsBy(10);

            modelBuilder.HasSequence("ProcurementSequence")
                .IncrementsBy(10);

            modelBuilder.HasSequence("SelectionPurchaseSalePointKeySeq")
                .IncrementsBy(10);

            modelBuilder.HasSequence("selectionSeq")
                .IncrementsBy(10);

            modelBuilder.Entity("Superbrands.Selection.Infrastructure.DAL.ColorModelMetaDalDto", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:HiLoSequenceName", "colorModelSeq")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SequenceHiLo);

                    b.Property<int>("ColorModelPriority")
                        .HasColumnType("integer");

                    b.Property<int>("ColorModelStatus")
                        .HasColumnType("integer");

                    b.Property<string>("ColorModelVendorCodeSbs")
                        .HasColumnType("text");

                    b.Property<string>("Currency")
                        .HasColumnType("text");

                    b.Property<string>("EntityModificationInfo")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("ModelVendorCodeSbs")
                        .HasColumnType("text");

                    b.Property<long>("SelectionId")
                        .HasColumnType("bigint");

                    b.Property<int>("SizeChartCount")
                        .HasColumnType("integer");

                    b.Property<int>("SizeChartId")
                        .HasColumnType("integer");

                    b.Property<string>("Sizes")
                        .HasColumnType("jsonb");

                    b.HasKey("Id");

                    b.HasIndex("SelectionId");

                    b.HasIndex("ColorModelVendorCodeSbs", "SelectionId", "ModelVendorCodeSbs")
                        .IsUnique();

                    b.ToTable("ColorModelMetas");
                });

            modelBuilder.Entity("Superbrands.Selection.Infrastructure.DAL.LogEntryDalDto", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:HiLoSequenceName", "LogSeq")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("EntityModificationInfo")
                        .HasColumnType("jsonb");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("SelectionId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("SelectionId");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("Superbrands.Selection.Infrastructure.DAL.NomenclatureTemplateDalDto", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:HiLoSequenceName", "NomenclatureTemplateSequence")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("EntityModificationInfo")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Parameters")
                        .HasColumnType("jsonb");

                    b.HasKey("Id");

                    b.ToTable("NomenclatureTemplates");
                });

            modelBuilder.Entity("Superbrands.Selection.Infrastructure.DAL.ProcurementDalDto", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:HiLoSequenceName", "ProcurementSequence")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("Brands")
                        .HasColumnType("jsonb");

                    b.Property<long>("CooperationId")
                        .HasColumnType("bigint");

                    b.Property<string>("CounterpartyConditions")
                        .HasColumnType("jsonb");

                    b.Property<string>("EntityModificationInfo")
                        .HasColumnType("jsonb");

                    b.Property<string>("ExternalKeyFrom1C")
                        .HasColumnType("text");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("boolean");

                    b.Property<bool?>("IsPreorder")
                        .HasColumnType("boolean");

                    b.Property<int>("Kind")
                        .HasColumnType("integer");

                    b.Property<long>("PartnerId")
                        .HasColumnType("bigint");

                    b.Property<string>("PartnerTeamMembersIds")
                        .HasColumnType("jsonb");

                    b.Property<string>("SalePoints")
                        .HasColumnType("jsonb");

                    b.Property<string>("SbsTeamMembersIds")
                        .HasColumnType("jsonb");

                    b.Property<long>("SeasonId")
                        .HasColumnType("bigint");

                    b.Property<long>("SegmentId")
                        .HasColumnType("bigint");

                    b.Property<int>("Stage")
                        .HasColumnType("integer");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ExternalKeyFrom1C")
                        .IsUnique();

                    b.ToTable("Procurements");
                });

            modelBuilder.Entity("Superbrands.Selection.Infrastructure.DAL.ProcurementKeySetDalDto", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:HiLoSequenceName", "ProcurementKeySetSequence")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SequenceHiLo);

                    b.Property<int>("BuyerId")
                        .HasColumnType("integer");

                    b.Property<string>("EntityModificationInfo")
                        .HasColumnType("jsonb");

                    b.Property<int>("FinancialPlaningCenterId")
                        .HasColumnType("integer");

                    b.Property<long>("ProcurementId")
                        .HasColumnType("bigint");

                    b.Property<int>("PurchaseKeyId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ProcurementId");

                    b.HasIndex("BuyerId", "FinancialPlaningCenterId", "ProcurementId", "PurchaseKeyId")
                        .IsUnique();

                    b.ToTable("ProcurementKeySets");
                });

            modelBuilder.Entity("Superbrands.Selection.Infrastructure.DAL.SelectionDalDto", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:HiLoSequenceName", "selectionSeq")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SequenceHiLo);

                    b.Property<long>("BuyerId")
                        .HasColumnType("bigint");

                    b.Property<string>("EntityModificationInfo")
                        .HasColumnType("jsonb");

                    b.Property<long>("ProcurementId")
                        .HasColumnType("bigint");

                    b.Property<long>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasDefaultValue(1L);

                    b.HasKey("Id");

                    b.HasIndex("ProcurementId");

                    b.ToTable("Selection");
                });

            modelBuilder.Entity("Superbrands.Selection.Infrastructure.DAL.SelectionPurchaseSalePointKeyDalDto", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:HiLoSequenceName", "SelectionPurchaseSalePointKeySeq")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("EntityModificationInfo")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<long>("PurchaseKeyId")
                        .HasColumnType("bigint");

                    b.Property<long>("SalePointId")
                        .HasColumnType("bigint");

                    b.Property<long>("SelectionId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("PurchaseKeyId");

                    b.HasIndex("SalePointId");

                    b.HasIndex("SelectionId");

                    b.HasIndex("PurchaseKeyId", "SelectionId", "SalePointId");

                    b.ToTable("SelectionPurchaseSalePointKeys");
                });

            modelBuilder.Entity("Superbrands.Selection.Infrastructure.DAL.ColorModelMetaDalDto", b =>
                {
                    b.HasOne("Superbrands.Selection.Infrastructure.DAL.SelectionDalDto", "Selection")
                        .WithMany("ColorModelMetas")
                        .HasForeignKey("SelectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Superbrands.Selection.Domain.Selections.ColorModelGroupKeys", "ColorModelGroupKeys", b1 =>
                        {
                            b1.Property<long>("ColorModelMetaDalDtoId")
                                .HasColumnType("bigint");

                            b1.Property<int>("ActivityId")
                                .HasColumnType("integer")
                                .HasColumnName("ActivityId");

                            b1.Property<int>("ActivityTypeId")
                                .HasColumnType("integer")
                                .HasColumnName("ActivityTypeId");

                            b1.Property<long>("AssortmentGroupId")
                                .HasColumnType("bigint")
                                .HasColumnName("AssortmentGroupId");

                            b1.Property<long>("BrandId")
                                .HasColumnType("bigint")
                                .HasColumnName("BrandId");

                            b1.Property<long>("PurchaseKeyId")
                                .HasColumnType("bigint")
                                .HasColumnName("PurchaseKeyId");

                            b1.Property<long>("SalePointId")
                                .HasColumnType("bigint")
                                .HasColumnName("SalePointId");

                            b1.HasKey("ColorModelMetaDalDtoId");

                            b1.ToTable("ColorModelMetas");

                            b1.WithOwner()
                                .HasForeignKey("ColorModelMetaDalDtoId");
                        });

                    b.Navigation("ColorModelGroupKeys");

                    b.Navigation("Selection");
                });

            modelBuilder.Entity("Superbrands.Selection.Infrastructure.DAL.LogEntryDalDto", b =>
                {
                    b.HasOne("Superbrands.Selection.Infrastructure.DAL.SelectionDalDto", null)
                        .WithMany("Logs")
                        .HasForeignKey("SelectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Superbrands.Selection.Infrastructure.DAL.ProcurementKeySetDalDto", b =>
                {
                    b.HasOne("Superbrands.Selection.Infrastructure.DAL.ProcurementDalDto", "Procurement")
                        .WithMany("ProcurementKeySets")
                        .HasForeignKey("ProcurementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Procurement");
                });

            modelBuilder.Entity("Superbrands.Selection.Infrastructure.DAL.SelectionDalDto", b =>
                {
                    b.HasOne("Superbrands.Selection.Infrastructure.DAL.ProcurementDalDto", "Procurement")
                        .WithMany("Selections")
                        .HasForeignKey("ProcurementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Procurement");
                });

            modelBuilder.Entity("Superbrands.Selection.Infrastructure.DAL.SelectionPurchaseSalePointKeyDalDto", b =>
                {
                    b.HasOne("Superbrands.Selection.Infrastructure.DAL.SelectionDalDto", "Selection")
                        .WithMany("SelectionPurchaseSalePointKeys")
                        .HasForeignKey("SelectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Selection");
                });

            modelBuilder.Entity("Superbrands.Selection.Infrastructure.DAL.ProcurementDalDto", b =>
                {
                    b.Navigation("ProcurementKeySets");

                    b.Navigation("Selections");
                });

            modelBuilder.Entity("Superbrands.Selection.Infrastructure.DAL.SelectionDalDto", b =>
                {
                    b.Navigation("ColorModelMetas");

                    b.Navigation("Logs");

                    b.Navigation("SelectionPurchaseSalePointKeys");
                });
#pragma warning restore 612, 618
        }
    }
}
