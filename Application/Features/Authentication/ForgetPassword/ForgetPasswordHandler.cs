using Domain.Common;
using Domain.Enums;
using Domain.Errors;
using Domain.Identity;
using Infrastructure.Persistence;
using MediatR;

namespace Application.Features.Authentication.ForgetPassword;

public class ForgetPasswordHandler(SarhneDbContext context) : IRequestHandler<ForgetPasswordCommand, Result>
{
    public async Task<Result> Handle(ForgetPasswordCommand request, CancellationToken cancellation)
    {
        var user = await context.Users.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return UserErrors.NotFound;
        }
        if (request.OTP != user.OTP || DateTime.UtcNow > user.OTPExpire)
        {
            return new Error("The OTP Is Wrong Or Expire", "Cannot add new password", ErrorType.BadRequest);
        }
        user.OTP = null;
        user.OTPExpire = null;
        var passwordHash = PasswordService.HashPassword(request.NewPassword);
        user.PasswordHashed = passwordHash;
        await context.SaveChangesAsync();

        return Result.Success();
    }
}