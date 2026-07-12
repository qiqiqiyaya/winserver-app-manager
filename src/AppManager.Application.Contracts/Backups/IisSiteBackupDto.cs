using System;
using Volo.Abp.Application.Dtos;

namespace AppManager.Backups;

public class IisSiteBackupDto : EntityDto<Guid>
{
    public string SiteName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}
