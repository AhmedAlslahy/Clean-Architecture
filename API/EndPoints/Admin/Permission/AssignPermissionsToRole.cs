using Domain.Constants;
using MediatR;

namespace API.EndPoints.Admin.Permission;

public static class AssignPermissionsToRole
{
    public static IEndpointRouteBuilder MapAssignPermissionsToRoleEndpoint(
 this IEndpointRouteBuilder app)
    {
        app.MapPut("/admin/permissions/{roleId:int}", async (
            int roleId,
            Application.Features.Admin.Permissions.AssignPermissionsToRole.Command req,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new Application.Features.Admin.Permissions
               .AssignPermissionsToRole.Command(roleId, req.PermissionIds);

            var result = await sender.Send(command, cancellationToken);

            return Results.Ok(result);
        })
        .WithTags("Admin")
        .RequireAuthorization("AdminOnly")
        .RequireAuthorization(Permissions.PermissionManagement.Update);

        return app;
    }
}