using System.Threading.Tasks;
using AppManager.Localization;
using AppManager.MultiTenancy;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;

namespace AppManager.Web.Menus;

public class AppManagerMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var administration = context.Menu.GetAdministration();
        var l = context.GetLocalizer<AppManagerResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                AppManagerMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fas fa-home",
                order: 0
            )
        );

        // IIS site management
        context.Menu.AddItem(
            new ApplicationMenuItem(
                "IisSites",
                l["Menu:IisSites"],
                "~/IisSites",
                icon: "fas fa-globe",
                order: 1
            )
        );

        // Windows service management
        context.Menu.AddItem(
            new ApplicationMenuItem(
                "WindowsServices",
                l["Menu:WindowsServices"],
                "~/WindowsServices",
                icon: "fas fa-cog",
                order: 2
            )
        );

        // Backup management
        var backups = new ApplicationMenuItem(
            "Backups",
            l["Menu:Backups"],
            icon: "fas fa-archive",
            order: 3
        );
        backups.AddItem(new ApplicationMenuItem(
            "IisSiteBackups",
            l["Menu:IisSiteBackups"],
            "~/Backups/IisSiteBackups"
        ));
        backups.AddItem(new ApplicationMenuItem(
            "WindowsServiceBackups",
            l["Menu:WindowsServiceBackups"],
            "~/Backups/WindowsServiceBackups"
        ));
        context.Menu.AddItem(backups);

        // System
        var system = new ApplicationMenuItem(
            "System",
            l["Menu:System"],
            icon: "fas fa-tools",
            order: 98
        );
        system.AddItem(new ApplicationMenuItem(
            "Permissions",
            l["Menu:Permissions"],
            "~/Permissions"
        ));
        system.AddItem(new ApplicationMenuItem(
            "SystemLogs",
            l["Menu:SystemLogs"],
            "~/SystemLogs"
        ));
        system.AddItem(new ApplicationMenuItem(
            "AuditLogs",
            l["Menu:AuditLogs"],
            "~/AuditLogs"
        ));
        context.Menu.AddItem(system);

        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);

        return Task.CompletedTask;
    }
}
