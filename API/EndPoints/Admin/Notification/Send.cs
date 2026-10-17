using API.Common;
using MediatR;

namespace API.EndPoints.Admin.Notification;

public static class Send
{
    public static IEndpointRouteBuilder MapSendNotificationEndpoint(
       this IEndpointRouteBuilder app)
    {
        app.MapPost("/admin/notifications",
            async (Application.Features.Admin.Notifications.SendNotification.Command command,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                return result.ToHttpResult();
            })
            .RequireAuthorization("AdminOnly").WithTags("Admin");

        return app;
    }
}