using API.Common;
using Domain.Constants;
using MediatR;

namespace API.EndPoints.Admin.Role;

public static class Create
{
    public static IEndpointRouteBuilder MapAddRoleEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/admin/roles",
            async (Application.Features.Admin.Roles.CreateRole.Command command
            , ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                return result.ToHttpResult();
            }).RequireAuthorization("AdminOnly")
              .RequireAuthorization(Permissions.Roles.Create).WithTags("Admin");

        return app;
    }
}