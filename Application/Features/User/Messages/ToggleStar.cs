using Application.Common.Abstraction;
using Application.Common.Errors;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.Messages;

public static class ToggleStar
{
    public sealed record Command(int MessageId) : IRequest<Result>;

    public sealed class Handler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
        : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(
            Command req,
            CancellationToken cancellationToken)
        {
            var message = await context.Messages
                .FirstOrDefaultAsync(
                    m => m.Id == req.MessageId &&
                         m.ReceiverId == currentUser.UserId,
                    cancellationToken);

            if (message is null)
                return MessageErrors.NotFound;

            message.ToggleStar();

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}