using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace AppManager.IisSite;

public class IisSite : FullAuditedAggregateRoot<Guid>
{
    public string SiteName { get; set; } = string.Empty;
    public string PhysicalPath { get; set; } = string.Empty;
    public string Status { get; set; } = "Unknown";
    public int? Port { get; set; }
    public string? BindingsJson { get; set; }
    public string? AppPoolName { get; set; }
    public string? AppPoolConfigJson { get; set; }
    public string? SubApplicationsJson { get; set; }
    public string? VirtualDirectoriesJson { get; set; }
    public string? NtfsPermissionsJson { get; set; }

    /// <summary>
    /// 所属 IIS 实例 ID（通过 ID 引用，无导航属性）
    /// </summary>
    public Guid IisInstanceId { get; set; }
}
