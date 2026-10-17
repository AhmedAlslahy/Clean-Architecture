using Application.Common.Abstraction;
using Application.Common.Interfaces;
using Application.Common.Pagination;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Features.User.Messages;

public class GetAllSender
{
    public sealed class Query : PaginationRequest,
      IRequest<Result<PaginatedResult<MessageRes>>>;

    public sealed record MessageRes
    {
        public int Id { get; init; }
        public string? Content { get; init; }
        public string? PhotoUrl { get; init; }
        public DateTime CreatedAt { get; init; }
        public bool IsRead { get; init; }
        public bool IsStarred { get; init; }
    }

    public static readonly Expression<Func<Message, MessageRes>> Selector =
        m => new MessageRes
        {
            Id = m.Id,
            Content = m.Content,
            PhotoUrl = m.PhotoUrl,
            CreatedAt = m.CreatedAt,
            IsRead = m.IsRead,
            IsStarred = m.IsStarred
        };

    public sealed class Handler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
        : IRequestHandler<Query, Result<PaginatedResult<MessageRes>>>
    {
        public async Task<Result<PaginatedResult<MessageRes>>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            return await context.Messages
                .AsNoTracking()
                .Where(m => m.SenderId == currentUser.UserId)
                .OrderByDescending(m => m.Id)
                .Select(Selector)
                .ToPaginatedListAsync(
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);
        }
    }
}