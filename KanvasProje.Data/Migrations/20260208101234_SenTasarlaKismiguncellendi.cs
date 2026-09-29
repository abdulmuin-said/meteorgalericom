using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    /// <inheritdoc />
    public partial class SenTasarlaKismiguncellendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SenTasarla",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KullaniciId = table.Column<string>(type: "text", nullable: true),
                    DosyaYolu = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Olcu = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Fiyat = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Efekt = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CercveliMi = table.Column<bool>(type: "boolean", nullable: false),
                    ParcaSayisi = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SepeteEklendi = table.Column<bool>(type: "boolean", nullable: false),
                    SessionId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SenTasarla", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SenTasarla");
        }
    }
}
