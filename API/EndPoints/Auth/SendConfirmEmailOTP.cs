using API.Common;
using MediatR;

namespace API.EndPoints.Auth;

public static class SendConfirmEmailOTP
{
    public static IEndpointRouteBuilder MapSendConfirmEmailOTPEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/send-confirm-email-otp",
            async (Application.Features.Auth.SendConfirmEmailOTP.Command command, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                return result.ToHttpResult();
            }).RequireAuthorization().WithTags("Auth");

        return app;
    }
}