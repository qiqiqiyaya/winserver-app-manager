using System.Threading.Tasks;
using AppManager.AuditLogs;
using Microsoft.AspNetCore.Mvc;

namespace AppManager.Web.Pages.AuditLogs;

public class IndexModel : AppManagerPageModel
{
    private readonly IAuditLogAppService _auditLogAppService;

    [BindProperty(SupportsGet = true)]
    public AuditLogFilterDto Input { get; set; } = new();

    public IndexModel(IAuditLogAppService auditLogAppService) { _auditLogAppService = auditLogAppService; }

    public async Task<IActionResult> OnGetAsync()
    {
        Input.MaxResultCount = Input.MaxResultCount > 0 ? Input.MaxResultCount : 20;
        return Page();
    }
}
