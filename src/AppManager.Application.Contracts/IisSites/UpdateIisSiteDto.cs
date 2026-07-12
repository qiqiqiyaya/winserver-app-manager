using System;

namespace AppManager.IisSites;

public class UpdateIisSiteDto
{
    public string? SiteName { get; set; }
    public string? PhysicalPath { get; set; }
    public int? Port { get; set; }
    public Guid? IisInstanceId { get; set; }
}
