using API.Common;
using API.Models;
using Application.Features.User.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.EndPoints.Users.User;

public static class Update
{
    public static IEndpointRouteBuilder MapUpdateUserEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPut("/users",
            async (
                [FromForm] UpdateUserRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                Stream? imageStream = null;

                if (request.Image is not null)
                {
                    if (request.Image.Length == 0)
                        return Results.BadRequest("Image is empty.");

                    if (request.Image.Length > 5 * 1024 * 1024)
                        return Results.BadRequest(
                            "Image must not exceed 5 MB.");

                    imageStream = request.Image.OpenReadStream();
                }

                if (imageStream is not null)
                {
                    await using (imageStream)
                    {
                        var command = new UpdateUser.Command
                        {
                            FullName = request.FullName,
                            PhoneNumber = request.PhoneNumber,
                            ProfileDescription = request.ProfileDescription,
                            PublicLink = request.PublicLink,
                            ImageStream = imageStream,
                            ImageFileName = request.Image!.FileName
                        };

                        var result = await sender.Send(
                            command, cancellationToken);

                        return result.ToHttpResult();
                    }
                }

                var updateCommand = new UpdateUser.Command
                {
                    FullName = request.FullName,
                    PhoneNumber = request.PhoneNumber,
                    ProfileDescription = request.ProfileDescription,
                    PublicLink = request.PublicLink
                };

                var updateResult = await sender.Send(
                    updateCommand, cancellationToken);

                return updateResult.ToHttpResult();
            })
            .DisableAntiforgery().WithTags("User");

        return app;
    }
}