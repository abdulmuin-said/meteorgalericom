using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    public partial class AddAdvancedProductMedia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AltMetin",
                table: "UrunResimleri",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Baslik",
                table: "UrunResimleri",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Etiketler",
                table: "UrunResimleri",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MedyaAlani",
                table: "UrunResimleri",
                type: "text",
                nullable: false,
                defaultValue: "Galeri");

            migrationBuilder.AddColumn<string>(
                name: "MedyaTipi",
                table: "UrunResimleri",
                type: "text",
                nullable: false,
                defaultValue: "Gorsel");

            migrationBuilder.AddColumn<string>(
                name: "MobilResimYolu",
                table: "UrunResimleri",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Sira",
                table: "UrunResimleri",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailYolu",
                table: "UrunResimleri",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UrunSecenekId",
                table: "UrunResimleri",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "UrunResimleri",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                UPDATE ""UrunResimleri""
                SET
                    ""Baslik"" = CASE WHEN COALESCE(""Baslik"", '') = '' THEN 'Galeri' ELSE ""Baslik"" END,
                    ""ThumbnailYolu"" = CASE WHEN COALESCE(""ThumbnailYolu"", '') = '' THEN ""ResimYolu"" ELSE ""ThumbnailYolu"" END,
                    ""Sira"" = CASE WHEN ""Sira"" = 0 THEN ""Id"" ELSE ""Sira"" END;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_UrunResimleri_UrunId_Sira",
                table: "UrunResimleri",
                columns: new[] { "UrunId", "Sira" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UrunResimleri_UrunId_Sira",
                table: "UrunResimleri");

            migrationBuilder.DropColumn(
                name: "AltMetin",
                table: "UrunResimleri");

            migrationBuilder.DropColumn(
                name: "Baslik",
                table: "UrunResimleri");

            migrationBuilder.DropColumn(
                name: "Etiketler",
                table: "UrunResimleri");

            migrationBuilder.DropColumn(
                name: "MedyaAlani",
                table: "UrunResimleri");

            migrationBuilder.DropColumn(
                name: "MedyaTipi",
                table: "UrunResimleri");

            migrationBuilder.DropColumn(
                name: "MobilResimYolu",
                table: "UrunResimleri");

            migrationBuilder.DropColumn(
                name: "Sira",
                table: "UrunResimleri");

            migrationBuilder.DropColumn(
                name: "ThumbnailYolu",
                table: "UrunResimleri");

            migrationBuilder.DropColumn(
                name: "UrunSecenekId",
                table: "UrunResimleri");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "UrunResimleri");
        }
    }
}
