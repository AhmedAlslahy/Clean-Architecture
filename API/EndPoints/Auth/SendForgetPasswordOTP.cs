using API.Common;
using MediatR;

namespace API.EndPoints.Auth;

public static class SendForgetPasswordOTP
{
    public static IEndpointRouteBuilder MapSendForgetPasswordOTPEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/send-forget-password-otp",
            async (Application.Features.Auth.SendForgetPasswordOTP.Command command, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                return result.ToHttpResult();
            }).WithTags("Auth");

        return app;
    }
}