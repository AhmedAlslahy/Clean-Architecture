using API.Common;
using Domain.Constants;
using MediatR;

namespace API.EndPoints.Users.UserSetting;

public static class Update
{
    public static IEndpointRouteBuilder MapUpdateUserSettingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/user/usersetting",
            async (Application.Features.User.UsersSetting.UpdateUserSetting.Command command
            , ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                return result.ToHttpResult();
            }).RequireAuthorization("UserOnly")
              .RequireAuthorization(Permissions.UserSetting.Update).WithTags("User");

        return app;
    }
}