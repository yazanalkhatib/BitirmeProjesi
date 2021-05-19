using BitirmeProjesiWeb.CustomAttributes;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BitirmeProjesiWeb.Utilities
{
    public class Mail : IMailing
    {
        [CustomRequiredStar]
        [DisplayName("Ad Soyad")]
        public string SenderFullName { get; set; }
        [CustomRequiredStar]
        [DisplayName("Email")]
        public string SenderEmailAddress { get; set; }
        [CustomRequiredStar]
        [DisplayName("Mesaj")]
        public string Message { get; set; }

        public bool? Sent { get; set; } = null;

        static readonly string From = "";
        static readonly string To = "";

        public Task<bool> Send()
        {
            try
            {
                MailMessage mailMessage = new MailMessage(From, To)
                {
                    Subject = SenderEmailAddress + " - " + SenderFullName,
                    Body = Message,
                };

                SmtpClient smtpClient = new SmtpClient("", 8889)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(From, "")
                };

                smtpClient.Send(mailMessage);
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }
    }
}