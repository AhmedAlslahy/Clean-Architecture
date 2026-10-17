using API.Common;
using MediatR;

namespace API.EndPoints.Admin.User;

public static class GetAll
{
    public static IEndpointRouteBuilder MapGetAllUsersEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/admin/users",
            async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Application.Features.Admin.Users.GetAllUsers.Query(), cancellationToken);

                return result.ToHttpResult();
            }).RequireAuthorization("AdminOnly").WithTags("Admin");

        return app;
    }
}