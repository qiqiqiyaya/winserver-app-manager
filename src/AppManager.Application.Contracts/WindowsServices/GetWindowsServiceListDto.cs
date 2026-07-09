using Volo.Abp.Application.Dtos;

namespace AppManager.WindowsServices;

public class GetWindowsServiceListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
