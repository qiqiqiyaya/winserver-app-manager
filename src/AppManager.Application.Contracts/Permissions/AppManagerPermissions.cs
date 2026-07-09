namespace AppManager.Permissions;

public static class AppManagerPermissions
{
    public const string GroupName = "AppManager";

    public static class IisSites
    {
        public const string Default = GroupName + ".IisSites";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string ManageBinding = Default + ".ManageBinding";
        public const string ManageAppPool = Default + ".ManageAppPool";
        public const string ManagePermissions = Default + ".ManagePermissions";
        public const string Backup = Default + ".Backup";
        public const string Restore = Default + ".Restore";
    }

    public static class WindowsServices
    {
        public const string Default = GroupName + ".WindowsServices";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Start = Default + ".Start";
        public const string Stop = Default + ".Stop";
        public const string Restart = Default + ".Restart";
        public const string Backup = Default + ".Backup";
        public const string Restore = Default + ".Restore";
    }

    public static class SystemLogs
    {
        public const string Default = GroupName + ".SystemLogs";
        public const string View = Default + ".View";
    }

    public static class AuditLogs
    {
        public const string Default = GroupName + ".AuditLogs";
        public const string View = Default + ".View";
    }

    public static class PermissionManagement
    {
        public const string Default = GroupName + ".PermissionManagement";
        public const string ManagePermissions = Default + ".ManagePermissions";
    }
}
