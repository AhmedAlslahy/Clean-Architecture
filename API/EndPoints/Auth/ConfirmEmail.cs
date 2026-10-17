using API.Common;
using MediatR;

namespace API.EndPoints.Auth;

public static class ConfirmEmail
{
    public static IEndpointRouteBuilder MapConfirmEmailEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/confirm-email",
            async (Application.Features.Auth.ConfirmEmail.Command command, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                return result.ToHttpResult();
            }).RequireAuthorization().WithTags("Auth");

        return app;
    }
}