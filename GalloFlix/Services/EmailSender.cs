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

        MailMessage message = new();
        message.From = new MailAddress(fromMail);
        message.Subject = subject;
        message.To.Add(new MailAddress(email));
        message.Body = "<html><body>" + htmlMessage + "</body></html>";
        message.IsBodyHtml = true;
        var smtpClient = new SmtpClient("smtp-mail.outlook.com") 
        {
            Port = 587,
            Credentials = new NetworkCredential(fromMail, fromPassword),
            EnableSsl = true
        };
        await smtpClient.SendMailAsync(message);
    }
}