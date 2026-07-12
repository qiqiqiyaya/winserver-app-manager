using System.ComponentModel.DataAnnotations;

namespace AppManager.IisSites;

public class CreateIisInstanceDto
{
    [Required]
    [StringLength(256)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// applicationHost.config 路径。为空使用系统默认路径。
    /// </summary>
    [StringLength(1024)]
    public string ConfigPath { get; set; } = string.Empty;
}
