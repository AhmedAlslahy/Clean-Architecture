namespace Presentation.EndPoints.User;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetAllEndpoint();
        app.MapAddAdminRoleEndpoint();
        app.MapDeleteEndpoint();
        app.MapGetByLinkEndpoint();
        app.MapUpdateEndpoint();

        return app;
    }
}