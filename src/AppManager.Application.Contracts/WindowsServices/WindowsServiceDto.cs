using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace AppManager.WindowsServices;

public class WindowsServiceDto : EntityDto<Guid>
{
    public string ServiceName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ExecutablePath { get; set; } = string.Empty;
    public string StartType { get; set; } = "Manual";
    public string Account { get; set; } = "LocalSystem";
    public string Status { get; set; } = "Unknown";
    public List<FailureActionDto>? FailureActions { get; set; }
    public List<string>? Dependencies { get; set; }
    public DateTime CreationTime { get; set; }
}
