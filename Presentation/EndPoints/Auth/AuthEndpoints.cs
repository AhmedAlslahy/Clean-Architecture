namespace Presentation.EndPoints.Auth;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapRegisterEndpoint();
        app.MapLoginEndpoint();
        app.MapLogoutEndpoint();
        app.MapSendConfirmEmailOTPEndpoint();
        app.MapConfirmEmailEndpoint();
        app.MapSendForgetPasswordOTPEndpoint();
        app.MapForgetPasswordEndpoint();
        app.MapResetPasswordEndpoint();

        return app;
    }
}