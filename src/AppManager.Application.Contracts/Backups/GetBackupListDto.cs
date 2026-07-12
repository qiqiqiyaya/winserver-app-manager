using Volo.Abp.Application.Dtos;

namespace AppManager.Backups;

public class GetBackupListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
