using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace AppManager.WindowsService;

public class WindowsService : FullAuditedAggregateRoot<Guid>
{
    public string ServiceName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ExecutablePath { get; set; } = string.Empty;
    public string StartType { get; set; } = "Manual";
    public string Account { get; set; } = "LocalSystem";
    public string? Password { get; set; }
    public string? FailureActionsJson { get; set; }
    public string? DependenciesJson { get; set; }
    public string Status { get; set; } = "Unknown";
}
