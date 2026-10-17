using Application.Common.Interfaces;
using FluentValidation;
using MediatR;
using Application.Common.Abstraction;
using Application.Common.Errors;
using Domain.Enums;

namespace Application.Features.Auth;

public static class ConfirmEmail
{
    public sealed record Command(string OTP) : IRequest<Result>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.OTP)
                .NotEmpty()
                .Matches(@"^\d{6}$");
        }
    }

    public sealed class Handler(IApplicationDbContext context,
        ICurrentUserService currentUser, IUserService users)
        : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command req, CancellationToken cancellationToken)
        {
            var user = await users.FindByIdAsync(currentUser.UserId);

            if (user == null)
            {
                return UserErrors.NotFound;
            }

            if (req.OTP != user.OTP || DateTime.UtcNow > user.OTPExpire)
            {
                return new Error("The OTP Is Wrong Or Expire", "Cannot Confirm Email", ErrorType.BadRequest);
            }

            if (user.EmailConfirmed)
            {
                return new Error(
                    "Email already confirmed",
                    "Cannot Confirm Email",
                    ErrorType.BadRequest);
            }

            user.OTP = null;
            user.OTPExpire = null;
            user.EmailConfirmed = true;

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}