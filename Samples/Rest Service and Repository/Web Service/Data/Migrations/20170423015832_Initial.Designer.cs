using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Web_Service.Data.Entities;

namespace Web_Service.Data.Migrations
{
    [DbContext(typeof(InternationalizationContext))]
    [Migration("20170423015832_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Web_Service.Data.Entities.CultureEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nchar(10)");

                    b.Property<string>("EnglishName")
                        .IsRequired()
                        .HasColumnType("nchar(255)");

                    b.Property<int>("Lcid")
                        .HasColumnName("LCID");

                    b.Property<string>("NativeName")
                        .IsRequired()
                        .HasColumnType("nchar(255)");

                    b.Property<int?>("ParentId")
                        .HasColumnName("ParentID");

                    b.Property<int?>("RegionId")
                        .HasColumnName("RegionID");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasName("IX_Cultures_Code");

                    b.HasIndex("ParentId");

                    b.HasIndex("RegionId");

                    b.ToTable("Cultures");

                    b.HasAnnotation("SqlServer:TableName", "Cultures");
                });

            modelBuilder.Entity("Web_Service.Data.Entities.CurrencyEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nchar(10)");

                    b.Property<string>("CurrencySymbol")
                        .IsRequired()
                        .HasColumnType("nchar(10)");

                    b.Property<bool>("IsCurrent")
                        .HasColumnType("bit");

                    b.Property<bool>("IsFund")
                        .HasColumnType("bit");

                    b.Property<int>("Number");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasName("IX_Currencies_Code");

                    b.HasIndex("Number")
                        .IsUnique()
                        .HasName("IX_Currencies_Number");

                    b.ToTable("Currencies");

                    b.HasAnnotation("SqlServer:TableName", "Currencies");
                });

            modelBuilder.Entity("Web_Service.Data.Entities.RegionEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nchar(10)");

                    b.Property<int>("CurrencyId")
                        .HasColumnName("CurrencyID");

                    b.Property<string>("EnglishName")
                        .IsRequired()
                        .HasColumnType("nchar(255)");

                    b.Property<bool>("GeoId");

                    b.Property<string>("NativeName")
                        .IsRequired()
                        .HasColumnType("nchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasName("IX_Regions_Code");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("GeoId")
                        .IsUnique()
                        .HasName("IX_Regions_GeoId");

                    b.ToTable("Regions");

                    b.HasAnnotation("SqlServer:TableName", "Regions");
                });

            modelBuilder.Entity("Web_Service.Data.Entities.CultureEntity", b =>
                {
                    b.HasOne("Web_Service.Data.Entities.CultureEntity", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .HasConstraintName("FK_Cultures_Cultures");

                    b.HasOne("Web_Service.Data.Entities.RegionEntity", "Region")
                        .WithMany("Cultures")
                        .HasForeignKey("RegionId")
                        .HasConstraintName("FK_Cultures_Regions");
                });

            modelBuilder.Entity("Web_Service.Data.Entities.RegionEntity", b =>
                {
                    b.HasOne("Web_Service.Data.Entities.CurrencyEntity", "Currency")
                        .WithMany("Regions")
                        .HasForeignKey("CurrencyId")
                        .HasConstraintName("FK_Regions_Currencies");
                });
        }
    }
}
