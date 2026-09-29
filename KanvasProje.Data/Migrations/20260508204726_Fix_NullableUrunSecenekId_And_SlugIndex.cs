using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    /// <inheritdoc />
    public partial class Fix_NullableUrunSecenekId_And_SlugIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SiparisDetaylari_Siparisler_siparisId",
                table: "SiparisDetaylari");

            migrationBuilder.DropForeignKey(
                name: "FK_SiparisDetaylari_UrunSecenekleri_UrunSecenekId",
                table: "SiparisDetaylari");

            migrationBuilder.DropColumn(
                name: "SiparisId",
                table: "SiparisDetaylari");

            migrationBuilder.RenameColumn(
                name: "siparisId",
                table: "SiparisDetaylari",
                newName: "SiparisId");

            migrationBuilder.RenameIndex(
                name: "IX_SiparisDetaylari_siparisId",
                table: "SiparisDetaylari",
                newName: "IX_SiparisDetaylari_SiparisId");

            migrationBuilder.AlterColumn<int>(
                name: "UrunSecenekId",
                table: "SiparisDetaylari",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_Slug",
                table: "Urunler",
                column: "Slug",
                filter: "\"Slug\" IS NOT NULL AND \"Slug\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Kategoriler_Slug",
                table: "Kategoriler",
                column: "Slug",
                filter: "\"Slug\" IS NOT NULL AND \"Slug\" <> ''");

            migrationBuilder.AddForeignKey(
                name: "FK_SiparisDetaylari_Siparisler_SiparisId",
                table: "SiparisDetaylari",
                column: "SiparisId",
                principalTable: "Siparisler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SiparisDetaylari_UrunSecenekleri_UrunSecenekId",
                table: "SiparisDetaylari",
                column: "UrunSecenekId",
                principalTable: "UrunSecenekleri",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SiparisDetaylari_Siparisler_SiparisId",
                table: "SiparisDetaylari");

            migrationBuilder.DropForeignKey(
                name: "FK_SiparisDetaylari_UrunSecenekleri_UrunSecenekId",
                table: "SiparisDetaylari");

            migrationBuilder.DropIndex(
                name: "IX_Urunler_Slug",
                table: "Urunler");

            migrationBuilder.DropIndex(
                name: "IX_Kategoriler_Slug",
                table: "Kategoriler");

            migrationBuilder.RenameColumn(
                name: "SiparisId",
                table: "SiparisDetaylari",
                newName: "siparisId");

            migrationBuilder.RenameIndex(
                name: "IX_SiparisDetaylari_SiparisId",
                table: "SiparisDetaylari",
                newName: "IX_SiparisDetaylari_siparisId");

            migrationBuilder.AlterColumn<int>(
                name: "UrunSecenekId",
                table: "SiparisDetaylari",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SiparisId",
                table: "SiparisDetaylari",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_SiparisDetaylari_Siparisler_siparisId",
                table: "SiparisDetaylari",
                column: "siparisId",
                principalTable: "Siparisler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SiparisDetaylari_UrunSecenekleri_UrunSecenekId",
                table: "SiparisDetaylari",
                column: "UrunSecenekId",
                principalTable: "UrunSecenekleri",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
