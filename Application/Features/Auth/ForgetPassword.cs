using Application.Common.Abstraction;
using Application.Common.Errors;
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
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.OTP)
                .NotEmpty()
                .Matches(@"^\d{6}$");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(8);
        }
    }

    public sealed class Handler(IApplicationDbContext context, IUserService users, IPasswordService passwordService)
        : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(
            Command req,
            CancellationToken cancellationToken)
        {
            var user = await users.FindByEmailAsync(req.Email);

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