using System.Collections.Generic;
using System.Threading.Tasks;
using AppManager.IisSites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppManager.Web.Pages.IisSites;

public class IndexModel : AppManagerPageModel
{
    private readonly IIisSiteAppService _iisSiteAppService;
    private readonly IIisInstanceAppService _instanceAppService;

    [BindProperty(SupportsGet = true)]
    public GetIisSiteListDto Input { get; set; } = new();

    public List<SelectListItem> InstanceList { get; set; } = new();

    public IndexModel(IIisSiteAppService iisSiteAppService, IIisInstanceAppService instanceAppService)
    {
        _iisSiteAppService = iisSiteAppService;
        _instanceAppService = instanceAppService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Input.MaxResultCount = Input.MaxResultCount > 0 ? Input.MaxResultCount : 20;

        var instances = await _instanceAppService.GetListAsync(new Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto { MaxResultCount = 100 });
        InstanceList = new List<SelectListItem> { new SelectListItem("所有实例", "") };
        foreach (var item in instances.Items)
            InstanceList.Add(new SelectListItem(item.Name, item.Id.ToString()));

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