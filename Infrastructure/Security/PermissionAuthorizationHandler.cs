using Application.Common.Abstraction;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Security;

public sealed class PermissionAuthorizationHandler(
    IServiceScopeFactory serviceScopeFactory,
    ICacheService cache, ICurrentUserService currentUser)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var cacheKey = $"permissions:user:{currentUser.UserId}";

        var userPermissions =
            await cache.GetAsync<List<string>>(cacheKey);

        if (userPermissions is null)
        {
            using var scope =
                serviceScopeFactory.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<IApplicationDbContext>();

            userPermissions = await dbContext.UserRoles
                .AsNoTracking()
                .Where(ur => ur.UserId == currentUser.UserId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .ToListAsync();

            await cache.SetAsync(
                cacheKey,
                userPermissions,
                TimeSpan.FromHours(12));
        }

        if (userPermissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}