using Volo.Abp.Application.Dtos;

namespace AppManager.IisSites;

public class GetIisSiteListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
