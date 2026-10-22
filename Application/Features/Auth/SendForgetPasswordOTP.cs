using Application.Common.Abstraction;
using Application.Common.Errors;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities.Outbox;
using FluentValidation;
using MediatR;
using System.Security.Cryptography;
using System.Text.Json;

namespace Application.Features.Auth;

public static class SendForgetPasswordOTP
{
    public sealed record Command(string Email) : IRequest<Result>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
             .Cascade(CascadeMode.Stop)
             .NotEmpty()
             .WithMessage("Email is required.")
             .MaximumLength(254)
             .WithMessage("Email must not exceed 254 characters.")
             .EmailAddress()
             .WithMessage("Invalid email format.");
        }
    }

    public sealed class Handler(
        IApplicationDbContext context,
        IFilePathService filePathService)
        : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(
            Command req,
            CancellationToken cancellationToken)
        {
            var user = await context.Users.FindByEmailAsync(req.Email, cancellationToken);

            if (user is null)
                return UserErrors.NotFound;

            var otp = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

            var htmlPath = filePathService.GetPath("Templates/ForgetPassword.html");

            var htmlBody = await File.ReadAllTextAsync(htmlPath, cancellationToken);

            htmlBody = htmlBody.Replace("{{Name}}", user.FullName).Replace("{{OTP}}", otp);

            var emailPayload = new EmailOutboxPayload(
                user.Email,
                "Forget Password",
                htmlBody);

            var outboxMessage = new OutboxMessage(
                "SendEmail",
                JsonSerializer.Serialize(emailPayload));

            user.OTP = otp;
            user.OTPExpire = DateTime.UtcNow.AddMinutes(5);

            context.OutboxMessages.Add(outboxMessage);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}