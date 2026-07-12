using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace AppManager.Backups;

public class IisSiteBackup : FullAuditedAggregateRoot<Guid>
{
    public string SiteName { get; set; } = string.Empty;
    public string BackupData { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}
