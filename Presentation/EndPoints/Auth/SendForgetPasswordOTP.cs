using Application.Features.Authentication.SendForgetPasswordOTP;
using MediatR;
using Presentation.Common;

namespace Presentation.EndPoints.Auth;

public static class SendForgetPasswordOTP
{
    public static IEndpointRouteBuilder MapSendForgetPasswordOTPEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/send-forget-password-otp",
            async (SendForgetPasswordOTPCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                return result.ToHttpResult();
            }).WithTags("Auth");

        return app;
    }
}