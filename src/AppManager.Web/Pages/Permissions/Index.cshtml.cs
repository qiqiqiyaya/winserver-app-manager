using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppManager.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;

namespace AppManager.Web.Pages.Permissions;

[Authorize(AppManagerPermissions.PermissionManagement.ManagePermissions)]
public class IndexModel : AppManagerPageModel
{
    private readonly IPermissionAppService _permissionAppService;
    private readonly IIdentityRoleAppService _roleAppService;

    public List<SelectListItem> Roles { get; set; } = new();

    [BindProperty]
    public string SelectedRole { get; set; } = "admin";

    public IndexModel(IPermissionAppService permissionAppService, IIdentityRoleAppService roleAppService)
    {
        _permissionAppService = permissionAppService;
        _roleAppService = roleAppService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var roles = await _roleAppService.GetAllListAsync();
        Roles = roles.Items.Select(r => new SelectListItem(r.Name, r.Name)).ToList();
        return Page();
    }

    public async Task<IActionResult> OnGetPermissionTreeAsync(string roleName)
    {
        SelectedRole = roleName;
        return Page();
    }
}
