using System;
using Volo.Abp.Application.Dtos;

namespace AppManager.AuditLogs;

public class AuditLogFilterDto : PagedAndSortedResultRequestDto
{
    public string? UserName { get; set; }
    public string? HttpMethod { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? HttpStatusCode { get; set; }
}
