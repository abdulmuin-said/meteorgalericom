using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSeoFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeoDescription",
                table: "Urunler",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SeoKeywords",
                table: "Urunler",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SeoTitle",
                table: "Urunler",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KargoFirmasi",
                table: "Siparisler",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KargoFirmasiId",
                table: "Siparisler",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "KargoFirmalari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    Kod = table.Column<string>(type: "text", nullable: false),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    Telefon = table.Column<string>(type: "text", nullable: true),
                    TakipUrl = table.Column<string>(type: "text", nullable: true),
                    GondericiUnvan = table.Column<string>(type: "text", nullable: false),
                    GondericiAdres = table.Column<string>(type: "text", nullable: false),
                    GondericiTelefon = table.Column<string>(type: "text", nullable: false),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    VarsayilanMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KargoFirmalari", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KargoFirmalari_Kod",
                table: "KargoFirmalari",
                column: "Kod",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KargoFirmalari");

            migrationBuilder.DropColumn(
                name: "SeoDescription",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "SeoKeywords",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "SeoTitle",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "KargoFirmasi",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "KargoFirmasiId",
                table: "Siparisler");
        }
    }
}
