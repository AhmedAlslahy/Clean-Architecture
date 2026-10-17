using Application.Common.Abstraction;
using Application.Common.Errors;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.UsersSetting;

public static class GetByUserId
{
    public sealed record Query : IRequest<Result<UserSettingRes>>;

    public sealed class UserSettingRes
    {
        public bool AllowAnonymousMessages { get; set; }
        public bool ShowLastSeen { get; set; }
        public bool ShowProfileViews { get; set; }
    }

    public sealed class Handler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
        : IRequestHandler<Query, Result<UserSettingRes>>
    {
        public async Task<Result<UserSettingRes>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var result = await context.UserSettings.FirstOrDefaultAsync(
                us => us.UserId == currentUser.UserId,
                cancellationToken);

            if (result == null)
            {
                return UserErrors.NotFound;
            }

            var data = new UserSettingRes
            {
                AllowAnonymousMessages = result.AllowAnonymousMessages,
                ShowLastSeen = result.ShowLastSeen,
                ShowProfileViews = result.ShowProfileViews,
            };

            return Result<UserSettingRes>.Success(data);
        }
    }
}