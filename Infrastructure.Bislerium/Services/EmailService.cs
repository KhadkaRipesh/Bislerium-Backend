using Application.Bislerium;
using Domain.Bislerium.DTOs.EmailHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.Bislerium.Services.EmailService;

namespace Infrastructure.Bislerium.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendMail(EmailMessage email)
        {
            string fromMail = "khadkanripesh@gmail.com";
            string password = "dasd ecss tvad hyer";

            using MailMessage message = new();
            message.From = new MailAddress(fromMail);
            message.Subject = email.Subject;
            message.To.Add(new MailAddress(email.To));
            message.Body = email.Body;
            message.IsBodyHtml = true;
            using SmtpClient smtpClient = new("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential(fromMail, password);
            smtpClient.EnableSsl = true;
            smtpClient.Send(message);
        }
    }
}
