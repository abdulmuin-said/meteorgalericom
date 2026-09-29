using System.ComponentModel.DataAnnotations.Schema;

namespace KanvasProje.Core.Varliklar
{
    public class UrunOzellikDegeri : BaseEntity
    {
        [ForeignKey("Urun")]
        public int UrunId { get; set; }
        public Urun Urun { get; set; } = default!;

        [ForeignKey("UrunOzellikTanimi")]
        public int UrunOzellikTanimiId { get; set; }
        public UrunOzellikTanimi UrunOzellikTanimi { get; set; } = default!;

        public string Deger { get; set; } = string.Empty;

        [NotMapped]
        public string DegerGosterimi =>
            Deger switch
            {
                "true" => "Evet",
                "false" => "Hayir",
                _ => Deger
            };
    }
}
