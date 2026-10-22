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

public sealed class JwtService(IOptions<JwtInformations> options)
    : IJwtService
{
    private readonly JwtInformations _jwt = options.Value;

    public Task<Result<GenerateTokenRes>> GenerateToken(
        User user,
        IList<string> roles)
    {
        var now = DateTime.UtcNow;
        var expires = now.AddHours(1);

        List<Claim> claims =
        [
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwt.SecretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.IssuerIP,
            audience: _jwt.AudienceIP,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler()
            .WriteToken(token);

        var data = new GenerateTokenRes
        {
            Token = tokenString,
            ExpireIn = expires
        };

        return Task.FromResult(
            Result<GenerateTokenRes>.Success(data));
    }
}