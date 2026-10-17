using Application.Common.Abstraction;
using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Enums;
using FluentValidation;
using MediatR;

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
                .NotEmpty()
                .MinimumLength(8)
                .WithMessage("New password must be at least 8 characters.")
                .Matches("[A-Z]")
                .WithMessage("Must contain uppercase letter.")
                .Matches("[a-z]")
                .WithMessage("Must contain lowercase letter.")
                .Matches("[0-9]")
                .WithMessage("Must contain number.");
        }
    }

    public sealed class Handler(
        IApplicationDbContext context,
        IUserService users,
        IPasswordService passwordService,
        ICurrentUserService currentUser)
        : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(
            Command req,
            CancellationToken cancellationToken)
        {
            var user = await users.FindByIdAsync(currentUser.UserId);

            if (user == null)
            {
                return UserErrors.NotFound;
            }

            var verify = passwordService.VerifyPassword(
                req.CurrentPassword,
                user.PasswordHashed);

            if (!verify)
            {
                return new Error(
                    "Wrong Password",
                    "Current password is incorrect",
                    ErrorType.BadRequest);
            }

            user.PasswordHashed = passwordService.HashPassword(req.NewPassword);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}