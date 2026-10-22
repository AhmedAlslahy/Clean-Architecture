using Application.Common.Abstraction;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.Permission;

public static class GetMyPermissions
{
    public sealed record Query : IRequest<Result<List<string>>>;

    public sealed class Handler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        ICacheService cache)
        : IRequestHandler<Query, Result<List<string>>>
    {
        public async Task<Result<List<string>>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var cacheKey = $"permissions:user:{currentUser.UserId}";

            var cachedPermissions = await cache
                .GetAsync<List<string>>(cacheKey, cancellationToken);
            if (cachedPermissions is not null)
            {
                return Result<List<string>>.Success(cachedPermissions);
            }

            var permissions = await context.UserRoles
                .AsNoTracking()
                .Where(ur => ur.UserId == currentUser.UserId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .ToListAsync(cancellationToken);

            await cache.SetAsync(cacheKey, permissions, TimeSpan.FromHours(12), cancellationToken);

            return Result<List<string>>.Success(permissions);
        }
    }
}