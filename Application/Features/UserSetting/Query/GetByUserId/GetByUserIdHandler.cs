using Domain.Common;
using Domain.Errors;
using Infrastructure.Persistence;
using Infrastructure.Service.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UserSetting.Query.GetByUserId;

public sealed class GetByUserIdHandler(SarhneDbContext context, ICurrentUser currentUser)
    : IRequestHandler<GetByUserIdQuery, Result<GetByUserIdResponse>>
{
    public async Task<Result<GetByUserIdResponse>> Handle(
        GetByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        var result = await context.UserSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(
                us => us.UserId == currentUser.UserId,
                cancellationToken);

        if (result is null)
            return UserErrors.NotFound;

        return Result<GetByUserIdResponse>.Success(new GetByUserIdResponse
        {
            AllowAnonymousMessages = result.AllowAnonymousMessages,
            ShowLastSeen = result.ShowLastSeen,
            ShowProfileViews = result.ShowProfileViews
        });
    }
}