using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    /// <inheritdoc />
    public partial class ZiyaretciDetay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IsletimSistemi",
                table: "ZiyaretciLoglari",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sehir",
                table: "ZiyaretciLoglari",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tarayici",
                table: "ZiyaretciLoglari",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ulke",
                table: "ZiyaretciLoglari",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsletimSistemi",
                table: "ZiyaretciLoglari");

            migrationBuilder.DropColumn(
                name: "Sehir",
                table: "ZiyaretciLoglari");

            migrationBuilder.DropColumn(
                name: "Tarayici",
                table: "ZiyaretciLoglari");

            migrationBuilder.DropColumn(
                name: "Ulke",
                table: "ZiyaretciLoglari");
        }
    }
}
