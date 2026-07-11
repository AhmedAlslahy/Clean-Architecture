using Application.Features.User.Command.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common;

namespace Presentation.EndPoints.User
{
    public static class Update
    {
        public static IEndpointRouteBuilder MapUpdateEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPut("/users",
                async ([FromForm] UpdateUserCommand command, ISender sender, CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(command, cancellationToken);

                    return result.ToHttpResult();
                }).DisableAntiforgery().WithTags("Users");

            return app;
        }
    }
}