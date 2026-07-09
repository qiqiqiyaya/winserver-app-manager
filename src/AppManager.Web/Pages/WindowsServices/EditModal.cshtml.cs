using System;
using System.Threading.Tasks;
using AppManager.WindowsServices;
using Microsoft.AspNetCore.Mvc;

namespace AppManager.Web.Pages.WindowsServices;

public class EditModalModel : AppManagerPageModel
{
    private readonly IWindowsServiceAppService _svc;

    [BindProperty]
    public UpdateWindowsServiceDto Input { get; set; } = new();

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public EditModalModel(IWindowsServiceAppService svc) { _svc = svc; }

    public async Task<IActionResult> OnGetAsync()
    {
        var s = await _svc.GetAsync(Id);
        Input = new UpdateWindowsServiceDto { DisplayName = s.DisplayName, Description = s.Description, StartType = s.StartType, Account = s.Account, ExecutablePath = s.ExecutablePath };
        return Partial("_EditModal");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _svc.UpdateAsync(Id, Input);
        return NoContent();
    }
}
