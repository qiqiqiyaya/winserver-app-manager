using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppManager.IisSites;
using Microsoft.AspNetCore.Mvc;

namespace AppManager.Web.Pages.IisSites;

public class ManageModel : AppManagerPageModel
{
    private readonly IIisSiteAppService _iisSiteAppService;

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public IisSiteDto Site { get; set; } = new();
    public List<SiteBindingDto> Bindings { get; set; } = new();
    public List<SubApplicationDto> SubApplications { get; set; } = new();
    public List<VirtualDirectoryDto> VirtualDirectories { get; set; } = new();
    public List<NtfsPermissionDto> NtfsPermissions { get; set; } = new();
    public AppPoolConfigDto AppPoolConfig { get; set; } = new();

    [BindProperty]
    public SiteBindingDto NewBinding { get; set; } = new();
    [BindProperty]
    public SubApplicationDto NewSubApp { get; set; } = new();
    [BindProperty]
    public VirtualDirectoryDto NewVDir { get; set; } = new();
    [BindProperty]
    public NtfsPermissionDto NewPermission { get; set; } = new();
    [BindProperty]
    public AppPoolConfigDto EditAppPool { get; set; } = new();

    public ManageModel(IIisSiteAppService iisSiteAppService)
    {
        _iisSiteAppService = iisSiteAppService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadAllAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAddBindingAsync()
    {
        await _iisSiteAppService.AddBindingAsync(Id, NewBinding);
        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostRemoveBindingAsync(string protocol, string ip, int port, string host)
    {
        await _iisSiteAppService.RemoveBindingAsync(Id, new SiteBindingDto
        {
            Protocol = protocol, IpAddress = ip, Port = port, HostName = host
        });
        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostAddSubAppAsync()
    {
        await _iisSiteAppService.AddSubApplicationAsync(Id, NewSubApp);
        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostRemoveSubAppAsync(string alias)
    {
        await _iisSiteAppService.RemoveSubApplicationAsync(Id, alias);
        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostAddVDirAsync()
    {
        await _iisSiteAppService.AddVirtualDirectoryAsync(Id, NewVDir);
        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostRemoveVDirAsync(string alias)
    {
        await _iisSiteAppService.RemoveVirtualDirectoryAsync(Id, alias);
        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostUpdateAppPoolAsync()
    {
        await _iisSiteAppService.UpdateAppPoolConfigAsync(Id, EditAppPool);
        return RedirectToPage(new { Id });
    }

    private async Task LoadAllAsync()
    {
        Site = await _iisSiteAppService.GetAsync(Id);
        Bindings = await _iisSiteAppService.GetBindingsAsync(Id);
        SubApplications = await _iisSiteAppService.GetSubApplicationsAsync(Id);
        VirtualDirectories = await _iisSiteAppService.GetVirtualDirectoriesAsync(Id);
        NtfsPermissions = await _iisSiteAppService.GetNtfsPermissionsAsync(Id);
        AppPoolConfig = await _iisSiteAppService.GetAppPoolConfigAsync(Id);
    }
}
