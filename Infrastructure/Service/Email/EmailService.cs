using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Infrastructure.Service.Email;

public class EmailService(IOptions<EmailInformations> options) : IEmailService
{
    private readonly EmailInformations emailInformations = options.Value;

    public async Task<Result> EmailBody(string to, string subject, string body, CancellationToken cancellation = default)
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
        return Result.Success();
    }
}