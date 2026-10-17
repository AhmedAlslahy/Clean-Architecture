using Application.Common.Abstraction;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using MediatR;
using Application.Common.Errors;

namespace Application.Features.Admin.Roles;

public static class DeleteRole
{
    public sealed record Command(int RoleId) : IRequest<Result>;

    public sealed class Handler(IApplicationDbContext context) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command req, CancellationToken cancellationToken = default)
        {
            var rows = await context.Roles
           .Where(r => r.Id == req.RoleId)
           .ExecuteDeleteAsync(cancellationToken);

            if (rows == 0)
            {
                return RoleErrors.NotFound;
            }

            return Result.Success();
        }
    }
}