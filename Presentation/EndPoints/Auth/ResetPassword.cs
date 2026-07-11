using Application.Features.Authentication.ResetPassword;
using MediatR;
using Presentation.Common;

namespace Presentation.EndPoints.Auth;

public static class ResetPassword
{
    public static IEndpointRouteBuilder MapResetPasswordEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/reset-password",
            async (ResetPasswordCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                return result.ToHttpResult();
            }).RequireAuthorization().WithTags("Auth");

        return app;
    }
}