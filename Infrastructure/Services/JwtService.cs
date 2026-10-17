using Application.Common.Abstraction;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities.Identity;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services;

public class JwtService(IOptions<JwtInformations> options) : IJwtService
{
    private readonly JwtInformations jwt = options.Value;

    public async Task<Result<GenerateTokenRes>> GenerateToken(User user, IList<string> roles)
    {
        List<Claim> UserClaims =
     [
         new(ClaimTypes.Name, user.UserName),
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    ];

        foreach (var role in roles)
        {
            UserClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        var SignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey));
        var SignInCred = new SigningCredentials(SignInKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken userToken = new JwtSecurityToken(
            audience: jwt.AudienceIP,
            issuer: jwt.IssuerIP,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: SignInCred,
            claims: UserClaims
         );
        var data = new GenerateTokenRes
        {
            Token = new JwtSecurityTokenHandler().WriteToken(userToken),
            ExpireIn = DateTime.UtcNow.AddHours(1),
        };

        return Result<GenerateTokenRes>.Success(data);
    }
}