namespace Presentation.EndPoints.Role;

public static class RoleEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAddRoleEndpoint();
        app.MapDeleteRoleEndpoint();
        app.MapGetAllRolesEndpoint();

        return app;
    }
}