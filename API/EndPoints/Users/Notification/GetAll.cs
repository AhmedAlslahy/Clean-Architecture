using Application.Features.User.Notifications;
using Domain.Constants;
using MediatR;

namespace API.EndPoints.Users.Notification;

public static class GetAll
{
    public static IEndpointRouteBuilder MapGetAllNotifications(this IEndpointRouteBuilder app)
    {
        app.MapGet("/user/notifications", async (
            [AsParameters] GetAllNotifications.Query query,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(query, cancellationToken);

            return Results.Ok(result);
        }).RequireAuthorization("UserOnly")
        .RequireAuthorization(Permissions.Notifications.Read)
        .WithTags("User");

        return app;
    }
}