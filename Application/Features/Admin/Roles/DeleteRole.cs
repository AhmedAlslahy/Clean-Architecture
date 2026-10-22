using Application.Common.Abstraction;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Application.Common.Errors;
using Application.Common.Interfaces;

namespace Application.Features.Admin.Roles;

public static class DeleteRole
{
    public sealed record Command(int RoleId) : IRequest<Result>;

    public sealed class Handler(IApplicationDbContext context, ICacheService cache) : IRequestHandler<Command, Result>
    {
        private static readonly HashSet<string> ProtectedRoles = new(StringComparer.OrdinalIgnoreCase)
        {
            "User",
            "Admin"
        };

        public async Task<Result> Handle(Command req, CancellationToken cancellationToken = default)
        {
            var role = await context.Roles
                            .FirstOrDefaultAsync(r => r.Id == req.RoleId, cancellationToken);

            if (role is null)
            {
                return RoleErrors.NotFound;
            }

            if (ProtectedRoles.Contains(role.Name))
            {
                return RoleErrors.CannotDeleteSystemRole;
            }

            context.Roles.Remove(role);

            await context.SaveChangesAsync(cancellationToken);

            await cache.RemoveAsync("roles:all", cancellationToken);

            return Result.Success();
        }
    }
}