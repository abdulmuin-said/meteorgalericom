using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRoomVisualizations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomVisualizations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoomVisualizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppUserId = table.Column<string>(type: "text", nullable: true),
                    UrunId = table.Column<int>(type: "integer", nullable: true),
                    Not = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OturumAnahtari = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SonucGorselYolu = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    UrunGenislik = table.Column<decimal>(type: "numeric", nullable: false),
                    UrunGorselYolu = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    UrunX = table.Column<decimal>(type: "numeric", nullable: false),
                    UrunY = table.Column<decimal>(type: "numeric", nullable: false),
                    UrunYukseklik = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomVisualizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomVisualizations_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RoomVisualizations_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomVisualizations_AppUserId",
                table: "RoomVisualizations",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomVisualizations_UrunId",
                table: "RoomVisualizations",
                column: "UrunId");
        }
    }
}
