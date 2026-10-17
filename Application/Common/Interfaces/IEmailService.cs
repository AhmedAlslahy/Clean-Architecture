using Application.Common.Abstraction;

namespace Application.Common.Interfaces;

public interface IEmailService
{
    Task<Result> EmailBody(string to, string subject, string body, CancellationToken cancellation = default);
}