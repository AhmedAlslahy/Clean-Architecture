using Application.Common.Abstraction;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.Permissions;

public static class GetRolePermissions
{
    public sealed record PermissionRes(int Id, string Name);
    public sealed record Query(int RoleId) : IRequest<Result<List<PermissionRes>>>;

    public sealed class Handler(IApplicationDbContext context, ICacheService cache)
        : IRequestHandler<Query, Result<List<PermissionRes>>>
    {
        public async Task<Result<List<PermissionRes>>> Handle(
            Query req,
            CancellationToken cancellationToken)
        {
            var key = $"permissions:role:{req.RoleId}:all";
            var cachedPermissions = await cache
                .GetAsync<List<PermissionRes>>(key, cancellationToken);

            if (cachedPermissions != null)
            {
                return Result<List<PermissionRes>>.Success(cachedPermissions);
            }

            var permissions = await context.RolePermissions.Where(rp => rp.RoleId == req.RoleId)
                 .Select(rp => new PermissionRes(rp.Permission.Id, rp.Permission.Name))
                 .ToListAsync(cancellationToken);

            await cache.SetAsync(key, permissions,
                TimeSpan.FromHours(24),
                cancellationToken);

            return Result<List<PermissionRes>>.Success(permissions);
        }
    }
}