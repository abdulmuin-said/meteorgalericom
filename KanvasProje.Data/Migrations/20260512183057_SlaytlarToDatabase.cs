using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    /// <inheritdoc />
    public partial class SlaytlarToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Slaytlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Baslik = table.Column<string>(type: "text", nullable: false),
                    AltBaslik = table.Column<string>(type: "text", nullable: false),
                    Aciklama = table.Column<string>(type: "text", nullable: false),
                    ResimUrl = table.Column<string>(type: "text", nullable: true),
                    VideoUrl = table.Column<string>(type: "text", nullable: true),
                    Tur = table.Column<string>(type: "text", nullable: false),
                    Sira = table.Column<int>(type: "integer", nullable: false),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slaytlar", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Slaytlar");
        }
    }
}
