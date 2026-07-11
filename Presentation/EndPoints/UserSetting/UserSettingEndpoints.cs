namespace Presentation.EndPoints.UserSetting;

public static class UserSettingEndpoints
{
    public static IEndpointRouteBuilder MapUserSettingEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetByUserIdEndpoint();
        app.MapUpdateEndpoint();

        return app;
    }
}