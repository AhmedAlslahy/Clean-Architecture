using Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs.Services;

public class SignalRMessagePublisher(IHubContext<MessageHub> hubContext) : IMessageHub
{
    public Task SendUnreadCountAsync(
       int userId,
       int unreadCount,
       CancellationToken cancellationToken)
    {
        return hubContext.Clients
            .User(userId.ToString())
            .SendAsync(
                "UnreadMessageCount",
                unreadCount,
                cancellationToken);
    }
}