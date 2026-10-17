using Application.Features.User.Messages;
using MediatR;

namespace API.EndPoints.Users.Message;

public static class GetAll
{
    public static IEndpointRouteBuilder MapGetAllMessages(
       this IEndpointRouteBuilder app)
    {
        app.MapGet("/user/messages/get-all", async (
            [AsParameters] GetAllMessages.Query query,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(query, cancellationToken);

            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithTags("User");

        return app;
    }
}