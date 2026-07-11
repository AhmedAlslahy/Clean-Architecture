using Application.Features.User.Query.GetAll;
using MediatR;
using Presentation.Common;

namespace Presentation.EndPoints.User;

public static class GetAll
{
    public static IEndpointRouteBuilder MapGetAllEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/users",
            async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAllQuery(), cancellationToken);

                return result.ToHttpResult();
            }).RequireAuthorization("AdminOnly").WithTags("Admin");

        return app;
    }
}