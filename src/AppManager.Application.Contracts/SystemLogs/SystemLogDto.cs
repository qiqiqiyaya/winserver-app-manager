using System;
using Volo.Abp.Application.Dtos;

namespace AppManager.SystemLogs;

public class SystemLogDto : EntityDto<int>
{
    public string? Message { get; set; }
    public string? MessageTemplate { get; set; }
    public string? Level { get; set; }
    public DateTime? TimeStamp { get; set; }
    public string? Exception { get; set; }
    public string? Properties { get; set; }
    public string? SourceContext { get; set; }
    public string? RequestPath { get; set; }
    public string? UserName { get; set; }
}
