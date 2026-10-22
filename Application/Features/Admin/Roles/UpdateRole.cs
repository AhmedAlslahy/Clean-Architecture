using Application.Common.Abstraction;
using Application.Common.Errors;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Application.Common.Interfaces;

namespace Application.Features.Admin.Roles;

public static class UpdateRole
{
    public sealed record Command(int Id, string RoleName) : IRequest<Result>;

    public sealed class Handler(IApplicationDbContext context, ICacheService cache)
        : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RoleName))
            {
                return RoleErrors.InvalidData;
            }

            var role = await context.Roles
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (role is null)
            {
                return RoleErrors.NotFound;
            }

            var nameExists = await context.Roles
                .AnyAsync(r => r.Name == request.RoleName && r.Id != request.Id, cancellationToken);

            if (nameExists)
            {
                return RoleErrors.AlreadyExists;
            }

            role.Name = request.RoleName;
            await context.SaveChangesAsync(cancellationToken);

            await cache.RemoveAsync("roles:all", cancellationToken);

            return Result.Success();
        }
    }
}