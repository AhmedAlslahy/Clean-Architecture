using Application.Common.Abstraction;
using Application.Common.Errors;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Enums;
using FluentValidation;
using MediatR;

namespace Application.Features.Auth;

public static class ForgetPassword
{
    public sealed record Command(string Email, string OTP, string NewPassword) : IRequest<Result>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(254);

            RuleFor(x => x.OTP)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Matches(@"^\d{6}$");

            RuleFor(x => x.NewPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(128)
                .Matches("[A-Z]")
                .WithMessage("Password must contain an uppercase letter.")
                .Matches("[a-z]")
                .WithMessage("Password must contain a lowercase letter.")
                .Matches("[0-9]")
                .WithMessage("Password must contain a number.");
        }
    }

    public sealed class Handler(IApplicationDbContext context, IPasswordService passwordService)
        : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(
            Command req,
            CancellationToken cancellationToken)
        {
            var user = await context.Users.FindByEmailAsync(req.Email, cancellationToken);

            if (user == null)
            {
                return UserErrors.NotFound;
            }

            if (req.OTP != user.OTP || DateTime.UtcNow > user.OTPExpire)
            {
                return new Error("The OTP Is Wrong Or Expire", "Cannot add new password", ErrorType.BadRequest);
            }

            user.OTP = null;
            user.OTPExpire = null;
            user.PasswordHashed = passwordService.HashPassword(req.NewPassword);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}