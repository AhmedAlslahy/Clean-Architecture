using Application.Features.Role.Command.Delete;
using MediatR;
using Presentation.Common;

namespace Presentation.EndPoints.Role;

public static class Delete
{
    public static IEndpointRouteBuilder MapDeleteRoleEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/roles/{roleId}",
            async (string roleId, ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new DeleteCommand(roleId);

                var result = await sender.Send(command, cancellationToken);

                return result.ToHttpResult();
            })
            .RequireAuthorization("AdminOnly").WithTags("Admin");

        return app;
    }
}