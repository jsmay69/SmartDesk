using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SmartDesk.Application.Configurations;
using SmartDesk.Application.Interfaces;
using SmartDesk.Domain.Entities;
using System;
using System.Threading.Tasks;
using MailKitSmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace SmartDesk.Infrastructure.Services
{
    public class SmtpReminderService : IReminderService
    {
        private readonly SmtpSettings _settings;

        public SmtpReminderService(IOptions<SmtpSettings> opts)
        {
            _settings = opts.Value;
        }

        public async Task SendReminderAsync(TodoItem item)
        {
            // Convert UTC due date to local time
            var dueUtc = item.DueDate ?? DateTime.UtcNow;
            var dueLocal = dueUtc.ToLocalTime();

            // Build the email
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_settings.From));
            message.To.Add(MailboxAddress.Parse(_settings.To));
            message.Subject = $"Reminder: {item.Title} is due at {dueLocal:yyyy-MM-dd HH:mm zzz}";
            message.Body = new TextPart("plain")
            {
                Text = $@"Hi,

This is a reminder that the task '{item.Title}' is due on {dueLocal:yyyy-MM-dd HH:mm zzz} (your local time).

Description: {item.Description}

Regards,
SmartDesk Agent"
            };

            // Send via MailKit SMTP
            using var client = new MailKitSmtpClient();
            await client.ConnectAsync(
                _settings.Host,
                _settings.Port,
                _settings.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls
            );
            if (!string.IsNullOrWhiteSpace(_settings.Username))
                await client.AuthenticateAsync(_settings.Username, _settings.Password);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
