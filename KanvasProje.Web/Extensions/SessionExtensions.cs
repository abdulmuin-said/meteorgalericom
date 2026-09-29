using System.Text.Json;

namespace KanvasProje.Web.Extensions
{
    public static class SessionExtensions
    {
        // Nesneyi JSON'a çevirip saklar
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        // JSON'u tekrar Nesneye çevirip okur
        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }



        // Nesneyi JSON'a çevirip Session'a kaydeder
        public static void SetJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        // Session'daki JSON verisini tekrar nesneye çevirir
        public static T? GetJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
        
    }
}