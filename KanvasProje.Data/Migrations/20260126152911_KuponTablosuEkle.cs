using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    /// <inheritdoc />
    public partial class KuponTablosuEkle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kuponlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kod = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Tip = table.Column<int>(type: "integer", nullable: false),
                    Deger = table.Column<decimal>(type: "numeric", nullable: false),
                    MinSepetTutari = table.Column<decimal>(type: "numeric", nullable: false),
                    SonKullanmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    KullanimLimiti = table.Column<int>(type: "integer", nullable: false),
                    KullanilanMiktar = table.Column<int>(type: "integer", nullable: false),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kuponlar", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Kuponlar");
        }
    }
}
