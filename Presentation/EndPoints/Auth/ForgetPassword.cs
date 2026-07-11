using Application.Features.Authentication.ForgetPassword;
using MediatR;
using Presentation.Common;

namespace Presentation.EndPoints.Auth;

public static class ForgetPassword
{
    public static IEndpointRouteBuilder MapForgetPasswordEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/forget-password",
            async (ForgetPasswordCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                return result.ToHttpResult();
            }).WithTags("Auth");

        return app;
    }
}