using System;
using System.Threading.Tasks;
using AppManager.IisSites;
using Microsoft.AspNetCore.Mvc;

namespace AppManager.Web.Pages.IisSites;

public class EditModalModel : AppManagerPageModel
{
    private readonly IIisSiteAppService _iisSiteAppService;

    [BindProperty]
    public UpdateIisSiteDto Input { get; set; } = new();

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public EditModalModel(IIisSiteAppService iisSiteAppService)
    {
        _iisSiteAppService = iisSiteAppService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var site = await _iisSiteAppService.GetAsync(Id);
        Input = new UpdateIisSiteDto
        {
            SiteName = site.SiteName,
            PhysicalPath = site.PhysicalPath,
            Port = site.Port
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _iisSiteAppService.UpdateAsync(Id, Input);
        return NoContent();
    }
}
