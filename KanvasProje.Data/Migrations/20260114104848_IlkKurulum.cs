using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    /// <inheritdoc />
    public partial class IlkKurulum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kategoriler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    Aciklama = table.Column<string>(type: "text", nullable: false),
                    GorselUrl = table.Column<string>(type: "text", nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategoriler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Siparisler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MusteriAdSoyad = table.Column<string>(type: "text", nullable: false),
                    Telefon = table.Column<string>(type: "text", nullable: false),
                    Eposta = table.Column<string>(type: "text", nullable: false),
                    Sehir = table.Column<string>(type: "text", nullable: false),
                    Ilce = table.Column<string>(type: "text", nullable: false),
                    AcikAdres = table.Column<string>(type: "text", nullable: false),
                    ToplamTutar = table.Column<decimal>(type: "numeric", nullable: false),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    KargoTakipNo = table.Column<string>(type: "text", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Siparisler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Urunler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Baslik = table.Column<string>(type: "text", nullable: false),
                    UrlYolu = table.Column<string>(type: "text", nullable: false),
                    Aciklama = table.Column<string>(type: "text", nullable: false),
                    AnaGorselUrl = table.Column<string>(type: "text", nullable: false),
                    KategoriId = table.Column<int>(type: "integer", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urunler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Urunler_Kategoriler_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "Kategoriler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UrunSecenekleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UrunId = table.Column<int>(type: "integer", nullable: false),
                    Olcu = table.Column<string>(type: "text", nullable: false),
                    CerceveTipi = table.Column<string>(type: "text", nullable: false),
                    SatisFiyati = table.Column<decimal>(type: "numeric", nullable: false),
                    MaliyetFiyati = table.Column<decimal>(type: "numeric", nullable: false),
                    StokAdedi = table.Column<int>(type: "integer", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrunSecenekleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UrunSecenekleri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiparisDetaylari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SiparisId = table.Column<int>(type: "integer", nullable: false),
                    UrunSecenekId = table.Column<int>(type: "integer", nullable: false),
                    Adet = table.Column<int>(type: "integer", nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "numeric", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiparisDetaylari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiparisDetaylari_Siparisler_SiparisId",
                        column: x => x.SiparisId,
                        principalTable: "Siparisler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SiparisDetaylari_UrunSecenekleri_UrunSecenekId",
                        column: x => x.UrunSecenekId,
                        principalTable: "UrunSecenekleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SiparisDetaylari_SiparisId",
                table: "SiparisDetaylari",
                column: "SiparisId");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisDetaylari_UrunSecenekId",
                table: "SiparisDetaylari",
                column: "UrunSecenekId");

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_KategoriId",
                table: "Urunler",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_UrunSecenekleri_UrunId",
                table: "UrunSecenekleri",
                column: "UrunId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiparisDetaylari");

            migrationBuilder.DropTable(
                name: "Siparisler");

            migrationBuilder.DropTable(
                name: "UrunSecenekleri");

            migrationBuilder.DropTable(
                name: "Urunler");

            migrationBuilder.DropTable(
                name: "Kategoriler");
        }
    }
}
