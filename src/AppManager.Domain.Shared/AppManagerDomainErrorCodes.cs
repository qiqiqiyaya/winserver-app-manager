namespace AppManager;

public static class AppManagerDomainErrorCodes
{
    /* IIS 站点相关错误码 */
    public const string SiteAlreadyExists = "AppManager:01001";
    public const string SiteNotFound = "AppManager:01002";
    public const string SiteStartFailed = "AppManager:01003";
    public const string SiteStopFailed = "AppManager:01004";

    /* Windows 服务相关错误码 */
    public const string ServiceAlreadyExists = "AppManager:02001";
    public const string ServiceNotFound = "AppManager:02002";
    public const string ServiceStartFailed = "AppManager:02003";
    public const string ServiceStopFailed = "AppManager:02004";
    public const string ServiceRestartFailed = "AppManager:02005";

    /* 备份相关错误码 */
    public const string BackupNotFound = "AppManager:03001";
    public const string BackupCreateFailed = "AppManager:03002";
    public const string BackupRestoreFailed = "AppManager:03003";

    /* IIS 操作错误 */
    public const string BindingAddFailed = "AppManager:04001";
    public const string BindingRemoveFailed = "AppManager:04002";
    public const string AppPoolConfigFailed = "AppManager:04003";
    public const string SubApplicationAddFailed = "AppManager:04004";
    public const string SubApplicationRemoveFailed = "AppManager:04005";
    public const string VirtualDirectoryAddFailed = "AppManager:04006";
    public const string VirtualDirectoryRemoveFailed = "AppManager:04007";
    public const string NtfsPermissionSetFailed = "AppManager:04008";
}
