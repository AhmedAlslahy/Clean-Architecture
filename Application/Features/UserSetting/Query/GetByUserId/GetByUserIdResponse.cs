namespace Application.Features.UserSetting.Query.GetByUserId;

public sealed class GetByUserIdResponse
{
    public bool AllowAnonymousMessages { get; init; }
    public bool ShowLastSeen { get; init; }
    public bool ShowProfileViews { get; init; }
}