namespace Application.Common.Interfaces;

public interface IMessageHub
{
    Task SendUnreadCountAsync(
        int userId,
        int unreadCount,
        CancellationToken cancellationToken);
}