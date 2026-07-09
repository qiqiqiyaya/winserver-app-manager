using System.Threading.Tasks;
using AppManager.SystemLogs;
using Microsoft.AspNetCore.Mvc;

namespace AppManager.Web.Pages.SystemLogs;

public class IndexModel : AppManagerPageModel
{
    private readonly ISystemLogAppService _systemLogAppService;

    [BindProperty(SupportsGet = true)]
    public SystemLogFilterDto Input { get; set; } = new();

    public IndexModel(ISystemLogAppService systemLogAppService) { _systemLogAppService = systemLogAppService; }

    public async Task<IActionResult> OnGetAsync()
    {
        Input.MaxResultCount = Input.MaxResultCount > 0 ? Input.MaxResultCount : 20;
        return Page();
    }
}
