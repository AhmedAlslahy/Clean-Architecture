using Application.Common.Abstraction;
using Application.Common.Errors;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth;

public static class ResetPassword
{
    public sealed record Command(
        string CurrentPassword,
        string NewPassword
    ) : IRequest<Result>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .WithMessage("Current password is required.");

            RuleFor(x => x.NewPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("New password is required.")
                .MinimumLength(8)
                .WithMessage("New password must be at least 8 characters.")
                .MaximumLength(128)
                .WithMessage("New password must not exceed 128 characters.")
                .Matches("[A-Z]")
                .WithMessage("Password must contain an uppercase letter.")
                .Matches("[a-z]")
                .WithMessage("Password must contain a lowercase letter.")
                .Matches("[0-9]")
                .WithMessage("Password must contain a number.")
                .NotEqual(x => x.CurrentPassword)
                .WithMessage("New password must be different from current password.");
        }
    }

    public sealed class Handler(
        IApplicationDbContext context,
        IPasswordService passwordService,
        ICurrentUserService currentUser)
        : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(
            Command req,
            CancellationToken cancellationToken)
        {
            var userId = currentUser.UserId;

            var user = await context.Users.FindByIdAsync(
                userId,
                cancellationToken);

            if (user is null)
            {
                return UserErrors.NotFound;
            }

            var isCurrentPasswordValid = passwordService.VerifyPassword(
                req.CurrentPassword,
                user.PasswordHashed);

            if (!isCurrentPasswordValid)
            {
                return new Error(
                    "Auth.InvalidCurrentPassword",
                    "Current password is incorrect.",
                    ErrorType.BadRequest);
            }

            var isSamePassword = passwordService.VerifyPassword(
                req.NewPassword,
                user.PasswordHashed);

            if (isSamePassword)
            {
                return new Error(
                    "Auth.PasswordUnchanged",
                    "New password must be different from current password.",
                    ErrorType.BadRequest);
            }

            user.PasswordHashed = passwordService.HashPassword(
                req.NewPassword);

            // Revoke existing refresh tokens after password change.
            var now = DateTime.UtcNow;

            var activeRefreshTokens = context.RefreshTokens
                .Where(x =>
                    x.UserId == user.Id &&
                    x.RevokedOn == null &&
                    x.ExpiresOn > now);

            await foreach (var refreshToken in activeRefreshTokens
                .AsAsyncEnumerable()
                .WithCancellation(cancellationToken))
            {
                refreshToken.RevokedOn = now;
            }

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}