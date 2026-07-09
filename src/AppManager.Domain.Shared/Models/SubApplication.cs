namespace AppManager.Models;

public class SubApplication
{
    public string Alias { get; set; } = string.Empty;
    public string PhysicalPath { get; set; } = string.Empty;
    public string? AppPoolName { get; set; }
}
