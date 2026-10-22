using Application.Common.Abstraction;
using Application.Common.Errors;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth;

public static class RefreshToken
{
    public sealed class RefreshTokenRes
    {
        public required string Token { get; set; }
        public DateTime TokenExpireIn { get; set; }
        public required string RefreshToken { get; set; }
        public DateTime RefreshTokenExpireIn { get; set; }
    }

    public sealed record Command(string RefreshToken) : IRequest<Result<RefreshTokenRes>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.RefreshToken)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Refresh token is required.")
                .MaximumLength(256)
                .WithMessage("Refresh token is too long.");
        }
    }

    public sealed class Handler(IApplicationDbContext context,
    IJwtService jwtService,
    IRefreshTokenService refreshTokenService)
    : IRequestHandler<Command, Result<RefreshTokenRes>>
    {
        public async Task<Result<RefreshTokenRes>> Handle(
            Command req,
            CancellationToken cancellationToken)
        {
            var tokenHash = refreshTokenService.HashToken(req.RefreshToken);

            var storedToken = await context.RefreshTokens
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

            if (storedToken is null || !storedToken.IsActive)
            {
                return AuthErrors.InvalidRefreshToken;
            }

            var user = storedToken.User;

            var roles = await context.Users.GetRolesAsync(user.Id, cancellationToken);

            // Generate new Access Token
            var tokenResult = await jwtService.GenerateToken(user, roles);

            if (!tokenResult.IsSuccess)
            {
                return tokenResult.Failure;
            }

            // Generate new Refresh Token
            var newRefreshToken = refreshTokenService.GenerateToken();

            var newRefreshTokenHash = refreshTokenService.HashToken(newRefreshToken);

            var now = DateTime.UtcNow;
            var newRefreshTokenEntity = new Domain.Entities.Identity.RefreshToken
            {
                TokenHash = newRefreshTokenHash,
                UserId = user.Id,
                CreatedOn = now,
                ExpiresOn = now.AddDays(7)
            };

            // Revoke old token
            storedToken.RevokedOn = now;
            storedToken.ReplacedByTokenHash = newRefreshTokenHash;
            context.RefreshTokens.Add(newRefreshTokenEntity);
            await context.SaveChangesAsync(cancellationToken);

            return new RefreshTokenRes
            {
                Token = tokenResult.Data!.Token,
                TokenExpireIn = tokenResult.Data.ExpireIn,
                RefreshToken = newRefreshToken,
                RefreshTokenExpireIn = newRefreshTokenEntity.ExpiresOn
            };
        }
    }
}