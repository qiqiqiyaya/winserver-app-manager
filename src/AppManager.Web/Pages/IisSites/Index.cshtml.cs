using System;
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

    /// <summary>
    /// 路由参数：从实例列表跳转时传入的实例 ID，用于自动选中筛选器
    /// 路由模板：/IisSites/{iisInstanceId:guid?}
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public Guid? IisInstanceId { get; set; }

    public IndexModel(IIisSiteAppService iisSiteAppService, IIisInstanceAppService instanceAppService)
    {
        _iisSiteAppService = iisSiteAppService;
        _instanceAppService = instanceAppService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Input.MaxResultCount = Input.MaxResultCount > 0 ? Input.MaxResultCount : 20;

        // 如果通过查询参数传入了 IisInstanceId，则设置过滤条件
        if (IisInstanceId.HasValue)
        {
            Input.IisInstanceId = IisInstanceId.Value;
        }

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