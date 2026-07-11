using Domain.Common;
using Domain.Errors;
using Domain.Identity;
using Infrastructure.Persistence;
using Infrastructure.Service.JWT;
using MediatR;

namespace Application.Features.Authentication.Login;

public class LoginHandler(SarhneDbContext context, IJwtService jwtService) : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return UserErrors.NotFound;
        }

        var passwordValid = PasswordService.VerifyPassword(request.Password, user.PasswordHashed);
        if (!passwordValid)
        {
            return AuthErrors.InvalidPassword;
        }

        var roles = await context.Users.GetRolesAsync(user.Id);
        var tokenResult = await jwtService.GenerateToken(user, roles);

        if (!tokenResult.IsSuccess)
        {
            return tokenResult.Failure;
        }
        var data = new LoginResponse
        {
            Token = tokenResult.Data.Token,
            ExpireIn = tokenResult.Data.ExpireIn
        };

        return data;
    }
}