using Domain.Common;
using Domain.Errors;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.Query.GetByLink;

public sealed class GetByLinkHandler(SarhneDbContext context) : IRequestHandler<GetByLinkQuery, Result<GetByLinkResponse>>
{
    public async Task<Result<GetByLinkResponse>> Handle(GetByLinkQuery request, CancellationToken cancellation)
    {
        var user = await context.Users.Where(u => u.PublicLink == request.Link)
            .Select(item => new GetByLinkResponse
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
            }).FirstOrDefaultAsync();
        if (user == null)
        {
            return UserErrors.NotFound;
        }

        user.ProfileViewsCount++;
        await context.SaveChangesAsync(cancellation);
        return Result<GetByLinkResponse>.Success(user);
    }
}