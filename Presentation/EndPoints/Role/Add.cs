using Application.Features.Role.Command.Add;
using MediatR;
using Presentation.Common;

namespace Presentation.EndPoints.Role;

public static class Add
{
    public static IEndpointRouteBuilder MapAddRoleEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/roles",
            async (AddCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                return result.ToHttpResult();
            }).RequireAuthorization("AdminOnly").WithTags("Admin");

        return app;
    }
}