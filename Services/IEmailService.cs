using System.Net;
using System.Net.Mail;

namespace PetShop.Services
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            try
            {
                var smtp = new SmtpClient(_config["Smtp:Host"])
                {
                    Port = int.Parse(_config["Smtp:Port"]),
                    Credentials = new NetworkCredential(_config["Smtp:User"], _config["Smtp:Pass"]),
                    EnableSsl = true
                };

                var message = new MailMessage(_config["Smtp:From"], to, subject, body);
                await smtp.SendMailAsync(message);
            }
            catch
            {
                throw;
            }
        }
    }
}
