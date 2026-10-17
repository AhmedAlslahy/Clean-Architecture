using Application.Common.Abstraction;
using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Features.User.Messages;

public static class SendMessage
{
    public sealed record Command(
    string? Content,
    Stream? PhotoStream,
    string? PhotoFileName,
    int ReceiverId
) : IRequest<Result>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ReceiverId)
                .GreaterThan(0)
                .WithMessage("ReceiverId must be greater than zero.");

            RuleFor(x => x)
                .Must(x =>
                    !string.IsNullOrWhiteSpace(x.Content) ||
                    x.PhotoStream != null)
                .WithMessage("Message must contain content or a photo.");

            RuleFor(x => x.Content)
                .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.Content));

            RuleFor(x => x.PhotoFileName)
                .Must(fileName =>
                    fileName == null ||
                    new[] { ".jpg", ".jpeg", ".png", ".webp" }
                        .Contains(
                            Path.GetExtension(fileName),
                            StringComparer.OrdinalIgnoreCase))
                .WithMessage("Only jpg, jpeg, png and webp images are allowed.");
        }
    }

    public sealed class Handler(
    IApplicationDbContext context,
    INotificationHub notificationHub,
    IMessageHub messageHub,
    ICurrentUserService currentUser,
    IFileStorageService fileStorage)
    : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(
            Command req,
            CancellationToken cancellationToken)
        {
            string? photoUrl = null;

            if (req.PhotoStream is not null &&
                !string.IsNullOrWhiteSpace(req.PhotoFileName))
            {
                var storedFileName = await fileStorage.UploadAsync(
                    req.PhotoStream,
                    req.PhotoFileName,
                    "Photos",
                    cancellationToken);

                photoUrl = $"Photos/{storedFileName}";
            }

            var message = new Message(
                currentUser.UserId,
                req.ReceiverId,
                req.Content,
                photoUrl);

            var body = !string.IsNullOrWhiteSpace(req.Content)
                ? req.Content
                : "Sent an image";

            var notification = new Notification(
                "New Message",
                body,
                currentUser.UserId,
                req.ReceiverId);

            context.Messages.Add(message);
            context.Notifications.Add(notification);

            await context.SaveChangesAsync(cancellationToken);

            var unreadMessages = await context.Messages
                .CountAsync(
                    m => m.ReceiverId == req.ReceiverId && !m.IsRead,
                    cancellationToken);

            var unreadNotifications = await context.Notifications
                .CountAsync(
                    n => n.ReceiverId == req.ReceiverId && !n.IsRead,
                    cancellationToken);

            await messageHub.SendUnreadCountAsync(req.ReceiverId, unreadMessages, cancellationToken);

            await notificationHub.SendUnreadCountAsync(req.ReceiverId, unreadNotifications, cancellationToken);

            return Result.Success();
        }
    }
}