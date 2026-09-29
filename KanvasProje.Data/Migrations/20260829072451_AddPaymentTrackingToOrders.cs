using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentTrackingToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OdemeDurumu",
                table: "Siparisler",
                type: "text",
                nullable: false,
                defaultValue: "Bekliyor");

            migrationBuilder.AddColumn<string>(
                name: "OdemeHataKodu",
                table: "Siparisler",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OdemeHataMesaji",
                table: "Siparisler",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OdemeOnayTarihi",
                table: "Siparisler",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OdemeSaglayici",
                table: "Siparisler",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "OdemeTamamlandiMi",
                table: "Siparisler",
                type: "boolean",
                nullable: false,
                defaultValue: false);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OdemeDurumu",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "OdemeHataKodu",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "OdemeHataMesaji",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "OdemeOnayTarihi",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "OdemeSaglayici",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "OdemeTamamlandiMi",
                table: "Siparisler");

        }
    }
}
