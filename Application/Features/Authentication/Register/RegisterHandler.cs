using Domain.Common;
using Domain.Errors;
using Domain.Identity;
using Infrastructure.Persistence;
using MediatR;

namespace Application.Features.Authentication.Register;

public class RegisterHandler(SarhneDbContext context) : IRequestHandler<RegisterCommand, Result>
{
    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellation)
    {
        var existingUser = await context.Users.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return UserErrors.AlreadyExists;
        }

        var user = new Domain.Identity.User
        {
            Email = request.Email,
            NormalizedEmail = request.Email.ToUpperInvariant(),
            FullName = request.FullName,
            UserName = request.UserName,
            NormalizedUserName = request.UserName.ToUpperInvariant(),
            PasswordHashed = PasswordService.HashPassword(request.Password),
            EmailConfirmed = false,
            UserSetting = new Domain.Entities.UserSetting()
        };

        var result = await context.Users.AddAsync(user);
        await context.AddRoleAsync(user, "User");
        await context.SaveChangesAsync(cancellation);
        return Result.Success();
    }
}