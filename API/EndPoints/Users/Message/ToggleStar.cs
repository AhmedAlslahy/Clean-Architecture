using Domain.Constants;
using MediatR;

namespace API.EndPoints.Users.Message;

public static class ToggleStar
{
    public static IEndpointRouteBuilder MapToggleStar(
       this IEndpointRouteBuilder app)
    {
        app.MapPatch("/user/messages/star/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new Application.Features.User.Messages.ToggleStar.Command(id), cancellationToken);

            return Results.Ok(result);
        }).RequireAuthorization("UserOnly")
        .RequireAuthorization(Permissions.Messages.Update)
        .WithTags("User");

        return app;
    }
}