using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KanvasProje.Core.Varliklar
{
    public class UrunResim : BaseEntity
    {
        public string ResimYolu { get; set; } = string.Empty;
        public string Baslik { get; set; } = string.Empty;
        public string AltMetin { get; set; } = string.Empty;
        public string MedyaTipi { get; set; } = "Gorsel";
        public string MedyaAlani { get; set; } = "Galeri";
        public string VideoUrl { get; set; } = string.Empty;
        public string ThumbnailYolu { get; set; } = string.Empty;
        public string MobilResimYolu { get; set; } = string.Empty;
        public string Etiketler { get; set; } = string.Empty;
        public int Sira { get; set; }
        public bool VarsayilanMi { get; set; }
        public int? UrunSecenekId { get; set; }

        public int UrunId { get; set; }

        [ForeignKey("UrunId")]
        public virtual Urun Urun { get; set; } = default!;

        [NotMapped]
        public bool VideoMu => string.Equals(MedyaTipi, "Video", StringComparison.OrdinalIgnoreCase);

        [NotMapped]
        public string EtkinKaynak =>
            !string.IsNullOrWhiteSpace(VideoUrl)
                ? VideoUrl
                : ResimYolu;

        [NotMapped]
        public string EtkinPosterYolu =>
            !string.IsNullOrWhiteSpace(ThumbnailYolu)
                ? ThumbnailYolu
                : (!string.IsNullOrWhiteSpace(MobilResimYolu)
                    ? MobilResimYolu
                    : ResimYolu);
    }
}
