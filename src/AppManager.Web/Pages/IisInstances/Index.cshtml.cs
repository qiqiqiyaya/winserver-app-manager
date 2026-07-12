using System.Threading.Tasks;
using AppManager.IisSites;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace AppManager.Web.Pages.IisInstances;

public class IndexModel : AppManagerPageModel
{
    private readonly IIisInstanceAppService _instanceAppService;

    [BindProperty(SupportsGet = true)]
    public PagedAndSortedResultRequestDto Input { get; set; } = new();

    public IndexModel(IIisInstanceAppService instanceAppService)
    {
        _instanceAppService = instanceAppService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Input.MaxResultCount = Input.MaxResultCount > 0 ? Input.MaxResultCount : 50;
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        if (System.Guid.TryParse(id, out var guid))
            await _instanceAppService.DeleteAsync(guid);
        return NoContent();
    }
}