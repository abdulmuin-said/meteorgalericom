using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    /// <inheritdoc />
    public partial class SiparisUserIliskisi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Siparisler",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Siparisler_AppUserId",
                table: "Siparisler",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Siparisler_AspNetUsers_AppUserId",
                table: "Siparisler",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Siparisler_AspNetUsers_AppUserId",
                table: "Siparisler");

            migrationBuilder.DropIndex(
                name: "IX_Siparisler_AppUserId",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Siparisler");
        }
    }
}
