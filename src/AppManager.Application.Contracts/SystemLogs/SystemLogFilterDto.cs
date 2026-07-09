using System;
using Volo.Abp.Application.Dtos;

namespace AppManager.SystemLogs;

public class SystemLogFilterDto : PagedAndSortedResultRequestDto
{
    public string? Level { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Keyword { get; set; }
}
