using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace KanvasProje.Core.Helpers
{
    public static class SlugHelper
    {
        /// <summary>
        /// Türkçe karakterleri İngilizce'ye çevirir ve URL-friendly slug oluşturur
        /// Örnek: "Atatürk Portresi 50x70 cm" → "ataturk-portresi-50x70-cm"
        /// </summary>
        public static string GenerateSlug(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // 1. Küçük harfe çevir
            text = text.ToLowerInvariant();

            // 2. Türkçe karakterleri değiştir
            text = text.Replace("ç", "c")
                       .Replace("ğ", "g")
                       .Replace("ı", "i")
                       .Replace("İ", "i")
                       .Replace("ö", "o")
                       .Replace("ş", "s")
                       .Replace("ü", "u");

            // 3. Aksanlı karakterleri temizle (è, é, ê vb.)
            text = RemoveDiacritics(text);

            // 4. Sadece harf, rakam ve boşluk bırak
            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");

            // 5. Birden fazla boşluğu tek tire yap
            text = Regex.Replace(text, @"\s+", "-");

            // 6. Birden fazla tire varsa tek tire yap
            text = Regex.Replace(text, @"-+", "-");

            // 7. Baş ve sondaki tireleri temizle
            text = text.Trim('-');

            // 8. Maksimum 100 karakter (SEO için)
            if (text.Length > 100)
                text = text.Substring(0, 100).Trim('-');

            return text;
        }

        /// <summary>
        /// Aksanlı karakterleri kaldırır (è → e, é → e vb.)
        /// </summary>
        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Slug'ın unique olup olmadığını kontrol et, değilse sonuna sayı ekle
        /// Örnek: "ataturk-portresi" → "ataturk-portresi-2"
        /// </summary>
        public static string EnsureUnique(string slug, List<string> existingSlugs)
        {
            if (!existingSlugs.Contains(slug))
                return slug;

            int counter = 2;
            string newSlug;
            
            do
            {
                newSlug = $"{slug}-{counter}";
                counter++;
            } while (existingSlugs.Contains(newSlug));

            return newSlug;
        }
    }
}
