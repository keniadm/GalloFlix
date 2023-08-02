using System.Net;
using System.Net.Mail;

namespace GalloFlix.Services;
public class EmailSender : IEmailSender
{
    public EmailSender()
    {
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        string fromMail = "gallozord@outlook.com";
        string fromPassword = "@Etec123#";

        MailMessage message = new()
        {
            From = new MailAddress(fromMail),
            Subject = subject,
            Body = "<html><body>" + htmlMessage + "</body></html>",
            IsBodyHtml = true
        };
        message.To.Add(new MailAddress(email));
        var smtpClient = new SmtpClient("smtp-mail.outlook.com") 
        {
            Port = 587,
            Credentials = new NetworkCredential(fromMail, fromPassword),
            EnableSsl = true
        };
        await smtpClient.SendMailAsync(message);
    }
}