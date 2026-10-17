using Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs.Services;

public sealed class SignalRNotificationPublisher(
    IHubContext<NotificationHub> hubContext)
    : INotificationHub
{
    public Task SendUnreadCountAsync(
        int userId,
        int unreadCount,
        CancellationToken cancellationToken)
    {
        return hubContext.Clients
            .User(userId.ToString())
            .SendAsync(
                "UnreadNotificationCount",
                unreadCount,
                cancellationToken);
    }
}