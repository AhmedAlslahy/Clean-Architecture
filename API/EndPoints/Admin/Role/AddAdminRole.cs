using API.Common;
using Domain.Constants;
using MediatR;

namespace API.EndPoints.Admin.Role;

public static class AddAdminRole
{
    public static IEndpointRouteBuilder MapAddAdminRoleToUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/admin/roles/{userId:int}",
            async (int userId
            , ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new Application.Features.Admin.Roles.AddAdminRole.Command(userId);
                var result = await sender.Send(command, cancellationToken);

                return result.ToHttpResult();
            }).RequireAuthorization("AdminOnly")
            .RequireAuthorization(Permissions.Roles.Create).WithTags("Admin");

        return app;
    }
}