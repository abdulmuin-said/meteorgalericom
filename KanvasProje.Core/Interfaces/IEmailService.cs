namespace KanvasProje.Core.Interfaces
{
    public interface IEmailService
    {
        // Kime, Konu, İçerik (HTML olabilir)
        Task SendMailAsync(string to, string subject, string body);
        Task SendTemplateMailAsync(string to, string baslik, string adSoyad, string icerik, string btnLink = "", string btnYazi = "");
        Task<bool> SendKargoNotificationEmail(string toEmail, string musteriAdi, string siparisNo, string kargoFirmasi, string kargoTakipNo);
        // Fatura maili için attachment destekli method
        Task<bool> SendInvoiceEmailAsync(string toEmail, string musteriAdi, string siparisNo, string filePath);
    }
}