using Application.Common.Abstraction;
using Application.Common.Errors;
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
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters.");
        }
    }

    public sealed class Handler(
        IApplicationDbContext context,
        IPasswordService passwordService,
        IUserService users,
        IRefreshTokenService refreshTokenService,
        IJwtService jwtService)
        : IRequestHandler<Command, Result<LoginRes>>
    {
        public async Task<Result<LoginRes>> Handle(
            Command req,
            CancellationToken cancellationToken)
        {
            var user = await users.FindByEmailAsync(req.Email);

            if (user == null)
            {
                return UserErrors.NotFound;
            }

            var passwordValid = passwordService.VerifyPassword(
                req.Password,
                user.PasswordHashed);

            if (!passwordValid)
            {
                return AuthErrors.InvalidPassword;
            }

            var roles = await users.GetRolesAsync(user.Id);

            var tokenResult = await jwtService.GenerateToken(user, roles);

            if (!tokenResult.IsSuccess)
            {
                return tokenResult.Failure;
            }

            // Generate Refresh Token
            var refreshToken = refreshTokenService.GenerateToken();

            var refreshTokenEntity = new Domain.Entities.Identity.RefreshToken
            {
                TokenHash = refreshTokenService.HashToken(refreshToken),
                UserId = user.Id,
                CreatedOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(7)
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