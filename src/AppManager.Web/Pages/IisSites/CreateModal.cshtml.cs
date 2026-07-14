using System.Collections.Generic;
using System.Threading.Tasks;
using AppManager.IisSites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppManager.Web.Pages.IisSites;

public class CreateModalModel : AppManagerPageModel
{
    private readonly IIisSiteAppService _iisSiteAppService;
    private readonly IIisInstanceAppService _instanceAppService;

    [BindProperty]
    public CreateIisSiteDto Input { get; set; } = new();

    public List<SelectListItem> InstanceList { get; set; } = new();

    public CreateModalModel(IIisSiteAppService iisSiteAppService, IIisInstanceAppService instanceAppService)
    {
        _iisSiteAppService = iisSiteAppService;
        _instanceAppService = instanceAppService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var instances = await _instanceAppService.GetListAsync(new Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto { MaxResultCount = 100 });
        InstanceList = new List<SelectListItem>();
        foreach (var item in instances.Items)
            InstanceList.Add(new SelectListItem(item.Name, item.Id.ToString()));

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _iisSiteAppService.CreateAsync(Input);
        return NoContent();
    }
}