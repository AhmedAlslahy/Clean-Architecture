using Application.Features.UserSetting.Query.GetByUserId;
using MediatR;
using Presentation.Common;

namespace Presentation.EndPoints.UserSetting;

public static class GetByUserId
{
    public static IEndpointRouteBuilder MapGetByUserIdEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/usersettings",
            async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetByUserIdQuery(), cancellationToken);

                return result.ToHttpResult();
            })
            .RequireAuthorization().WithTags("UserSetting");

        return app;
    }
}