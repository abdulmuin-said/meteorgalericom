using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    /// <inheritdoc />
    public partial class CihazModeliEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CihazModeli",
                table: "ZiyaretciLoglari",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CihazModeli",
                table: "ZiyaretciLoglari");
        }
    }
}
