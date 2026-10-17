namespace Application.Common.Interfaces;

public interface INotificationHub
{
    Task SendUnreadCountAsync(
        int userId,
        int unreadCount,
        CancellationToken cancellationToken);
}