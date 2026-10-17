using API.Common;
using API.Models;
using Application.Features.User.Messages;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.EndPoints.Users.Message;

public static class Send
{
    public static IEndpointRouteBuilder MapSendMessageEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/user/messages",
            async (
                [FromForm] SendMessageRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                // Validate receiver
                if (request.ReceiverId <= 0)
                {
                    return Results.BadRequest(
                        "ReceiverId must be greater than zero.");
                }

                // Message must contain text or photo
                if (string.IsNullOrWhiteSpace(request.Content) &&
                    request.Photo is null)
                {
                    return Results.BadRequest(
                        "Message must contain content or a photo.");
                }

                // Validate content length
                if (request.Content?.Length > 200)
                {
                    return Results.BadRequest(
                        "Message content must not exceed 200 characters.");
                }

                // Validate photo size
                if (request.Photo is { Length: > 5 * 1024 * 1024 })
                {
                    return Results.BadRequest(
                        "Photo size must not exceed 5 MB.");
                }

                if (request.Photo is { Length: 0 })
                {
                    return Results.BadRequest("Photo is empty.");
                }

                // Validate photo extension
                if (request.Photo is not null)
                {
                    var extension = Path.GetExtension(
                        request.Photo.FileName);

                    var allowedExtensions = new[]
                    {
                        ".jpg", ".jpeg", ".png", ".webp"
                    };

                    if (!allowedExtensions.Contains(
                        extension,
                        StringComparer.OrdinalIgnoreCase))
                    {
                        return Results.BadRequest(
                            "Only jpg, jpeg, png and webp images are allowed.");
                    }
                }

                // Send text-only message
                if (request.Photo is null)
                {
                    var command = new SendMessage.Command(
                        request.Content,
                        null,
                        null,
                        request.ReceiverId);

                    var result = await sender.Send(
                        command,
                        cancellationToken);

                    return result.ToHttpResult();
                }

                // Send message with photo
                await using var stream =
                    request.Photo.OpenReadStream();

                var photoCommand = new SendMessage.Command(
                    request.Content,
                    stream,
                    request.Photo.FileName,
                    request.ReceiverId);

                var photoResult = await sender.Send(
                    photoCommand,
                    cancellationToken);

                return photoResult.ToHttpResult();
            })
            .DisableAntiforgery()
            .WithTags("User");

        return app;
    }
}