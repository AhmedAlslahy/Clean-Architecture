using MediatR;

namespace API.EndPoints.Users.Message
{
    public static class GetAllSender
    {
        public static IEndpointRouteBuilder MapGetAllSenderMessages(
        this IEndpointRouteBuilder app)
        {
            app.MapGet("/user/messages/get-all-sender", async (
                [AsParameters] Application.Features.User.Messages.GetAllSender.Query query,
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
}