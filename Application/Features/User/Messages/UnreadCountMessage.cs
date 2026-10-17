using Application.Common.Abstraction;
using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace Application.Features.User.Messages;

public static class UnreadCountMessage
{
    public sealed record Query : IRequest<Result<int>>;

    public sealed class Handler(IApplicationDbContext context, ICurrentUserService currentUser)
        : IRequestHandler<Query, Result<int>>
    {
        public async Task<Result<int>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var count = await context.Messages
                .CountAsync(
                    m => m.ReceiverId == currentUser.UserId && !m.IsRead,
                    cancellationToken);

            return count;
        }
    }
}