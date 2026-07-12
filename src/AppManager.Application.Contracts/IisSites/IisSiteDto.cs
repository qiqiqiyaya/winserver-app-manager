using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace AppManager.IisSites;

public class IisSiteDto : EntityDto<Guid>
{
    public string SiteName { get; set; } = string.Empty;
    public string PhysicalPath { get; set; } = string.Empty;
    public string Status { get; set; } = "Unknown";
    public int? Port { get; set; }
    public Guid IisInstanceId { get; set; }
    public string? IisInstanceName { get; set; }
    public List<SiteBindingDto> Bindings { get; set; } = new();
    public string? AppPoolName { get; set; }
    public AppPoolConfigDto? AppPoolConfig { get; set; }
    public List<SubApplicationDto> SubApplications { get; set; } = new();
    public List<VirtualDirectoryDto> VirtualDirectories { get; set; } = new();
    public List<NtfsPermissionDto> NtfsPermissions { get; set; } = new();
    public DateTime CreationTime { get; set; }
}
