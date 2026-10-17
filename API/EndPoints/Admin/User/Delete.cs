using API.Common;
using MediatR;

namespace API.EndPoints.Admin.User
{
    public static class Delete
    {
        public static IEndpointRouteBuilder MapDeleteUserEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapDelete("/admin/users/{userId:int}",
                async (int userId, ISender sender, CancellationToken cancellationToken) =>
                {
                    var command = new Application.Features.Admin.Users.DeleteUser.Command(userId);
                    var result = await sender.Send(command, cancellationToken);

                    return result.ToHttpResult();
                }).RequireAuthorization("AdminOnly").WithTags("Admin");

            return app;
        }
    }
}