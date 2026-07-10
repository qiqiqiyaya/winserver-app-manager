using System.Threading.Tasks;
using AppManager.IisSites;
using Microsoft.AspNetCore.Mvc;

namespace AppManager.Web.Pages.IisSites;

public class CreateModalModel : AppManagerPageModel
{
    private readonly IIisSiteAppService _iisSiteAppService;

    [BindProperty]
    public CreateIisSiteDto Input { get; set; } = new();

    public CreateModalModel(IIisSiteAppService iisSiteAppService)
    {
        _iisSiteAppService = iisSiteAppService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _iisSiteAppService.CreateAsync(Input);
        return NoContent();
    }
}
