using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    public partial class ExpandProductVariants : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AktifMi",
                table: "UrunSecenekleri",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "CerceveKalinligi",
                table: "UrunSecenekleri",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CerceveRengi",
                table: "UrunSecenekleri",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Desi",
                table: "UrunSecenekleri",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FiyatFarki",
                table: "UrunSecenekleri",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "GorselUrl",
                table: "UrunSecenekleri",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KisilestirmeMetni",
                table: "UrunSecenekleri",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MalzemeTuru",
                table: "UrunSecenekleri",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "OnSipariseAcikMi",
                table: "UrunSecenekleri",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OzelTasarimNotu",
                table: "UrunSecenekleri",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ParcaSayisi",
                table: "UrunSecenekleri",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Sira",
                table: "UrunSecenekleri",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "TukeninceGizle",
                table: "UrunSecenekleri",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UretimSuresiGun",
                table: "UrunSecenekleri",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VaryantSku",
                table: "UrunSecenekleri",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "VarsayilanMi",
                table: "UrunSecenekleri",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Yon",
                table: "UrunSecenekleri",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "AktifMi", table: "UrunSecenekleri");
            migrationBuilder.DropColumn(name: "CerceveKalinligi", table: "UrunSecenekleri");
            migrationBuilder.DropColumn(name: "CerceveRengi", table: "UrunSecenekleri");
            migrationBuilder.DropColumn(name: "Desi", table: "UrunSecenekleri");
            migrationBuilder.DropColumn(name: "FiyatFarki", table: "UrunSecenekleri");
            migrationBuilder.DropColumn(name: "GorselUrl", table: "UrunSecenekleri");
            migrationBuilder.DropColumn(name: "KisilestirmeMetni", table: "UrunSecenekleri");
            migrationBuilder.DropColumn(name: "MalzemeTuru", table: "UrunSecenekleri");
            migrationBuilder.DropColumn(name: "OnSipariseAcikMi", table: "UrunSecenekleri");
            migrationBuilder.DropColumn(name: "OzelTasarimNotu", table: "UrunSecenekleri");
            migrationBuilder.DropColumn(name: "ParcaSayisi", table: "UrunSecenekleri");
            migrationBuilder.DropColumn(name: "Sira", table: "UrunSecenekleri");
            migrationBuilder.DropColumn(name: "TukeninceGizle", table: "UrunSecenekleri");
            migrationBuilder.DropColumn(name: "UretimSuresiGun", table: "UrunSecenekleri");
            migrationBuilder.DropColumn(name: "VaryantSku", table: "UrunSecenekleri");
            migrationBuilder.DropColumn(name: "VarsayilanMi", table: "UrunSecenekleri");
            migrationBuilder.DropColumn(name: "Yon", table: "UrunSecenekleri");
        }
    }
}
