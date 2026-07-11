namespace Infrastructure.Service.Email;

public interface IEmailService
{
    Task<Result> EmailBody(string to, string subject, string body, CancellationToken cancellation = default);
}