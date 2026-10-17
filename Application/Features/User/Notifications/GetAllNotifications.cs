using Application.Common.Abstraction;
using Application.Common.Interfaces;
using Application.Common.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.Notifications;

public static class GetAllNotifications
{
    public sealed class Query : PaginationRequest,
        IRequest<Result<PaginatedResult<NotificationDetails>>>;

    public sealed class NotificationDetails
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }

    public sealed class Handler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
        : IRequestHandler<Query, Result<PaginatedResult<NotificationDetails>>>
    {
        public async Task<Result<PaginatedResult<NotificationDetails>>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var result = await context.Notifications
                .AsNoTracking()
                .Where(n => n.ReceiverId == currentUser.UserId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(item => new NotificationDetails
                {
                    Id = item.Id,
                    IsRead = item.IsRead,
                    Body = item.Body,
                    CreatedAt = item.CreatedAt,
                    Title = item.Title,
                })
                .ToPaginatedListAsync(
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);

            return result;
        }
    }
}