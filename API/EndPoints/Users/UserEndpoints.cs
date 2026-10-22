using API.EndPoints.Users.Message;
using API.EndPoints.Users.Notification;
using API.EndPoints.Users.Permission;
using API.EndPoints.Users.User;
using API.EndPoints.Users.UserSetting;

namespace API.EndPoints.Users;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        //permission
        app.MapGetMyPermissionsEndpoint();

        //user setting
        app.MapGetUserSettingByUserIdEndpoint();
        app.MapUpdateUserSettingEndpoint();

        //user
        app.MapUpdateUserEndpoint();
        app.MapGetUserByLinkEndpoint();

        //message
        app.MapSendMessageEndpoint();
        app.MapGetUnreadMessagesCount();
        app.MapGetAllMessages();
        app.MapGetAllSenderMessages();
        app.MapToggleStar();
        app.MapGetMessageById();

        //notification
        app.MapGetAllNotifications();
        app.MapGetUnreadNotificationsCount();
        app.MapGetNotificationById();

        return app;
    }
}