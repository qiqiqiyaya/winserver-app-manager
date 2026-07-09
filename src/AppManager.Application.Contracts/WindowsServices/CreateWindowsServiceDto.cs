using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppManager.WindowsServices;

public class CreateWindowsServiceDto
{
    [Required][StringLength(256)] public string ServiceName { get; set; } = string.Empty;
    [Required][StringLength(256)] public string DisplayName { get; set; } = string.Empty;
    [StringLength(1024)] public string? Description { get; set; }
    [Required][StringLength(1024)] public string ExecutablePath { get; set; } = string.Empty;
    public string StartType { get; set; } = "Manual";
    public string Account { get; set; } = "LocalSystem";
    public string? Password { get; set; }
    public List<FailureActionDto>? FailureActions { get; set; }
    public List<string>? Dependencies { get; set; }
}
