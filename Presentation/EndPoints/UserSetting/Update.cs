using Application.Features.UserSetting.Command.Update;
using MediatR;
using Presentation.Common;

namespace Presentation.EndPoints.UserSetting;

public static class Update
{
    public static IEndpointRouteBuilder MapUpdateEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/usersetting",
            async (UpdateUserSettingCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                return result.ToHttpResult();
            }).RequireAuthorization().WithTags("UserSetting");

        return app;
    }
}