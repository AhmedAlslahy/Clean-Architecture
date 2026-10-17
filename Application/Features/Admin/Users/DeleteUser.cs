using Microsoft.EntityFrameworkCore;
using MediatR;
using Application.Common.Abstraction;
using Application.Common.Errors;

namespace Application.Features.Admin.Users;

public static class DeleteUser
{
    public sealed record Command(int UserId) : IRequest<Result>;

    public sealed class Handler(IApplicationDbContext context)
        : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await context.Users.FindAsync([request.UserId], cancellationToken);

            if (user == null)
            {
                return UserErrors.NotFound;
            }

            context.Users.Remove(user);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}