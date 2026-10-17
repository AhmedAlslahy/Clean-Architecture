using Application.Common.Abstraction;
using Application.Common.Interfaces;
using Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Services;

public class EmailService(IOptions<EmailInformations> options
    , ILogger<EmailService> logger) : IEmailService
{
    private readonly EmailInformations emailInformations = options.Value;

    public async Task<Result> EmailBody(string to, string subject, string body, CancellationToken cancellation = default)
    {
        try
        {
            using (var Client = new SmtpClient())
            {
                await Client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls, cancellation);
                await Client.AuthenticateAsync(emailInformations.Email, emailInformations.Password, cancellation);

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body,
                };

                var message = new MimeMessage
                {
                    Body = bodyBuilder.ToMessageBody()
                };
                message.From.Add(MailboxAddress.Parse(emailInformations.Email));
                message.To.Add(MailboxAddress.Parse(to));
                message.Subject = subject;
                await Client.SendAsync(message, cancellation);
                await Client.DisconnectAsync(true, cancellation);
            }

            logger.LogInformation("SMTP accepted the message for recipient {Recipient}", to);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {Recipient}", to);

            throw;
        }
    }
}