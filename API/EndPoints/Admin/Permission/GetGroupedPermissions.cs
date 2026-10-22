using Domain.Constants;
using MediatR;

namespace API.EndPoints.Admin.Permission;

public static class GetGroupedPermissions
{
    public static IEndpointRouteBuilder MapGetGroupedPermissionsEndpoint(
  this IEndpointRouteBuilder app)
    {
        app.MapGet("/admin/permissions", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new Application.Features.Admin.Permissions.GetGroupedPermissions.Query(),
                cancellationToken);

            return Results.Ok(result);
        })
        .WithTags("Admin").RequireAuthorization("AdminOnly")
        .RequireAuthorization(Permissions.PermissionManagement.Read);

        return app;
    }
}