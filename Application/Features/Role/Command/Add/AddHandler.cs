using Domain.Common;
using Domain.Errors;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace Application.Features.Role.Command.Add;

public sealed class AddHandler(SarhneDbContext context)
    : IRequestHandler<AddCommand, Result>
{
    public async Task<Result> Handle(AddCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RoleName))
        {
            return RoleErrors.InvalidData;
        }

        var exists = await context.Roles.FirstOrDefaultAsync(r => r.RoleName == request.RoleName);
        if (exists != null)
        {
            return RoleErrors.AlreadyExists;
        }
        var role = new Domain.Identity.Role
        {
            RoleName = request.RoleName,
            NormalizedRoleName = request.RoleName.ToUpperInvariant(),
        };

        var result = await context.Roles.AddAsync(role);
        await context.SaveChangesAsync();
        return Result.Success();
    }
}