using Domain.Common;
using Domain.Errors;
using Domain.Identity;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.Command.AddAdminRole;

public sealed class AddAdminRoleHandler(SarhneDbContext context)
    : IRequestHandler<AddAdminRoleCommand, Result>
{
    public async Task<Result> Handle(AddAdminRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users.Include(u => u.Roles).FindByIdAsync(request.UserId);
        if (user == null)
        {
            return UserErrors.NotFound;
        }
        bool isAdmin = user.Roles.Any(ur => ur.RoleName == "Admin");
        if (isAdmin)
        {
            return RoleErrors.AlreadyExists;
        }

        await context.AddRoleToUserAsync(user.Id, "Admin");
        return Result.Success();
    }
}