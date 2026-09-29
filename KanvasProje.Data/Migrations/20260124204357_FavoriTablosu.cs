using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    /// <inheritdoc />
    public partial class FavoriTablosu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UrunSecenekleri_Urunler_UrunId1",
                table: "UrunSecenekleri");

            migrationBuilder.DropIndex(
                name: "IX_UrunSecenekleri_UrunId1",
                table: "UrunSecenekleri");

            migrationBuilder.DropColumn(
                name: "UrunId1",
                table: "UrunSecenekleri");

            /* migrationBuilder.AddColumn<decimal>(
                name: "Fiyat",
                table: "Urunler",
                type: "numeric",
                nullable: false,
                defaultValue: 0m); */

            migrationBuilder.CreateTable(
                name: "Favoriler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppUserId = table.Column<string>(type: "text", nullable: false),
                    UrunId = table.Column<int>(type: "integer", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favoriler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favoriler_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favoriler_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Favoriler_AppUserId",
                table: "Favoriler",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Favoriler_UrunId",
                table: "Favoriler",
                column: "UrunId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favoriler");

            migrationBuilder.DropColumn(
                name: "Fiyat",
                table: "Urunler");

            migrationBuilder.AddColumn<int>(
                name: "UrunId1",
                table: "UrunSecenekleri",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UrunSecenekleri_UrunId1",
                table: "UrunSecenekleri",
                column: "UrunId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UrunSecenekleri_Urunler_UrunId1",
                table: "UrunSecenekleri",
                column: "UrunId1",
                principalTable: "Urunler",
                principalColumn: "Id");
        }
    }
}
