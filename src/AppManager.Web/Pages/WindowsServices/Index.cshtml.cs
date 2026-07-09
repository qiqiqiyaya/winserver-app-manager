using System.Threading.Tasks;
using AppManager.WindowsServices;
using Microsoft.AspNetCore.Mvc;

namespace AppManager.Web.Pages.WindowsServices;

public class IndexModel : AppManagerPageModel
{
    private readonly IWindowsServiceAppService _windowsServiceAppService;

    [BindProperty(SupportsGet = true)]
    public GetWindowsServiceListDto Input { get; set; } = new();

    public IndexModel(IWindowsServiceAppService windowsServiceAppService)
    {
        _windowsServiceAppService = windowsServiceAppService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Input.MaxResultCount = Input.MaxResultCount > 0 ? Input.MaxResultCount : 20;
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        if (System.Guid.TryParse(id, out var guid))
            await _windowsServiceAppService.DeleteAsync(guid);
        return NoContent();
    }

    public async Task<IActionResult> OnPostStartAsync(string id)
    {
        if (System.Guid.TryParse(id, out var guid))
            await _windowsServiceAppService.StartAsync(guid);
        return NoContent();
    }

    public async Task<IActionResult> OnPostStopAsync(string id)
    {
        if (System.Guid.TryParse(id, out var guid))
            await _windowsServiceAppService.StopAsync(guid);
        return NoContent();
    }

    public async Task<IActionResult> OnPostRestartAsync(string id)
    {
        if (System.Guid.TryParse(id, out var guid))
            await _windowsServiceAppService.RestartAsync(guid);
        return NoContent();
    }
}
