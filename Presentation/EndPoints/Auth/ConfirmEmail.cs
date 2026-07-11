using Application.Features.Authentication.ConfirmEmail;
using MediatR;
using Presentation.Common;

namespace Presentation.EndPoints.Auth;

public static class ConfirmEmail
{
    public static IEndpointRouteBuilder MapConfirmEmailEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/confirm-email",
            async (ConfirmEmailCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                return result.ToHttpResult();
            }).RequireAuthorization().WithTags("Auth");

        return app;
    }
}