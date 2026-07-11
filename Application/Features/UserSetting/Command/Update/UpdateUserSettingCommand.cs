using Domain.Common;
using MediatR;

namespace Application.Features.UserSetting.Command.Update;

public sealed record UpdateUserSettingCommand(
bool AllowAnonymousMessages,
bool ShowLastSeen,
bool ShowProfileViews
) : IRequest<Result>;