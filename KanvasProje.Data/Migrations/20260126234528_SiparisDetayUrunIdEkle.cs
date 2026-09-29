using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    /// <inheritdoc />
    public partial class SiparisDetayUrunIdEkle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UrunId",
                table: "SiparisDetaylari",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SiparisDetaylari_UrunId",
                table: "SiparisDetaylari",
                column: "UrunId");

            migrationBuilder.AddForeignKey(
                name: "FK_SiparisDetaylari_Urunler_UrunId",
                table: "SiparisDetaylari",
                column: "UrunId",
                principalTable: "Urunler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SiparisDetaylari_Urunler_UrunId",
                table: "SiparisDetaylari");

            migrationBuilder.DropIndex(
                name: "IX_SiparisDetaylari_UrunId",
                table: "SiparisDetaylari");

            migrationBuilder.DropColumn(
                name: "UrunId",
                table: "SiparisDetaylari");
        }
    }
}
