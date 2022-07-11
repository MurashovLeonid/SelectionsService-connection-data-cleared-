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
    [Migration("20201226134035_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseHiLo("EntityFrameworkHiLoSequence")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.1");

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
                        .UseHiLo("colorModelSeq");

                    b.Property<int>("ColorModelId")
                        .HasColumnType("integer");

                    b.Property<int>("ColorModelPriority")
                        .HasColumnType("integer");

                    b.Property<int>("ColorModelStatus")
                        .HasColumnType("integer");

                    b.Property<string>("EntityModificationInfo")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<long>("ModelId")
                        .HasColumnType("bigint");

                    b.Property<long?>("SelectionDalDtoId")
                        .HasColumnType("bigint");

                    b.Property<long>("SelectionId")
                        .HasColumnType("bigint");

                    b.Property<int>("SizeChartCount")
                        .HasColumnType("integer");

                    b.Property<int>("SizeChartId")
                        .HasColumnType("integer");

                    b.Property<string>("Sizes")
                        .HasColumnType("jsonb");

                    b.HasKey("Id");

                    b.HasIndex("SelectionDalDtoId");

                    b.HasIndex("ColorModelId", "SelectionId", "ModelId")
                        .IsUnique();

                    b.ToTable("ColorModelMetas");
                });

            modelBuilder.Entity("Superbrands.Selection.Infrastructure.DAL.LogEntryDalDto", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseHiLo("LogSeq");

                    b.Property<string>("EntityModificationInfo")
                        .IsRequired()
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
                        .UseHiLo("NomenclatureTemplateSequence");

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
                        .UseHiLo("ProcurementSequence");

                    b.Property<string>("BrandIds")
                        .HasColumnType("jsonb");

                    b.Property<string>("EntityModificationInfo")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("ExternalKeyFrom1C")
                        .HasColumnType("text");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("boolean");

                    b.Property<int>("Kind")
                        .HasColumnType("integer");

                    b.Property<int>("PartnerId")
                        .HasColumnType("integer");

                    b.Property<string>("SalePoints")
                        .HasColumnType("jsonb");

                    b.Property<string>("SeasonCapsuleId")
                        .HasColumnType("text");

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
                        .UseHiLo("ProcurementKeySetSequence");

                    b.Property<int>("BuyerId")
                        .HasColumnType("integer");

                    b.Property<string>("EntityModificationInfo")
                        .IsRequired()
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
                        .UseHiLo("selectionSeq");

                    b.Property<long>("BuyerId")
                        .HasColumnType("bigint");

                    b.Property<string>("EntityModificationInfo")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true);

                    b.Property<bool>("IsDefault")
                        .HasColumnType("boolean");

                    b.Property<long>("PartnerId")
                        .HasColumnType("bigint");

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
                        .UseHiLo("SelectionPurchaseSalePointKeySeq");

                    b.Property<string>("EntityModificationInfo")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<int>("PurchaseKeyId")
                        .HasColumnType("integer");

                    b.Property<int>("SalePointId")
                        .HasColumnType("integer");

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
                    b.HasOne("Superbrands.Selection.Infrastructure.DAL.SelectionDalDto", null)
                        .WithMany("ColorModelMetas")
                        .HasForeignKey("SelectionDalDtoId");

                    b.OwnsOne("Superbrands.Selection.Domain.ColorModelGroupKeys", "ColorModelGroupKeys", b1 =>
                        {
                            b1.Property<long>("ColorModelMetaDalDtoId")
                                .HasColumnType("bigint");

                            b1.Property<int>("ActivityId")
                                .HasColumnType("integer")
                                .HasColumnName("ActivityId");

                            b1.Property<int>("ActivityTypeId")
                                .HasColumnType("integer")
                                .HasColumnName("ActivityTypeId");

                            b1.Property<int>("AssortmentGroupId")
                                .HasColumnType("integer")
                                .HasColumnName("AssortmentGroupId");

                            b1.Property<int>("BrandId")
                                .HasColumnType("integer")
                                .HasColumnName("BrandId");

                            b1.Property<int>("PurchaseKeyId")
                                .HasColumnType("integer")
                                .HasColumnName("PurchaseKeyId");

                            b1.Property<int>("SalePointId")
                                .HasColumnType("integer")
                                .HasColumnName("SalePointId");

                            b1.HasKey("ColorModelMetaDalDtoId");

                            b1.ToTable("ColorModelMetas");

                            b1.WithOwner()
                                .HasForeignKey("ColorModelMetaDalDtoId");
                        });

                    b.Navigation("ColorModelGroupKeys");
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