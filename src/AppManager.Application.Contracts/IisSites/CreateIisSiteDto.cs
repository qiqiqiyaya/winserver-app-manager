using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppManager.IisSites;

public class CreateIisSiteDto
{
    [Required]
    [StringLength(256)]
    public string SiteName { get; set; } = string.Empty;

    [Required]
    [StringLength(1024)]
    public string PhysicalPath { get; set; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; set; } = 80;

    public List<SiteBindingDto> Bindings { get; set; } = new();
    public string? AppPoolName { get; set; }
    public AppPoolConfigDto? AppPoolConfig { get; set; }
}
