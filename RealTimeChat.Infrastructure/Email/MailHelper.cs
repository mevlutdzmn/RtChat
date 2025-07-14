using System.Net;
using System.Net.Mail;

namespace RealTimeChat.Infrastructure.Email
{
    public class MailHelper
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _senderEmail = "mdizman124@gmail.com"; // Kendi Gmail adresin
        private readonly string _senderPassword = "ydnq jowk tvxq rtsk";  // Az önce aldığın uygulama şifresi

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var client = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_senderEmail, _senderPassword),
                EnableSsl = true
            };

            var mail = new MailMessage(_senderEmail, toEmail, subject, body)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mail);
        }
    }
}
