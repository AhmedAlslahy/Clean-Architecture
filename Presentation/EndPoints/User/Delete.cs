using Application.Features.User.Command.AddAdminRole;
using Application.Features.User.Command.Delete;
using Domain.Identity;
using MediatR;
using Presentation.Common;

namespace Presentation.EndPoints.User
{
    public static class Delete
    {
        public static IEndpointRouteBuilder MapDeleteEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapDelete("/user/{userId}",
                async (string userId, ISender sender, CancellationToken cancellationToken) =>
                {
                    var command = new DeleteCommand(userId);
                    var result = await sender.Send(command, cancellationToken);

                    return result.ToHttpResult();
                }).RequireAuthorization("AdminOnly").WithTags("Admin");

            return app;
        }
    }
}