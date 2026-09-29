using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDatabaseCartSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sepetler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppUserId = table.Column<string>(type: "text", nullable: true),
                    SessionId = table.Column<string>(type: "text", nullable: true),
                    SonGuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TerkEdildi = table.Column<bool>(type: "boolean", nullable: false),
                    TerkEdilmeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HatirlatmaGonderildi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sepetler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sepetler_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SepetItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SepetId = table.Column<int>(type: "integer", nullable: false),
                    UrunId = table.Column<int>(type: "integer", nullable: false),
                    UrunSecenekId = table.Column<int>(type: "integer", nullable: true),
                    Adet = table.Column<int>(type: "integer", nullable: false),
                    Fiyat = table.Column<decimal>(type: "numeric", nullable: false),
                    UrunBaslik = table.Column<string>(type: "text", nullable: false),
                    UrunResimUrl = table.Column<string>(type: "text", nullable: false),
                    SecenekAdi = table.Column<string>(type: "text", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SepetItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SepetItems_Sepetler_SepetId",
                        column: x => x.SepetId,
                        principalTable: "Sepetler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SepetItems_UrunSecenekleri_UrunSecenekId",
                        column: x => x.UrunSecenekId,
                        principalTable: "UrunSecenekleri",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SepetItems_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SepetItems_SepetId",
                table: "SepetItems",
                column: "SepetId");

            migrationBuilder.CreateIndex(
                name: "IX_SepetItems_UrunId",
                table: "SepetItems",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_SepetItems_UrunSecenekId",
                table: "SepetItems",
                column: "UrunSecenekId");

            migrationBuilder.CreateIndex(
                name: "IX_Sepetler_AppUserId",
                table: "Sepetler",
                column: "AppUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SepetItems");

            migrationBuilder.DropTable(
                name: "Sepetler");
        }
    }
}
