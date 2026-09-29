using System.IO;

namespace KanvasProje.Core.DTOs
{
    public class DosyaYuklemeDto
    {
        public string DosyaAdi { get; set; } = string.Empty;
        public Stream? IcerikAkisi { get; set; }
        public string ContentType { get; set; } = string.Empty;
        
        public bool GecerliMi => !string.IsNullOrEmpty(DosyaAdi) && IcerikAkisi != null && IcerikAkisi.Length > 0;
    }
}
