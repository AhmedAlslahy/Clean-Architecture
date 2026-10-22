using Application.Features.User.Messages;
using Domain.Constants;
using MediatR;

namespace API.EndPoints.Users.Message;

public static class GetMessageById
{
    public static IEndpointRouteBuilder MapGetMessageById(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/user/messages/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetMessage.Query(id), cancellationToken);

            return Results.Ok(result);
        }).RequireAuthorization("UserOnly")
        .RequireAuthorization(Permissions.Messages.Read)
        .WithTags("User");

        return app;
    }
}