using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace KanvasProje.Core.Varliklar
{
    public class UrunOzellikTanimi : BaseEntity
    {
        public string Ad { get; set; } = string.Empty;
        public string Kod { get; set; } = string.Empty;
        public string UrunTipi { get; set; } = "Genel";
        public string AlanTipi { get; set; } = "text";
        public string YardimMetni { get; set; } = string.Empty;
        public string Secenekler { get; set; } = string.Empty;
        public bool FiltredeGoster { get; set; }
        public bool DetaySayfasindaGoster { get; set; } = true;
        public bool TeknikTablodaGoster { get; set; } = true;
        public bool AktifMi { get; set; } = true;
        public int Sira { get; set; }

        public virtual ICollection<UrunOzellikDegeri> UrunOzellikDegerleri { get; set; } = new List<UrunOzellikDegeri>();

        [NotMapped]
        public IReadOnlyList<string> SecenekListesi =>
            (Secenekler ?? string.Empty)
                .Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
    }
}
