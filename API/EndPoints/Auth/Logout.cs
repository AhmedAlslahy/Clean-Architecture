using API.Common;
using MediatR;

namespace API.EndPoints.Auth;

public static class Logout
{
    public sealed record Request(string RefreshToken);

    public static IEndpointRouteBuilder MapLogoutEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/logout", async (Application.Features.Auth.Logout.Command command,
                ISender sender, HttpContext httpContext,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                httpContext.Response.Cookies.Delete("jwt", new CookieOptions
                {
                    Path = "/"
                });

                return result.ToHttpResult();
            })
            .RequireAuthorization()
            .WithTags("Auth");

        return app;
    }
}