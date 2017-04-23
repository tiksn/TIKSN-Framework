using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Web_Service.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nchar(10)", nullable: false),
                    CurrencySymbol = table.Column<string>(type: "nchar(10)", nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    IsFund = table.Column<bool>(type: "bit", nullable: false),
                    Number = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nchar(10)", nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    EnglishName = table.Column<string>(type: "nchar(255)", nullable: false),
                    GeoId = table.Column<bool>(nullable: false),
                    NativeName = table.Column<string>(type: "nchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Regions_Currencies",
                        column: x => x.CurrencyID,
                        principalTable: "Currencies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cultures",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nchar(10)", nullable: false),
                    EnglishName = table.Column<string>(type: "nchar(255)", nullable: false),
                    LCID = table.Column<int>(nullable: false),
                    NativeName = table.Column<string>(type: "nchar(255)", nullable: false),
                    ParentID = table.Column<int>(nullable: true),
                    RegionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cultures", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Cultures_Cultures",
                        column: x => x.ParentID,
                        principalTable: "Cultures",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cultures_Regions",
                        column: x => x.RegionID,
                        principalTable: "Regions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cultures_Code",
                table: "Cultures",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cultures_ParentID",
                table: "Cultures",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_Cultures_RegionID",
                table: "Cultures",
                column: "RegionID");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Code",
                table: "Currencies",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Number",
                table: "Currencies",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Regions_Code",
                table: "Regions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Regions_CurrencyID",
                table: "Regions",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_GeoId",
                table: "Regions",
                column: "GeoId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cultures");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropTable(
                name: "Currencies");
        }
    }
}
