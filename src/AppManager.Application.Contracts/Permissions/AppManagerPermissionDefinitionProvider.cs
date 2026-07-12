using AppManager.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace AppManager.Permissions;

public class AppManagerPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var appManagerGroup = context.AddGroup(AppManagerPermissions.GroupName, L("Permission:AppManager"));

        var iisSitesPermission = appManagerGroup.AddPermission(
            AppManagerPermissions.IisSites.Default, L("Permission:IisSites"));
        iisSitesPermission.AddChild(
            AppManagerPermissions.IisSites.Create, L("Permission:IisSites.Create"));
        iisSitesPermission.AddChild(
            AppManagerPermissions.IisSites.Edit, L("Permission:IisSites.Edit"));
        iisSitesPermission.AddChild(
            AppManagerPermissions.IisSites.Delete, L("Permission:IisSites.Delete"));
        iisSitesPermission.AddChild(
            AppManagerPermissions.IisSites.ManageBinding, L("Permission:IisSites.ManageBinding"));
        iisSitesPermission.AddChild(
            AppManagerPermissions.IisSites.ManageAppPool, L("Permission:IisSites.ManageAppPool"));
        iisSitesPermission.AddChild(
            AppManagerPermissions.IisSites.ManagePermissions, L("Permission:IisSites.ManagePermissions"));
        iisSitesPermission.AddChild(
            AppManagerPermissions.IisSites.Backup, L("Permission:IisSites.Backup"));
        iisSitesPermission.AddChild(
            AppManagerPermissions.IisSites.Restore, L("Permission:IisSites.Restore"));

        var iisInstancesPermission = appManagerGroup.AddPermission(
            AppManagerPermissions.IisInstances.Default, L("Permission:IisInstances"));
        iisInstancesPermission.AddChild(
            AppManagerPermissions.IisInstances.Create, L("Permission:IisInstances.Create"));
        iisInstancesPermission.AddChild(
            AppManagerPermissions.IisInstances.Edit, L("Permission:IisInstances.Edit"));
        iisInstancesPermission.AddChild(
            AppManagerPermissions.IisInstances.Delete, L("Permission:IisInstances.Delete"));

        var windowsServicesPermission = appManagerGroup.AddPermission(
            AppManagerPermissions.WindowsServices.Default, L("Permission:WindowsServices"));
        windowsServicesPermission.AddChild(
            AppManagerPermissions.WindowsServices.Create, L("Permission:WindowsServices.Create"));
        windowsServicesPermission.AddChild(
            AppManagerPermissions.WindowsServices.Edit, L("Permission:WindowsServices.Edit"));
        windowsServicesPermission.AddChild(
            AppManagerPermissions.WindowsServices.Delete, L("Permission:WindowsServices.Delete"));
        windowsServicesPermission.AddChild(
            AppManagerPermissions.WindowsServices.Start, L("Permission:WindowsServices.Start"));
        windowsServicesPermission.AddChild(
            AppManagerPermissions.WindowsServices.Stop, L("Permission:WindowsServices.Stop"));
        windowsServicesPermission.AddChild(
            AppManagerPermissions.WindowsServices.Restart, L("Permission:WindowsServices.Restart"));
        windowsServicesPermission.AddChild(
            AppManagerPermissions.WindowsServices.Backup, L("Permission:WindowsServices.Backup"));
        windowsServicesPermission.AddChild(
            AppManagerPermissions.WindowsServices.Restore, L("Permission:WindowsServices.Restore"));

        var systemLogsPermission = appManagerGroup.AddPermission(
            AppManagerPermissions.SystemLogs.Default, L("Permission:SystemLogs"));
        systemLogsPermission.AddChild(
            AppManagerPermissions.SystemLogs.View, L("Permission:SystemLogs.View"));

        var auditLogsPermission = appManagerGroup.AddPermission(
            AppManagerPermissions.AuditLogs.Default, L("Permission:AuditLogs"));
        auditLogsPermission.AddChild(
            AppManagerPermissions.AuditLogs.View, L("Permission:AuditLogs.View"));

        appManagerGroup.AddPermission(
            AppManagerPermissions.PermissionManagement.ManagePermissions,
            L("Permission:PermissionManagement.ManagePermissions"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AppManagerResource>(name);
    }
}
