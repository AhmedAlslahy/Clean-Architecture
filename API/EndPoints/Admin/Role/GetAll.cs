using API.Common;
using MediatR;

namespace API.EndPoints.Admin.Role;

public static class GetAll
{
    public static IEndpointRouteBuilder MapGetAllRolesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/admin/roles",
            async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Application.Features.Admin.Roles.GetAllRoles.Query(), cancellationToken);

                return result.ToHttpResult();
            }).RequireAuthorization("AdminOnly").WithTags("Admin");

        return app;
    }
}