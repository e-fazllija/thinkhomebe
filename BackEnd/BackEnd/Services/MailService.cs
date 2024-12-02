using Azure.Core;
using BackEnd.Interfaces;
using BackEnd.Models.MailModels;
using BackEnd.Models.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BackEnd.Services;

public class MailService : IMailService
{
    private readonly MailOptions _mailSettings;
    public MailService(IOptions<MailOptions> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }
    
    public async Task SendEmailAsync(MailRequest mailRequest)
    {
        var email = new MimeMessage();
        email.Sender = MailboxAddress.Parse("info@thinkhome.it");
        email.From.Add(InternetAddress.Parse("info@thinkhome.it"));
        email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
        if (mailRequest.CcEmail.Count > 0)
        {
            foreach (var item in mailRequest.CcEmail)
            {
                email.Cc.Add(InternetAddress.Parse(item));
            }
        }
        email.Subject = mailRequest.Subject;
        var builder = new BodyBuilder();
        if (mailRequest.Attachments != null)
        {
            byte[] fileBytes;
            foreach (var file in mailRequest.Attachments)
            {
                if (file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                    builder.Attachments.Add(file.FileName, fileBytes, MimeKit.ContentType.Parse(file.ContentType));
                }
            }
        }
        builder.HtmlBody = mailRequest.Body;
        email.Body = builder.ToMessageBody();
        using var smtp = new SmtpClient();
        smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.SslOnConnect);
        smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
        await smtp.SendAsync(email);
        smtp.Disconnect(true);
    }
}