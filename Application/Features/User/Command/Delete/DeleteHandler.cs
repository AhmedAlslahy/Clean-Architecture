using Domain.Common;
using Domain.Errors;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.Command.Delete;

public class DeleteHandler(SarhneDbContext context) : IRequestHandler<DeleteCommand, Result>
{
    public async Task<Result> Handle(DeleteCommand request, CancellationToken cancellation)
    {
        var user = await context.Users.FindAsync(request.UserId);
        if (user == null)
        {
            return UserErrors.NotFound;
        }

        context.Entry(user).State = EntityState.Deleted;
        await context.SaveChangesAsync(cancellation);
        return Result.Success();
    }
}