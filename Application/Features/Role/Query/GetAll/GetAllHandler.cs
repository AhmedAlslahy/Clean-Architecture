using Domain.Common;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace Application.Features.Role.Query.GetAll;

public sealed class GetAllHandler(SarhneDbContext context) : IRequestHandler<GetAllQuery, Result<List<GetAllResponse>>>
{
    public async Task<Result<List<GetAllResponse>>> Handle(GetAllQuery request, CancellationToken cancellation)
    {
        var roles = await context.Roles.Select(item => new GetAllResponse
        {
            Id = item.Id,
            RoleName = item.RoleName
        }).ToListAsync(cancellation);
        return roles;
    }
}