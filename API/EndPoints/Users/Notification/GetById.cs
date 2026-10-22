using Application.Features.User.Notifications;
using Domain.Constants;
using MediatR;

namespace API.EndPoints.Users.Notification;

public static class GetById
{
    public static IEndpointRouteBuilder MapGetNotificationById(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/user/notifications/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetNotificationById.Query(id), cancellationToken);

            return Results.Ok(result);
        }).RequireAuthorization("UserOnly")
        .RequireAuthorization(Permissions.Notifications.Read)
        .WithTags("User");

        return app;
    }
}