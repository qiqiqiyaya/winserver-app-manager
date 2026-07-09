using System.Threading.Tasks;
using AppManager.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace AppManager.Data;

public class AppManagerPermissionDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IPermissionDataSeeder _permissionDataSeeder;

    public AppManagerPermissionDataSeedContributor(IPermissionDataSeeder permissionDataSeeder)
    {
        _permissionDataSeeder = permissionDataSeeder;
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        await _permissionDataSeeder.SeedAsync(
            "R",
            "admin",
            new[]
            {
                AppManagerPermissions.IisSites.Default,
                AppManagerPermissions.IisSites.Create,
                AppManagerPermissions.IisSites.Edit,
                AppManagerPermissions.IisSites.Delete,
                AppManagerPermissions.IisSites.ManageBinding,
                AppManagerPermissions.IisSites.ManageAppPool,
                AppManagerPermissions.IisSites.ManagePermissions,
                AppManagerPermissions.IisSites.Backup,
                AppManagerPermissions.IisSites.Restore,
                AppManagerPermissions.WindowsServices.Default,
                AppManagerPermissions.WindowsServices.Create,
                AppManagerPermissions.WindowsServices.Edit,
                AppManagerPermissions.WindowsServices.Delete,
                AppManagerPermissions.WindowsServices.Start,
                AppManagerPermissions.WindowsServices.Stop,
                AppManagerPermissions.WindowsServices.Restart,
                AppManagerPermissions.WindowsServices.Backup,
                AppManagerPermissions.WindowsServices.Restore,
                AppManagerPermissions.SystemLogs.View,
                AppManagerPermissions.AuditLogs.View,
                AppManagerPermissions.PermissionManagement.ManagePermissions,
            },
            context.TenantId);
    }
}
