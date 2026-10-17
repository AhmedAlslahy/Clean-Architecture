using API.Common;
using MediatR;

namespace API.EndPoints.Auth;

public static class Register
{
    public static IEndpointRouteBuilder MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register",
            async (Application.Features.Auth.Register.Command command, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                return result.ToHttpResult();
            }).WithTags("Auth");

        return app;
    }
}