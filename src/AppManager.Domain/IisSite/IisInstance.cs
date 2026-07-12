using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace AppManager.IisSite;

public class IisInstance : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 显示名称，如 "Default"、"Staging"
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// applicationHost.config 的完整路径。
    /// 为空时使用系统默认路径 (%WINDIR%\System32\inetsrv\config\applicationHost.config)
    /// </summary>
    public string ConfigPath { get; set; } = string.Empty;

    /// <summary>
    /// 连接状态：Connected / Disconnected / Error
    /// </summary>
    public string Status { get; set; } = "Unknown";
}
