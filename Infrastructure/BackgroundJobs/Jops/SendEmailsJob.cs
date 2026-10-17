using Application.Common.Abstraction;
using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Text.Json;

namespace Infrastructure.BackgroundJobs.Jops;

[DisallowConcurrentExecution]
public sealed class SendEmailsJob(
    IApplicationDbContext context,
    IEmailService emailService,
    ILogger<SendEmailsJob> logger) : IJob
{
    public async ValueTask Execute(IJobExecutionContext jobContext, CancellationToken cancellationToken = default)
    {
        cancellationToken = jobContext.CancellationToken;

        var messages = await context.OutboxMessages
            .Where(x => !x.IsProcessed && x.Type == "SendEmail")
            .OrderBy(x => x.OccurredTime)
            .Take(20)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                var payload = JsonSerializer.Deserialize<EmailOutboxPayload>(message.Content);

                if (payload is null)
                {
                    message.Error = "Invalid email payload.";
                    continue;
                }

                await emailService.EmailBody(
                    payload.To,
                    payload.Subject,
                    payload.Body,
                    cancellationToken);

                message.MarkAsProcessed(DateTime.UtcNow);
                message.Error = null;
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                message.Error = ex.Message;

                logger.LogError(
                    ex,
                    "Failed to process Outbox message {MessageId}",
                    message.Id);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}