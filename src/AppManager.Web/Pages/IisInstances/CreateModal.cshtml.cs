using System.Threading.Tasks;
using AppManager.IisSites;
using Microsoft.AspNetCore.Mvc;

namespace AppManager.Web.Pages.IisInstances;

public class CreateModalModel : AppManagerPageModel
{
    private readonly IIisInstanceAppService _instanceAppService;

    [BindProperty]
    public CreateIisInstanceDto Input { get; set; } = new();

    public CreateModalModel(IIisInstanceAppService instanceAppService)
    {
        _instanceAppService = instanceAppService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _instanceAppService.CreateAsync(Input);
        return NoContent();
    }
}