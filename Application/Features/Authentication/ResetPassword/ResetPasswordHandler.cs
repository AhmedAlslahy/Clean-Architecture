using Domain.Common;
using Domain.Enums;
using Domain.Errors;
using Domain.Identity;
using Infrastructure.Persistence;
using Infrastructure.Service.Shared;
using MediatR;

namespace Application.Features.Authentication.ResetPassword;

public class ResetPasswordHandler(SarhneDbContext context, ICurrentUser currentUser) : IRequestHandler<ResetPasswordCommand, Result>
{
    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellation)
    {
        var user = await context.Users.FindByIdAsync(currentUser.UserId!);
        if (user == null)
        {
            return UserErrors.NotFound;
        }

        var verify = PasswordService.VerifyPassword(request.CurrentPassword, user.PasswordHashed);
        if (!verify)
        {
            return new Error("Wrong Password", "Current password is incorrect", ErrorType.BadRequest);
        }

        var NewpasswordHash = PasswordService.HashPassword(request.NewPassword);
        user.PasswordHashed = NewpasswordHash;
        await context.SaveChangesAsync();

        return Result.Success();
    }
}