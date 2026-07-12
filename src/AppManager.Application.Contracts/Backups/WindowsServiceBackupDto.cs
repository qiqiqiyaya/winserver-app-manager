using System;
using Volo.Abp.Application.Dtos;

namespace AppManager.Backups;

public class WindowsServiceBackupDto : EntityDto<Guid>
{
    public string ServiceName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}
