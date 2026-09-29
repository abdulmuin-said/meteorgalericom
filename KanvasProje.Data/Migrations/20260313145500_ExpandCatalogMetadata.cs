using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    public partial class ExpandCatalogMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AltMetin",
                table: "Kategoriler",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BannerUrl",
                table: "Kategoriler",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KampanyaEtiketi",
                table: "Kategoriler",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KisaAciklama",
                table: "Kategoriler",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ParentKategoriId",
                table: "Kategoriler",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoDescription",
                table: "Kategoriler",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SeoTitle",
                table: "Kategoriler",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Sira",
                table: "Kategoriler",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Kategoriler",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrunSiralamaTipi",
                table: "Kategoriler",
                type: "text",
                nullable: false,
                defaultValue: "manual");

            migrationBuilder.AddColumn<string>(
                name: "UstMetin",
                table: "Kategoriler",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "AktifMi",
                table: "Urunler",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "AnaSayfadaGoster",
                table: "Urunler",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "BakimTalimati",
                table: "Urunler",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Barkod",
                table: "Urunler",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Etiketler",
                table: "Urunler",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FavoriSayisi",
                table: "Urunler",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoruntulenmeSayisi",
                table: "Urunler",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "IndirimliFiyat",
                table: "Urunler",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KargoyaVerilisSuresiGun",
                table: "Urunler",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "KisaAciklama",
                table: "Urunler",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KisaAd",
                table: "Urunler",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "KdvOrani",
                table: "Urunler",
                type: "numeric",
                nullable: false,
                defaultValue: 20m);

            migrationBuilder.AddColumn<bool>(
                name: "KampanyaliMi",
                table: "Urunler",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Maliyet",
                table: "Urunler",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "MalzemeBilgisi",
                table: "Urunler",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Marka",
                table: "Urunler",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MaxSiparisAdedi",
                table: "Urunler",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinSiparisAdedi",
                table: "Urunler",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<bool>(
                name: "OneCikanMi",
                table: "Urunler",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PaketlemeBilgisi",
                table: "Urunler",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SatisSayisi",
                table: "Urunler",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SKU",
                table: "Urunler",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Sira",
                table: "Urunler",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StokDurumu",
                table: "Urunler",
                type: "text",
                nullable: false,
                defaultValue: "Stokta");

            migrationBuilder.AddColumn<string>(
                name: "TeknikOzellikler",
                table: "Urunler",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TahminiTeslimSuresiGun",
                table: "Urunler",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UretimSuresiGun",
                table: "Urunler",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "YeniUrunMu",
                table: "Urunler",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Kategoriler_ParentKategoriId",
                table: "Kategoriler",
                column: "ParentKategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_Kategoriler_Slug",
                table: "Kategoriler",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_SKU",
                table: "Urunler",
                column: "SKU");

            migrationBuilder.AddForeignKey(
                name: "FK_Kategoriler_Kategoriler_ParentKategoriId",
                table: "Kategoriler",
                column: "ParentKategoriId",
                principalTable: "Kategoriler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kategoriler_Kategoriler_ParentKategoriId",
                table: "Kategoriler");

            migrationBuilder.DropIndex(
                name: "IX_Kategoriler_ParentKategoriId",
                table: "Kategoriler");

            migrationBuilder.DropIndex(
                name: "IX_Kategoriler_Slug",
                table: "Kategoriler");

            migrationBuilder.DropIndex(
                name: "IX_Urunler_SKU",
                table: "Urunler");

            migrationBuilder.DropColumn(name: "AltMetin", table: "Kategoriler");
            migrationBuilder.DropColumn(name: "BannerUrl", table: "Kategoriler");
            migrationBuilder.DropColumn(name: "KampanyaEtiketi", table: "Kategoriler");
            migrationBuilder.DropColumn(name: "KisaAciklama", table: "Kategoriler");
            migrationBuilder.DropColumn(name: "ParentKategoriId", table: "Kategoriler");
            migrationBuilder.DropColumn(name: "SeoDescription", table: "Kategoriler");
            migrationBuilder.DropColumn(name: "SeoTitle", table: "Kategoriler");
            migrationBuilder.DropColumn(name: "Sira", table: "Kategoriler");
            migrationBuilder.DropColumn(name: "Slug", table: "Kategoriler");
            migrationBuilder.DropColumn(name: "UrunSiralamaTipi", table: "Kategoriler");
            migrationBuilder.DropColumn(name: "UstMetin", table: "Kategoriler");

            migrationBuilder.DropColumn(name: "AktifMi", table: "Urunler");
            migrationBuilder.DropColumn(name: "AnaSayfadaGoster", table: "Urunler");
            migrationBuilder.DropColumn(name: "BakimTalimati", table: "Urunler");
            migrationBuilder.DropColumn(name: "Barkod", table: "Urunler");
            migrationBuilder.DropColumn(name: "Etiketler", table: "Urunler");
            migrationBuilder.DropColumn(name: "FavoriSayisi", table: "Urunler");
            migrationBuilder.DropColumn(name: "GoruntulenmeSayisi", table: "Urunler");
            migrationBuilder.DropColumn(name: "IndirimliFiyat", table: "Urunler");
            migrationBuilder.DropColumn(name: "KargoyaVerilisSuresiGun", table: "Urunler");
            migrationBuilder.DropColumn(name: "KisaAciklama", table: "Urunler");
            migrationBuilder.DropColumn(name: "KisaAd", table: "Urunler");
            migrationBuilder.DropColumn(name: "KdvOrani", table: "Urunler");
            migrationBuilder.DropColumn(name: "KampanyaliMi", table: "Urunler");
            migrationBuilder.DropColumn(name: "Maliyet", table: "Urunler");
            migrationBuilder.DropColumn(name: "MalzemeBilgisi", table: "Urunler");
            migrationBuilder.DropColumn(name: "Marka", table: "Urunler");
            migrationBuilder.DropColumn(name: "MaxSiparisAdedi", table: "Urunler");
            migrationBuilder.DropColumn(name: "MinSiparisAdedi", table: "Urunler");
            migrationBuilder.DropColumn(name: "OneCikanMi", table: "Urunler");
            migrationBuilder.DropColumn(name: "PaketlemeBilgisi", table: "Urunler");
            migrationBuilder.DropColumn(name: "SatisSayisi", table: "Urunler");
            migrationBuilder.DropColumn(name: "SKU", table: "Urunler");
            migrationBuilder.DropColumn(name: "Sira", table: "Urunler");
            migrationBuilder.DropColumn(name: "StokDurumu", table: "Urunler");
            migrationBuilder.DropColumn(name: "TeknikOzellikler", table: "Urunler");
            migrationBuilder.DropColumn(name: "TahminiTeslimSuresiGun", table: "Urunler");
            migrationBuilder.DropColumn(name: "UretimSuresiGun", table: "Urunler");
            migrationBuilder.DropColumn(name: "YeniUrunMu", table: "Urunler");
        }
    }
}
