using Domain.Constants;
using MediatR;

namespace API.EndPoints.Users.Permission;

public static class GetMyPermissions
{
    public static IEndpointRouteBuilder MapGetMyPermissionsEndpoint(
       this IEndpointRouteBuilder app)
    {
        app.MapGet("/admin/permissions/me", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new Application.Features.User.Permission.GetMyPermissions.Query(),
                cancellationToken);

            return Results.Ok(result);
        })
        .WithTags("User").RequireAuthorization("UserOnly")
        .RequireAuthorization(Permissions.PermissionManagement.Read);

        return app;
    }
}