using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    /// <inheritdoc />
    public partial class SiteAyarlariToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteAyarlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SiteAdi = table.Column<string>(type: "text", nullable: false),
                    MarkaAdi = table.Column<string>(type: "text", nullable: false),
                    SiteBasligi = table.Column<string>(type: "text", nullable: false),
                    SiteAciklamasi = table.Column<string>(type: "text", nullable: false),
                    SiteLogoUrl = table.Column<string>(type: "text", nullable: false),
                    FaviconUrl = table.Column<string>(type: "text", nullable: false),
                    BaseUrl = table.Column<string>(type: "text", nullable: false),
                    TemaRengi = table.Column<string>(type: "text", nullable: false),
                    UstBarMesaji = table.Column<string>(type: "text", nullable: false),
                    KampanyaMesaji = table.Column<string>(type: "text", nullable: false),
                    FooterAciklamasi = table.Column<string>(type: "text", nullable: false),
                    Telefon = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Adres = table.Column<string>(type: "text", nullable: false),
                    WhatsappNumarasi = table.Column<string>(type: "text", nullable: false),
                    CalismaSaatleri = table.Column<string>(type: "text", nullable: false),
                    FacebookUrl = table.Column<string>(type: "text", nullable: false),
                    InstagramUrl = table.Column<string>(type: "text", nullable: false),
                    TwitterUrl = table.Column<string>(type: "text", nullable: false),
                    YoutubeUrl = table.Column<string>(type: "text", nullable: false),
                    TiktokUrl = table.Column<string>(type: "text", nullable: false),
                    PinterestUrl = table.Column<string>(type: "text", nullable: false),
                    ParaBirimi = table.Column<string>(type: "text", nullable: false),
                    KargoBedeli = table.Column<decimal>(type: "numeric", nullable: false),
                    UcretsizKargoLimiti = table.Column<decimal>(type: "numeric", nullable: false),
                    StokUyariLimiti = table.Column<int>(type: "integer", nullable: false),
                    StoktaYokSatisIzni = table.Column<bool>(type: "boolean", nullable: false),
                    KargoFirmasi = table.Column<string>(type: "text", nullable: false),
                    KargoTakipUrl = table.Column<string>(type: "text", nullable: false),
                    SiparisTeslimSuresiGun = table.Column<int>(type: "integer", nullable: false),
                    IadeHakkiGun = table.Column<int>(type: "integer", nullable: false),
                    MetaTitle = table.Column<string>(type: "text", nullable: false),
                    MetaDescription = table.Column<string>(type: "text", nullable: false),
                    MetaKeywords = table.Column<string>(type: "text", nullable: false),
                    GoogleAnalyticsId = table.Column<string>(type: "text", nullable: false),
                    FacebookPixelId = table.Column<string>(type: "text", nullable: false),
                    VarsayilanSosyalPaylasimGorseliUrl = table.Column<string>(type: "text", nullable: false),
                    CookieMetni = table.Column<string>(type: "text", nullable: false),
                    YeniSiparisMailBildirimi = table.Column<bool>(type: "boolean", nullable: false),
                    StokUyarisiMailBildirimi = table.Column<bool>(type: "boolean", nullable: false),
                    IadeTalebiMailBildirimi = table.Column<bool>(type: "boolean", nullable: false),
                    BildirimAliciEmail = table.Column<string>(type: "text", nullable: false),
                    BakimModuAktif = table.Column<bool>(type: "boolean", nullable: false),
                    BakimModuMesaji = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteAyarlari", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SiteAyarlari",
                columns: new[] { "Id", "SiteAdi", "MarkaAdi", "SiteBasligi", "SiteAciklamasi", "SiteLogoUrl", "FaviconUrl", "BaseUrl", "TemaRengi", "UstBarMesaji", "KampanyaMesaji", "FooterAciklamasi", "Telefon", "Email", "Adres", "WhatsappNumarasi", "CalismaSaatleri", "FacebookUrl", "InstagramUrl", "TwitterUrl", "YoutubeUrl", "TiktokUrl", "PinterestUrl", "ParaBirimi", "KargoBedeli", "UcretsizKargoLimiti", "StokUyariLimiti", "StoktaYokSatisIzni", "KargoFirmasi", "KargoTakipUrl", "SiparisTeslimSuresiGun", "IadeHakkiGun", "MetaTitle", "MetaDescription", "MetaKeywords", "GoogleAnalyticsId", "FacebookPixelId", "VarsayilanSosyalPaylasimGorseliUrl", "CookieMetni", "YeniSiparisMailBildirimi", "StokUyarisiMailBildirimi", "IadeTalebiMailBildirimi", "BildirimAliciEmail", "BakimModuAktif", "BakimModuMesaji" },
                values: new object[] { 1, "Canvasia", "Canvasia", "Canvasia - Online Dekorasyon Mağazası", "Duvar dekorasyonu ve yaşam alanları için premium ürünler.", "/logo_svg.svg", "/favicon.ico", "https://www.canvasia.com.tr", "#313511", "500 TL üzeri ücretsiz kargo", "Vade farksız 3 taksit", "Premium duvar dekorasyonu ve özel tasarım ürünleri.", "+90 543 221 23 20", "info@canvasia.com.tr", "Merkez/Kayseri", "905432212320", "Haftaiçi 09:00 - 19:00, Cumartesi 09:00 - 19:00, Pazar Tatil", "", "https://www.instagram.com/canvasia.store", "", "", "", "", "TL", 0m, 500m, 5, false, "Aras Kargo", "", 5, 14, "Canvasia - Premium Dekorasyon Ürünleri, Kanvas Tablo ve Duvar Sanatı", "Canvasia; kanvas tablo, cam tablo, duvar dekorasyonu ve yaşam alanlarına özel premium dekorasyon ürünleri sunar.", "kanvas tablo, cam tablo, duvar dekorasyonu, duvar sanatı, tablo, dekorasyon ürünleri, Canvasia", "", "", "/EmailTemplates/canvasia-logo.png", "Deneyiminizi iyileştirmek, sepetinizi korumak ve site trafiğini analiz etmek için çerezler kullanıyoruz.", true, true, true, "canvasia.com.tr@gmail.com", false, "Size daha iyi bir alışveriş deneyimi sunmak için kısa bir bakım çalışması yapıyoruz. Çok yakında premium dekorasyon ürünlerimizle yeniden yayında olacağız." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteAyarlari");
        }
    }
}
