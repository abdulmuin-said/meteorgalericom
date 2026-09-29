using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    /// <inheritdoc />
    public partial class IadeTalebiTablosu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IadeTalepleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SiparisId = table.Column<int>(type: "integer", nullable: false),
                    MusteriId = table.Column<string>(type: "text", nullable: false),
                    Neden = table.Column<string>(type: "text", nullable: false),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    IBAN = table.Column<string>(type: "text", nullable: true),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    AdminNotu = table.Column<string>(type: "text", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IadeTalepleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IadeTalepleri_Siparisler_SiparisId",
                        column: x => x.SiparisId,
                        principalTable: "Siparisler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IadeTalepleri_SiparisId",
                table: "IadeTalepleri",
                column: "SiparisId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IadeTalepleri");
        }
    }
}
