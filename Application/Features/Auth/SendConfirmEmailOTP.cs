using Application.Common.Abstraction;
using Application.Common.Errors;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities.Outbox;
using System.Security.Cryptography;
using System.Text.Json;
using FluentValidation;
using MediatR;

namespace Application.Features.Auth;

public static class SendConfirmEmailOTP
{
    public sealed record Command(string Email) : IRequest<Result>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.");
        }
    }

    public sealed class Handler(
        IApplicationDbContext context,
        IUserService users,
        IFilePathService filePathService)
        : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(
            Command req,
            CancellationToken cancellationToken)
        {
            var user = await users.FindByEmailAsync(req.Email);

            if (user is null)
                return UserErrors.NotFound;

            var otp = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

            user.OTP = otp;
            user.OTPExpire = DateTime.UtcNow.AddMinutes(5);

            var path = filePathService.GetPath("Templates/EmailConfirme.html");

            var htmlBody = await File.ReadAllTextAsync(
                path,
                cancellationToken);

            htmlBody = htmlBody
                .Replace("{{Name}}", user.FullName)
                .Replace("{{OTP}}", otp);

            var payload = new EmailOutboxPayload(
                user.Email,
                "Confirm Email",
                htmlBody);

            var outboxMessage = new OutboxMessage("SendEmail", JsonSerializer.Serialize(payload));

            context.OutboxMessages.Add(outboxMessage);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}