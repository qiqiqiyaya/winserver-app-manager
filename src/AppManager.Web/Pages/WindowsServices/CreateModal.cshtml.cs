using System.Threading.Tasks;
using AppManager.WindowsServices;
using Microsoft.AspNetCore.Mvc;

namespace AppManager.Web.Pages.WindowsServices;

public class CreateModalModel : AppManagerPageModel
{
    private readonly IWindowsServiceAppService _svc;

    [BindProperty]
    public CreateWindowsServiceDto Input { get; set; } = new();

    public CreateModalModel(IWindowsServiceAppService svc) { _svc = svc; }

    public async Task<IActionResult> OnGetAsync() { return Partial("_CreateModal"); }

    public async Task<IActionResult> OnPostAsync()
    {
        await _svc.CreateAsync(Input);
        return NoContent();
    }
}
