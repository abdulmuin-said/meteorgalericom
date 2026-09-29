using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    /// <inheritdoc />
    public partial class UrunSecenekFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
