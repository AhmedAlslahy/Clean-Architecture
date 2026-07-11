namespace Presentation.EndPoints.Auth;

public static class Logout
{
    public static IEndpointRouteBuilder MapLogoutEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/logout",
            (HttpContext httpContext) =>
            {
                httpContext.Response.Cookies.Delete("jwt", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                });

                return Results.Ok(new
                {
                    Message = "Logged out successfully."
                });
            })
            .RequireAuthorization()
            .WithTags("Auth");

        return app;
    }
}