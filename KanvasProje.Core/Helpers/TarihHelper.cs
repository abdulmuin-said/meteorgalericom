namespace KanvasProje.Core.Helpers;

/// <summary>
/// Tarih ile ilgili yardımcı metotlar.
/// Veritabani UTC kaydeder, ekranda Türkiye saati (UTC+3) gösterilir.
/// </summary>
public static class TarihHelper
{
    /// <summary>
    /// Türkiye saat dilimi (UTC+3 / GMT+3) — yaz saati/kiş saati farketmez.
    /// </summary>
    private static readonly TimeZoneInfo TurkeyTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");

    /// <summary>
    /// UTC DateTime'i Türkiye saatine (UTC+3) çevirir.
    /// </summary>
    public static DateTime ToTurkeyTime(this DateTime utcDateTime)
    {
        if (utcDateTime.Kind == DateTimeKind.Unspecified)
        {
            // Eğer Kind belirtilmemişse UTC kabul et
            utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
        }

        return TimeZoneInfo.ConvertTime(utcDateTime, TurkeyTimeZone);
    }

    /// <summary>
    /// Türkiye saatinde "dd.MM.yyyy HH:mm" formatinda string döndürür.
    /// </summary>
    public static string ToTurkeyString(this DateTime utcDateTime, string format = "dd.MM.yyyy HH:mm")
    {
        return utcDateTime.ToTurkeyTime().ToString(format);
    }

    /// <summary>
    /// Nullable DateTime için Türkiye saati formatı.
    /// Null ise "-" döndürür.
    /// </summary>
    public static string ToTurkeyString(this DateTime? utcDateTime, string format = "dd.MM.yyyy HH:mm")
    {
        return utcDateTime.HasValue
            ? utcDateTime.Value.ToTurkeyTime().ToString(format)
            : "-";
    }
}
