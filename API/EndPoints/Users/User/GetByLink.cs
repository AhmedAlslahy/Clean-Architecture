using API.Common;
using MediatR;

namespace API.EndPoints.Users.User;

public static class GetByLink
{
    public static IEndpointRouteBuilder MapGetUserByLinkEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/user/{link}",
            async (string link, ISender sender, CancellationToken cancellationToken) =>
            {
                var query = new Application.Features.User.Users.GetByLink.Query(link);
                var result = await sender.Send(query, cancellationToken);

                return result.ToHttpResult();
            }).WithTags("User");

        return app;
    }
}