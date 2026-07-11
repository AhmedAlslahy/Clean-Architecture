using Application.Features.Role.Query.GetAll;
using MediatR;
using Presentation.Common;

namespace Presentation.EndPoints.Role;

public static class GetAll
{
    public static IEndpointRouteBuilder MapGetAllRolesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/roles",
            async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAllQuery(), cancellationToken);

                return result.ToHttpResult();
            }).RequireAuthorization("AdminOnly").WithTags("Admin");

        return app;
    }
}