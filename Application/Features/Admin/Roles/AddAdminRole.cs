using Application.Common.Abstraction;
using MediatR;
using Application.Common.Errors;
using Application.Common.Extensions;

namespace Application.Features.Admin.Roles;

public static class AddAdminRole
{
    public sealed record Command(int UserId) : IRequest<Result>;

    public sealed class Handler(IApplicationDbContext context) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command req, CancellationToken cancellationToken = default)
        {
            var user = await context.Users.FindByIdAsync(req.UserId, cancellationToken);
            if (user == null)
            {
                return UserErrors.NotFound;
            }
            bool isAdmin = user.UserRoles.Any(ur => ur.Role.Name == "Admin");
            if (isAdmin)
            {
                return RoleErrors.AlreadyExists;
            }

            await context.AddRoleToUserAsync(user.Id, "Admin");
            return Result.Success();
        }
    }
}