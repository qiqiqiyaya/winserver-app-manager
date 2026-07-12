using System;
using Volo.Abp.Application.Dtos;

namespace AppManager.IisSites;

public class GetIisSiteListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public Guid? IisInstanceId { get; set; }
}
