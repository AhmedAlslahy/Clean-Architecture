using Domain.Common;
using Domain.Enums;
using Domain.Errors;
using Domain.Identity;
using Infrastructure.Persistence;
using Infrastructure.Service.Shared;
using MediatR;

namespace Application.Features.Authentication.ConfirmEmail;

public class ConfirmEmailHandler(SarhneDbContext context, ICurrentUser currentUser) : IRequestHandler<ConfirmEmailCommand, Result>
{
    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellation)
    {
        var user = await context.Users.FindByIdAsync(currentUser.UserId!);
        if (user == null)
        {
            return UserErrors.NotFound;
        }
        if (request.OTP != user.OTP || DateTime.UtcNow > user.OTPExpire)
        {
            return new Error("The OTP Is Wrong Or Expire", "Cannot Confirm Email", ErrorType.BadRequest);
        }

        if (user.EmailConfirmed)
        {
            return new Error("Email already confirmed", "Cannot Confirm Email", ErrorType.BadRequest);
        }
        user.OTP = null;
        user.OTPExpire = null;
        user.EmailConfirmed = true;
        await context.SaveChangesAsync();

        return Result.Success();
    }
}