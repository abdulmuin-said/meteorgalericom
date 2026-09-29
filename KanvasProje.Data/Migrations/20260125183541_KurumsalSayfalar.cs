using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    /// <inheritdoc />
    public partial class KurumsalSayfalar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KurumsalSayfalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Baslik = table.Column<string>(type: "text", nullable: false),
                    Icerik = table.Column<string>(type: "text", nullable: false),
                    UrlSlug = table.Column<string>(type: "text", nullable: false),
                    Sira = table.Column<int>(type: "integer", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KurumsalSayfalar", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KurumsalSayfalar");
        }
    }
}
