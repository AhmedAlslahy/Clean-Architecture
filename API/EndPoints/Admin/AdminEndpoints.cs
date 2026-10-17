using API.EndPoints.Admin.Notification;
using API.EndPoints.Admin.Role;
using API.EndPoints.Admin.User;

namespace API.EndPoints.Admin;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        //role
        app.MapAddRoleEndpoint();
        app.MapDeleteRoleEndpoint();
        app.MapGetAllRolesEndpoint();
        app.MapAddAdminRoleToUserEndpoint();

        //user
        app.MapGetAllUsersEndpoint();
        app.MapDeleteUserEndpoint();

        //notification
        app.MapSendNotificationEndpoint();

        return app;
    }
}