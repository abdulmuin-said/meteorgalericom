namespace KanvasProje.Core.Helpers
{
    public static class SiparisDurumHelper
    {
        public const int SiparisAlindi = 0;
        public const int UretimHazirlaniyor = 1;
        public const int Paketleniyor = 8;
        public const int KargoyaVerildi = 2;
        public const int TeslimEdildi = 3;
        public const int IptalEdildi = 4;
        public const int IadeTalebi = 5;
        public const int IadeOnaylandi = 6;
        public const int IadeTamamlandi = 7;

        public static string GetLabel(int durum) => durum switch
        {
            SiparisAlindi => "Sipariş Alındı",
            UretimHazirlaniyor => "Üretim Hazırlanıyor",
            Paketleniyor => "Paketleniyor",
            KargoyaVerildi => "Kargoya Verildi",
            TeslimEdildi => "Teslim Edildi",
            IptalEdildi => "İptal Edildi",
            IadeTalebi => "İade Talebi Alındı",
            IadeOnaylandi => "İade Onaylandı",
            IadeTamamlandi => "İade Tamamlandı",
            _ => "Sipariş Durumu Güncellendi"
        };

        public static string GetShortLabel(int durum) => durum switch
        {
            SiparisAlindi => "Yeni",
            UretimHazirlaniyor => "Üretimde",
            Paketleniyor => "Paketleniyor",
            KargoyaVerildi => "Kargoda",
            TeslimEdildi => "Teslim",
            IptalEdildi => "İptal",
            IadeTalebi => "İade Talebi",
            IadeOnaylandi => "İade Onaylandı",
            IadeTamamlandi => "İade Tamamlandı",
            _ => "Güncellendi"
        };

        public static int GetProgressStep(int durum) => durum switch
        {
            SiparisAlindi => 1,
            UretimHazirlaniyor => 2,
            Paketleniyor => 3,
            KargoyaVerildi => 4,
            TeslimEdildi => 5,
            _ => 0
        };

        public static bool IsCancelled(int durum) => durum == IptalEdildi;
        public static bool IsReturn(int durum) => durum is IadeTalebi or IadeOnaylandi or IadeTamamlandi;
        public static bool IsShipped(int durum) => durum == KargoyaVerildi;
    }
}
