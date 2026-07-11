using Domain.Common;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.Query.GetAll;

public sealed class GetAllHandler(SarhneDbContext context) : IRequestHandler<GetAllQuery, Result<List<GetAllResponse>>>
{
    public async Task<Result<List<GetAllResponse>>> Handle(GetAllQuery request, CancellationToken cancellation)
    {
        var users = await context.Users.AsNoTracking().Select(item => new GetAllResponse
        {
            Id = item.Id,
            Email = item.Email,
            FullName = item.FullName,
            PublicLink = item.PublicLink,
            PhoneNumber = item.PhoneNumber,
            ImageUrl = item.ImageUrl,
            Gender = item.Gender,
            ProfileDescription = item.ProfileDescription,
            LastSeen = item.LastSeen,
            ProfileViewsCount = item.ProfileViewsCount,
        }).ToListAsync(cancellation);

        return users;
    }
}