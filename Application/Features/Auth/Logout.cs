using Application.Common.Abstraction;
using Application.Common.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth;

public static class Logout
{
    public sealed record Command(string RefreshToken) : IRequest<Result>;

    public sealed class Validator
        : AbstractValidator<Command>
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

    public sealed class Handler(
        IApplicationDbContext context,
        IRefreshTokenService refreshTokenService)
        : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command req, CancellationToken cancellationToken)
        {
            var tokenHash = refreshTokenService.HashToken(req.RefreshToken);

            var refreshToken = await context.RefreshTokens
                .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

            if (refreshToken is null || refreshToken.RevokedOn is not null)
            {
                return Result.Success();
            }

            refreshToken.RevokedOn = DateTime.UtcNow;
            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}