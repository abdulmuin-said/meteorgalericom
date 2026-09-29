using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    /// <inheritdoc />
    public partial class LocalizeCategoryImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE public.""Kategoriler""
SET ""GorselUrl"" = CASE ""Slug""
    WHEN 'cam-tablo' THEN '/img/categories/cam-tablo.jpg'
    WHEN 'duvar-kagidi' THEN '/img/categories/duvar-kagidi.jpg'
    WHEN 'cam-kesme-tahtasi' THEN '/img/categories/cam-kesme-tahtasi.jpg'
    WHEN 'ayna' THEN '/img/categories/ayna.jpg'
    WHEN 'baskili-hali' THEN '/img/categories/baskili-hali.jpg'
    WHEN 'ahsap-dekorasyon-urunleri' THEN '/img/categories/ahsap-dekorasyon.jpg'
    WHEN 'mobilya-aksesuar' THEN '/img/categories/mobilya-aksesuar.jpg'
    WHEN 'lightbox' THEN '/img/categories/lightbox.jpg'
    WHEN 'afis' THEN '/img/categories/afis.jpg'
    ELSE ""GorselUrl""
END,
""BannerUrl"" = CASE ""Slug""
    WHEN 'cam-tablo' THEN '/img/categories/cam-tablo.jpg'
    WHEN 'duvar-kagidi' THEN '/img/categories/duvar-kagidi.jpg'
    WHEN 'cam-kesme-tahtasi' THEN '/img/categories/cam-kesme-tahtasi.jpg'
    WHEN 'ayna' THEN '/img/categories/ayna.jpg'
    WHEN 'baskili-hali' THEN '/img/categories/baskili-hali.jpg'
    WHEN 'ahsap-dekorasyon-urunleri' THEN '/img/categories/ahsap-dekorasyon.jpg'
    WHEN 'mobilya-aksesuar' THEN '/img/categories/mobilya-aksesuar.jpg'
    WHEN 'lightbox' THEN '/img/categories/lightbox.jpg'
    WHEN 'afis' THEN '/img/categories/afis.jpg'
    ELSE ""BannerUrl""
END
WHERE ""Slug"" IN ('cam-tablo', 'duvar-kagidi', 'cam-kesme-tahtasi', 'ayna', 'baskili-hali', 'ahsap-dekorasyon-urunleri', 'mobilya-aksesuar', 'lightbox', 'afis');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE public.""Kategoriler""
SET ""GorselUrl"" = CASE ""Slug""
    WHEN 'cam-tablo' THEN 'https://www.glassium.com/img/products/ct1865_td_3.jpg'
    WHEN 'duvar-kagidi' THEN 'https://www.bkdekor.com/idea/ij/91/myassets/products/676/bkdk142.jpg?revision=1697143329'
    WHEN 'cam-kesme-tahtasi' THEN 'https://productimages.hepsiburada.net/s/777/375-375/110000672470959.jpg'
    WHEN 'ayna' THEN 'https://images.pexels.com/photos/37148742/pexels-photo-37148742.jpeg'
    WHEN 'baskili-hali' THEN 'https://images.pexels.com/photos/35134989/pexels-photo-35134989.jpeg'
    WHEN 'ahsap-dekorasyon-urunleri' THEN 'https://images.pexels.com/photos/37134529/pexels-photo-37134529.jpeg'
    WHEN 'mobilya-aksesuar' THEN 'https://images.pexels.com/photos/36789165/pexels-photo-36789165.jpeg'
    WHEN 'lightbox' THEN 'https://images.pexels.com/photos/3843283/pexels-photo-3843283.jpeg'
    WHEN 'afis' THEN 'https://images.pexels.com/photos/33827721/pexels-photo-33827721.jpeg'
    ELSE ""GorselUrl""
END,
""BannerUrl"" = CASE ""Slug""
    WHEN 'cam-tablo' THEN 'https://www.glassium.com/img/products/ct1865_td_3.jpg'
    WHEN 'duvar-kagidi' THEN 'https://www.bkdekor.com/idea/ij/91/myassets/products/676/bkdk142.jpg?revision=1697143329'
    WHEN 'cam-kesme-tahtasi' THEN 'https://productimages.hepsiburada.net/s/777/375-375/110000672470959.jpg'
    WHEN 'ayna' THEN 'https://images.pexels.com/photos/37148742/pexels-photo-37148742.jpeg'
    WHEN 'baskili-hali' THEN 'https://images.pexels.com/photos/35134989/pexels-photo-35134989.jpeg'
    WHEN 'ahsap-dekorasyon-urunleri' THEN 'https://images.pexels.com/photos/37134529/pexels-photo-37134529.jpeg'
    WHEN 'mobilya-aksesuar' THEN 'https://images.pexels.com/photos/36789165/pexels-photo-36789165.jpeg'
    WHEN 'lightbox' THEN 'https://images.pexels.com/photos/3843283/pexels-photo-3843283.jpeg'
    WHEN 'afis' THEN 'https://images.pexels.com/photos/33827721/pexels-photo-33827721.jpeg'
    ELSE ""BannerUrl""
END
WHERE ""Slug"" IN ('cam-tablo', 'duvar-kagidi', 'cam-kesme-tahtasi', 'ayna', 'baskili-hali', 'ahsap-dekorasyon-urunleri', 'mobilya-aksesuar', 'lightbox', 'afis');");
        }
    }
}
