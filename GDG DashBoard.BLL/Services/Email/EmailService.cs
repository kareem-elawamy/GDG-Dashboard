using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace GDG_DashBoard.BLL.Services.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlContent)
    {
        var emailSettings = _config.GetSection("EmailSettings");
        var smtpServer = emailSettings["SmtpServer"] ?? throw new InvalidOperationException("SmtpServer not configured");
        var smtpPort = int.Parse(emailSettings["SmtpPort"]!);
        var smtpUser = emailSettings["SmtpUser"] ?? throw new InvalidOperationException("SmtpUser not configured");
        var smtpPass = emailSettings["SmtpPass"] ?? throw new InvalidOperationException("SmtpPass not configured");
        var fromEmail = emailSettings["FromEmail"] ?? throw new InvalidOperationException("FromEmail not configured");

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("GDG Dashboard", fromEmail));
        emailMessage.To.Add(new MailboxAddress("", to));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(TextFormat.Html)
        {
            Text = htmlContent
        };

        using var client = new SmtpClient();

        // Bypass strict SSL certificate validation on shared hosting (MonsterASP)
        client.ServerCertificateValidationCallback = (s, c, h, e) => true;

        await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(smtpUser, smtpPass);
        await client.SendAsync(emailMessage);
        await client.DisconnectAsync(true);
    }
}
