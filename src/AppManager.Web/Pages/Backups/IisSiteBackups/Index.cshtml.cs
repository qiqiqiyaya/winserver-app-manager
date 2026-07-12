using System.Threading.Tasks;
using AppManager.Backups;
using Microsoft.AspNetCore.Mvc;

namespace AppManager.Web.Pages.Backups.IisSiteBackups;

public class IndexModel : AppManagerPageModel
{
    private readonly IIisSiteBackupAppService _backupAppService;

    [BindProperty(SupportsGet = true)]
    public GetBackupListDto Input { get; set; } = new();

    public IndexModel(IIisSiteBackupAppService backupAppService) { _backupAppService = backupAppService; }

    public async Task<IActionResult> OnGetAsync()
    {
        Input.MaxResultCount = Input.MaxResultCount > 0 ? Input.MaxResultCount : 20;
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        if (System.Guid.TryParse(id, out var g)) await _backupAppService.DeleteAsync(g);
        return NoContent();
    }
}
