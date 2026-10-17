using Application.Common.Abstraction;
using Application.Common.Errors;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Domain.Entities.Identity;
using Application.Common.Interfaces;

namespace Application.Features.Admin.Roles;

public static class CreateRole
{
    public sealed record Command(string RoleName) : IRequest<Result>;

    public sealed class Handler(IApplicationDbContext context, ICacheService cache)
        : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RoleName))
            {
                return RoleErrors.InvalidData;
            }

            var exists = await context.Roles
                .FirstOrDefaultAsync(r => r.Name == request.RoleName, cancellationToken);

            if (exists is not null)
            {
                return RoleErrors.AlreadyExists;
            }

            await context.Roles.AddAsync(
                new Role
                {
                    Name = request.RoleName
                },
                cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            await cache.RemoveAsync(
                "roles:all",
                cancellationToken);

            return Result.Success();
        }
    }
}