using API.Common;
using Domain.Constants;
using MediatR;

namespace API.EndPoints.Admin.Role;

public static class Delete
{
    public static IEndpointRouteBuilder MapDeleteRoleEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapDelete("/admin/roles/{roleId:int}",
            async (int roleId, ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new Application.Features.Admin.Roles.DeleteRole.Command(roleId);

                var result = await sender.Send(command, cancellationToken);

                return result.ToHttpResult();
            }).RequireAuthorization("AdminOnly")
            .RequireAuthorization(Permissions.Roles.Delete).WithTags("Admin");

        return app;
    }
}