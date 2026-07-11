using Application.Features.Authentication.SendConfirmEmailOTP;
using MediatR;
using Presentation.Common;

namespace Presentation.EndPoints.Auth;

public static class SendConfirmEmailOTP
{
    public static IEndpointRouteBuilder MapSendConfirmEmailOTPEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/send-confirm-email-otp",
            async (SendConfirmEmailOTPCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                return result.ToHttpResult();
            }).RequireAuthorization().WithTags("Auth");

        return app;
    }
}