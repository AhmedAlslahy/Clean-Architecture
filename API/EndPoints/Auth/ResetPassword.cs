using API.Common;
using MediatR;

namespace API.EndPoints.Auth;

public static class ResetPassword
{
    public static IEndpointRouteBuilder MapResetPasswordEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/reset-password",
            async (Application.Features.Auth.ResetPassword.Command command, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                return result.ToHttpResult();
            }).RequireAuthorization().WithTags("Auth");

        return app;
    }
}