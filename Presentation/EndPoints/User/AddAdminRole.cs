using Application.Features.User.Command.AddAdminRole;
using Domain.Identity;
using MediatR;
using Presentation.Common;

namespace Presentation.EndPoints.User;

public static class AddAdminRole
{
    public static IEndpointRouteBuilder MapAddAdminRoleEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/users/add-admin-role/{userId}",
            async (string userId, ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new AddAdminRoleCommand(userId);
                var result = await sender.Send(command, cancellationToken);

                return result.ToHttpResult();
            }).RequireAuthorization("AdminOnly").WithTags("Admin");

        return app;
    }
}