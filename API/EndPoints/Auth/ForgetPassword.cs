using API.Common;
using MediatR;

namespace API.EndPoints.Auth;

public static class ForgetPassword
{
    public static IEndpointRouteBuilder MapForgetPasswordEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/forget-password",
            async (Application.Features.Auth.ForgetPassword.Command command, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                return result.ToHttpResult();
            }).WithTags("Auth");

        return app;
    }
}