using AppManager.Consts;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AppManager.IisSites;

public class CreateIisInstanceDto
{
    [Required]
    [StringLength(256)]
    [DisplayName("IisInstances:Name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// applicationHost.config 路径。为空使用系统默认路径。
    /// </summary>
    [Required]
    [StringLength(1024)]
    [DisplayName("IisInstances:ConfigPath")]
    public string ConfigPath { get; set; } = IisSiteConsts.DefaultConfigPath;
}
