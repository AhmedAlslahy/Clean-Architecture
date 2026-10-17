using Application.Common.Abstraction;
using MediatR;
using Application.Common.Interfaces;
using Application.Common.Errors;

namespace Application.Features.Admin.Roles;

public static class AddAdminRole
{
    public sealed record Command(int UserId) : IRequest<Result>;

    public sealed class Handler(IUserService users) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command req, CancellationToken cancellationToken = default)
        {
            var user = await users.FindByIdAsync(req.UserId);
            if (user == null)
            {
                return UserErrors.NotFound;
            }
            bool isAdmin = user.UserRoles.Any(ur => ur.Role.Name == "Admin");
            if (isAdmin)
            {
                return RoleErrors.AlreadyExists;
            }

            await users.AddRoleToUserAsync(user.Id, "Admin");
            return Result.Success();
        }
    }
}