using Application.Features.Authentication.Register;
using MediatR;
using Presentation.Common;

namespace Presentation.EndPoints.Auth;

public static class Register
{
    public static IEndpointRouteBuilder MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register",
            async (RegisterCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                return result.ToHttpResult();
            }).WithTags("Auth");

        return app;
    }
}