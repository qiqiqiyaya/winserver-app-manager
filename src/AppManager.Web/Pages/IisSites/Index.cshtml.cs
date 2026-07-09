using System.Threading.Tasks;
using AppManager.IisSites;
using Microsoft.AspNetCore.Mvc;

namespace AppManager.Web.Pages.IisSites;

public class IndexModel : AppManagerPageModel
{
    private readonly IIisSiteAppService _iisSiteAppService;

    [BindProperty(SupportsGet = true)]
    public GetIisSiteListDto Input { get; set; } = new();

    public IndexModel(IIisSiteAppService iisSiteAppService)
    {
        _iisSiteAppService = iisSiteAppService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Input.MaxResultCount = Input.MaxResultCount > 0 ? Input.MaxResultCount : 20;
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        if (System.Guid.TryParse(id, out var guid))
            await _iisSiteAppService.DeleteAsync(guid);
        return NoContent();
    }

    public async Task<IActionResult> OnPostStartAsync(string id)
    {
        if (System.Guid.TryParse(id, out var guid))
            await _iisSiteAppService.StartAsync(guid);
        return NoContent();
    }

    public async Task<IActionResult> OnPostStopAsync(string id)
    {
        if (System.Guid.TryParse(id, out var guid))
            await _iisSiteAppService.StopAsync(guid);
        return NoContent();
    }
}
