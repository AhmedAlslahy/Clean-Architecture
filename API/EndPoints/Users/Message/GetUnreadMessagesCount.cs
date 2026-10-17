using MediatR;

namespace API.EndPoints.Users.Message;

public static class GetUnreadMessagesCount
{
    public static IEndpointRouteBuilder MapGetUnreadMessagesCount(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/user/messages/unread-count", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new Application.Features.User.Messages.UnreadCountMessage.Query(), cancellationToken);

            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithTags("User");

        return app;
    }
}