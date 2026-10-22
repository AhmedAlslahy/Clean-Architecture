using Application.Common.Abstraction;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.Permissions;

public static class GetGroupedPermissions
{
    public sealed record PermissionRes(int Id, string Name);

    public sealed record GroupedPermissionsRes(
        string GroupName,
        List<PermissionRes> Permissions);

    public sealed record Query : IRequest<Result<List<GroupedPermissionsRes>>>;

    public sealed class Handler(IApplicationDbContext context, ICacheService cache)
        : IRequestHandler<Query, Result<List<GroupedPermissionsRes>>>
    {
        private const string CacheKey = "permissions:grouped:all";

        public async Task<Result<List<GroupedPermissionsRes>>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var cachedPermissions = await cache.GetAsync<List<GroupedPermissionsRes>>(
                CacheKey,
                cancellationToken);

            if (cachedPermissions is not null)
            {
                return Result<List<GroupedPermissionsRes>>.Success(cachedPermissions);
            }

            var permissions = await context.Permissions
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var groupedPermissions = permissions
                .GroupBy(p => p.Group)
                .Select(g => new GroupedPermissionsRes(
                    GroupName: g.Key,
                    Permissions: g.Select(p => new PermissionRes(p.Id, p.Name)).ToList()
                ))
                .OrderBy(g => g.GroupName).ToList();

            await cache.SetAsync(
                CacheKey,
                groupedPermissions,
                TimeSpan.FromHours(24),
                cancellationToken);

            return Result<List<GroupedPermissionsRes>>.Success(groupedPermissions);
        }
    }
}