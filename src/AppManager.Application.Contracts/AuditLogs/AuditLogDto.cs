using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace AppManager.AuditLogs;

public class AuditLogDto : EntityDto<Guid>
{
    public string? UserName { get; set; }
    public string? ClientIpAddress { get; set; }
    public string? Url { get; set; }
    public string? HttpMethod { get; set; }
    public int HttpStatusCode { get; set; }
    public int ExecutionDuration { get; set; }
    public DateTime ExecutionTime { get; set; }
    public string? BrowserInfo { get; set; }
    public string? ApplicationName { get; set; }
}
