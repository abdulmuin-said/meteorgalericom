using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceFieldsToSiparis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FaturaDosyaAdi",
                table: "Siparisler",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FaturaDosyaYolu",
                table: "Siparisler",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FaturaMailGonderildiMi",
                table: "Siparisler",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FaturaMailGonderimTarihi",
                table: "Siparisler",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FaturaYuklendiMi",
                table: "Siparisler",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FaturaYuklenmeTarihi",
                table: "Siparisler",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FaturaDosyaAdi",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "FaturaDosyaYolu",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "FaturaMailGonderildiMi",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "FaturaMailGonderimTarihi",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "FaturaYuklendiMi",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "FaturaYuklenmeTarihi",
                table: "Siparisler");
        }
    }
}
