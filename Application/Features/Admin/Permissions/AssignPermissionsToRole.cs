using Application.Common.Abstraction;
using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.Permissions;

public static class AssignPermissionsToRole
{
    public sealed record Command(int RoleId, List<int> PermissionIds) : IRequest<Result>;

    public sealed class Handler(IApplicationDbContext context, ICacheService cache)
        : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var role = await context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(
                    r => r.Id == request.RoleId,
                    cancellationToken);

            if (role is null)
            {
                return RoleErrors.NotFound;
            }

            var permissionIds = request.PermissionIds.Distinct().ToList();

            var validPermissionIds = await context.Permissions
                .Where(p => permissionIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);

            if (validPermissionIds.Count != permissionIds.Count)
            {
                return PermissionErrors.NotFound;
            }

            role.RolePermissions.Clear();

            foreach (var permissionId in validPermissionIds)
            {
                role.RolePermissions.Add(new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId
                });
            }

            await context.SaveChangesAsync(cancellationToken);

            await cache.RemoveAsync(
                $"permissions:role:{request.RoleId}:all",
                cancellationToken);

            return Result.Success();
        }
    }
}