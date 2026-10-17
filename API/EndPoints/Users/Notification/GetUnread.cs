using Application.Features.User.Notifications;
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
        })
        .RequireAuthorization()
        .WithTags("User");

        return app;
    }
}