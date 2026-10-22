using Application.Features.User.Notifications;
using Domain.Constants;
using MediatR;

namespace API.EndPoints.Users.Notification;

public static class GetUnread
{
    public static IEndpointRouteBuilder MapGetUnreadNotificationsCount(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/user/notifications/unread-count", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new UnreadCountNotifications.Query(), cancellationToken);

            return Results.Ok(result);
        }).RequireAuthorization("UserOnly")
        .RequireAuthorization(Permissions.Notifications.Read)
        .WithTags("User");

        return app;
    }
}