using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace TvBroadCast.Services
{
    public class EmailSender : IEmailSender
    {

        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public Task SendEmailAsync(string email , string subject , string htmlMessage)
        {
            var smtpClient = new SmtpClient(_config["Smtp:Host"])
            {
                Port = int.Parse(_config["Smtp:Port"]),
                Credentials = new NetworkCredential(_config["Smtp:Username"], _config["Smtp:Password"]),
                EnableSsl = bool.Parse(_config["Smtp:EnableSsl"])
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["Smtp:From"]),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            return smtpClient.SendMailAsync(mailMessage);
        }
        

    }
}
