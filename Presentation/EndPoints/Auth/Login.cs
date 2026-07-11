using Application.Features.Authentication.Login;
using MediatR;
using Presentation.Common;

namespace Presentation.EndPoints.Auth;

public static class Login
{
    public static IEndpointRouteBuilder MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login",
    async (
        LoginCommand command,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return result.ToHttpResult();

        httpContext.Response.Cookies.Append(
            "jwt",
            result.Data.Token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = result.Data.ExpireIn
            });

        return result.ToHttpResult();
    }).WithTags("Auth");

        return app;
    }
}