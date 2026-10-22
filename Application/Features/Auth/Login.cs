using Application.Common.Abstraction;
using Application.Common.Errors;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.Features.Auth;

public static class Login
{
    public sealed class LoginRes
    {
        public required string Token { get; set; }
        public DateTime TokenExpireIn { get; set; }
        public required string RefreshToken { get; set; }
        public DateTime RefreshTokenExpireIn { get; set; }
    }

    public sealed record Command(
        string Email,
        string Password
    ) : IRequest<Result<LoginRes>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("Email is required.")
                .MaximumLength(254)
                    .WithMessage("Email must not exceed 254 characters.")
                .EmailAddress()
                    .WithMessage("Please enter a valid email address.");

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("Password is required.")
                .MaximumLength(128)
                    .WithMessage("Password must not exceed 128 characters.");
        }
    }

    public sealed class Handler(
        IApplicationDbContext context,
        IPasswordService passwordService,
        IRefreshTokenService refreshTokenService,
        IJwtService jwtService)
        : IRequestHandler<Command, Result<LoginRes>>
    {
        public async Task<Result<LoginRes>> Handle(
            Command req,
            CancellationToken cancellationToken)
        {
            var user = await context.Users.FindByEmailAsync(req.Email, cancellationToken);

            if (user == null)
            {
                return AuthErrors.InvalidCredentials;
            }

            var passwordValid = passwordService.VerifyPassword(
                req.Password,
                user.PasswordHashed);

            if (!passwordValid)
            {
                return AuthErrors.InvalidCredentials;
            }

            var roles = await context.Users.GetRolesAsync(user.Id, cancellationToken);

            var tokenResult = await jwtService.GenerateToken(user, roles);

            if (!tokenResult.IsSuccess)
            {
                return tokenResult.Failure;
            }

            // Generate Refresh Token
            var refreshToken = refreshTokenService.GenerateToken();
            var now = DateTime.UtcNow;
            var refreshTokenEntity = new Domain.Entities.Identity.RefreshToken
            {
                TokenHash = refreshTokenService.HashToken(refreshToken),
                UserId = user.Id,
                CreatedOn = now,
                ExpiresOn = now.AddDays(7)
            };

            context.RefreshTokens.Add(refreshTokenEntity);
            await context.SaveChangesAsync(cancellationToken);

            return new LoginRes
            {
                Token = tokenResult.Data!.Token,
                TokenExpireIn = tokenResult.Data.ExpireIn,
                RefreshToken = refreshToken,
                RefreshTokenExpireIn = refreshTokenEntity.ExpiresOn
            };
        }
    }
}