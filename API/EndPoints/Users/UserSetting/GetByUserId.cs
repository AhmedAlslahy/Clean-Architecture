using API.Common;
using MediatR;

namespace API.EndPoints.Users.UserSetting;

public static class GetByUserId
{
    public static IEndpointRouteBuilder MapGetUserSettingByUserIdEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/user/usersettings",
            async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Application.Features.User.UsersSetting.GetByUserId.Query(), cancellationToken);

                return result.ToHttpResult();
            })
            .RequireAuthorization().WithTags("User");

        return app;
    }
}