namespace Domain.Constants;

public static class Permissions
{
    public static class Roles
    {
        public const string Read = "permissions:roles:read";
        public const string Create = "permissions:roles:create";
        public const string Update = "permissions:roles:update";
        public const string Delete = "permissions:roles:delete";
        public const string AssignPermissions = "permissions:roles:assign-permissions";
    }

    public static class PermissionManagement
    {
        public const string Read = "permissions:permissions:read";
        public const string Create = "permissions:permissions:create";
        public const string Update = "permissions:permissions:update";
        public const string Delete = "permissions:permissions:delete";
    }

    public static class UserSetting
    {
        public const string Read = "permissions:userSetting:read";
        public const string Update = "permissions:userSetting:update";
    }

    public static class Users
    {
        public const string Read = "permissions:users:read";
        public const string Create = "permissions:users:create";
        public const string Update = "permissions:users:update";
        public const string Delete = "permissions:users:delete";
    }

    public static class Messages
    {
        public const string Read = "permissions:messages:read";
        public const string Create = "permissions:messages:Create";
        public const string Update = "permissions:messages:Update";
        public const string Delete = "permissions:messages:delete";
    }

    public static class Notifications
    {
        public const string Read = "permissions:notifications:read";
        public const string Create = "permissions:notifications:create";
        public const string Delete = "permissions:notifications:delete";
    }

    public static readonly IReadOnlyCollection<string> GetAll =
    [
        Roles.Read,
        Roles.Create,
        Roles.Update,
        Roles.Delete,
        Roles.AssignPermissions,

        PermissionManagement.Read,
        PermissionManagement.Create,
        PermissionManagement.Update,
        PermissionManagement.Delete,

        Users.Read,
        Users.Create,
        Users.Update,
        Users.Delete,

        UserSetting.Read,
        UserSetting.Update,

        Messages.Read,
        Messages.Delete,

        Notifications.Read,
        Notifications.Create,
        Notifications.Delete,
    ];
}