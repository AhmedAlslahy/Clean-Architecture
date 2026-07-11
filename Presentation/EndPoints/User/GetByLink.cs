using Application.Features.User.Query.GetByLink;
using Domain.Identity;
using MediatR;
using Presentation.Common;

namespace Presentation.EndPoints.User;

public static class GetByLink
{
    public static IEndpointRouteBuilder MapGetByLinkEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/user/{link}",
            async (string link, ISender sender, CancellationToken cancellationToken) =>
            {
                var query = new GetByLinkQuery(link);
                var result = await sender.Send(query, cancellationToken);

                return result.ToHttpResult();
            }).WithTags("Users");

        return app;
    }
}