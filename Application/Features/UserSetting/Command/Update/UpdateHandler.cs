using Domain.Common;
using Domain.Errors;
using Infrastructure.Persistence;
using Infrastructure.Service.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UserSetting.Command.Update;

public class UpdateHandler(SarhneDbContext context, ICurrentUser currentUser) : IRequestHandler<UpdateUserSettingCommand, Result>
{
    public async Task<Result> Handle(UpdateUserSettingCommand request, CancellationToken cancellation)
    {
        if (request == null)
        {
            return UserErrors.InvalidSettingData;
        }

        var result = await context.UserSettings.FirstOrDefaultAsync(us => us.UserId == currentUser.UserId, cancellation);
        if (result == null)
        {
            return UserErrors.NotFound;
        }

        result.SetAnonymousMessages(request.AllowAnonymousMessages);
        result.SetLastSeen(request.ShowLastSeen);
        result.SetProfileViews(request.ShowProfileViews);
        await context.SaveChangesAsync(cancellation);
        return Result.Success();
    }
}