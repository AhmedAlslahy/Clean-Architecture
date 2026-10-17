using API.Common;
using MediatR;

namespace API.EndPoints.Auth;

public static class Login
{
    public static IEndpointRouteBuilder MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login",
        async (Application.Features.Auth.Login.Command command,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.BadRequest(result);
        }

        httpContext.Response.Cookies.Append("jwt", result.Data!.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(1),
            Path = "/"
        });

        return result.ToHttpResult();
    }).WithTags("Auth");

        return app;
    }
}