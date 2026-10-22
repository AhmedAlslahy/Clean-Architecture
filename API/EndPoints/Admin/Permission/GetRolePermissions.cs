using Domain.Constants;
using MediatR;

namespace API.EndPoints.Admin.Permission;

public static class GetRolePermissions
{
    public static IEndpointRouteBuilder MapGetRolePermissionsEndpoint(
      this IEndpointRouteBuilder app)
    {
        app.MapGet("/admin/permissions/{roleId:int}", async (
            int roleId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new Application.Features.Admin.Permissions.GetRolePermissions.Query(roleId);
            var result = await sender.Send(command, cancellationToken);

            return Results.Ok(result);
        })
        .WithTags("Admin").RequireAuthorization("AdminOnly")
        .RequireAuthorization(Permissions.PermissionManagement.Read);

        return app;
    }
}