using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaytrSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PaytrAktifMi",
                table: "SiteAyarlari",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PaytrBasariliDonusUrl",
                table: "SiteAyarlari",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaytrBasarisizDonusUrl",
                table: "SiteAyarlari",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaytrCallbackUrl",
                table: "SiteAyarlari",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaytrMerchantId",
                table: "SiteAyarlari",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaytrMerchantKeyProtected",
                table: "SiteAyarlari",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaytrMerchantSaltProtected",
                table: "SiteAyarlari",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "PaytrTestModu",
                table: "SiteAyarlari",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaytrAktifMi",
                table: "SiteAyarlari");

            migrationBuilder.DropColumn(
                name: "PaytrBasariliDonusUrl",
                table: "SiteAyarlari");

            migrationBuilder.DropColumn(
                name: "PaytrBasarisizDonusUrl",
                table: "SiteAyarlari");

            migrationBuilder.DropColumn(
                name: "PaytrCallbackUrl",
                table: "SiteAyarlari");

            migrationBuilder.DropColumn(
                name: "PaytrMerchantId",
                table: "SiteAyarlari");

            migrationBuilder.DropColumn(
                name: "PaytrMerchantKeyProtected",
                table: "SiteAyarlari");

            migrationBuilder.DropColumn(
                name: "PaytrMerchantSaltProtected",
                table: "SiteAyarlari");

            migrationBuilder.DropColumn(
                name: "PaytrTestModu",
                table: "SiteAyarlari");
        }
    }
}
