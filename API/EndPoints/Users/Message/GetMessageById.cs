using Application.Features.User.Messages;
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
        })
        .RequireAuthorization()
        .WithTags("User");

        return app;
    }
}