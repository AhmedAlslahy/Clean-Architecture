using Domain.Common;
using Domain.Errors;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Role.Command.Delete;

public class DeleteHandler(SarhneDbContext context) : IRequestHandler<DeleteCommand, Result>
{
    public async Task<Result> Handle(DeleteCommand request, CancellationToken cancellation)
    {
        var rows = await context.Roles
        .Where(r => r.Id == request.RoleId)
        .ExecuteDeleteAsync();

        if (rows == 0)
        {
            return RoleErrors.NotFound;
        }

        return Result.Success();
    }
}