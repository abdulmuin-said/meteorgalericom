using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    public partial class AddDynamicProductFeatures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrunTipi",
                table: "Urunler",
                type: "text",
                nullable: false,
                defaultValue: "Genel");

            migrationBuilder.CreateTable(
                name: "UrunOzellikTanimlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    Kod = table.Column<string>(type: "text", nullable: false),
                    UrunTipi = table.Column<string>(type: "text", nullable: false),
                    AlanTipi = table.Column<string>(type: "text", nullable: false),
                    YardimMetni = table.Column<string>(type: "text", nullable: false),
                    Secenekler = table.Column<string>(type: "text", nullable: false),
                    FiltredeGoster = table.Column<bool>(type: "boolean", nullable: false),
                    DetaySayfasindaGoster = table.Column<bool>(type: "boolean", nullable: false),
                    TeknikTablodaGoster = table.Column<bool>(type: "boolean", nullable: false),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    Sira = table.Column<int>(type: "integer", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrunOzellikTanimlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UrunOzellikDegerleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UrunId = table.Column<int>(type: "integer", nullable: false),
                    UrunOzellikTanimiId = table.Column<int>(type: "integer", nullable: false),
                    Deger = table.Column<string>(type: "text", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrunOzellikDegerleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UrunOzellikDegerleri_UrunOzellikTanimlari_UrunOzellikTanimiId",
                        column: x => x.UrunOzellikTanimiId,
                        principalTable: "UrunOzellikTanimlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UrunOzellikDegerleri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UrunOzellikDegerleri_UrunId",
                table: "UrunOzellikDegerleri",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_UrunOzellikDegerleri_UrunOzellikTanimiId",
                table: "UrunOzellikDegerleri",
                column: "UrunOzellikTanimiId");

            migrationBuilder.CreateIndex(
                name: "IX_UrunOzellikTanimlari_UrunTipi_Kod",
                table: "UrunOzellikTanimlari",
                columns: new[] { "UrunTipi", "Kod" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "UrunOzellikDegerleri");
            migrationBuilder.DropTable(name: "UrunOzellikTanimlari");
            migrationBuilder.DropColumn(name: "UrunTipi", table: "Urunler");
        }
    }
}
