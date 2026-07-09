namespace AppManager.IisSites;

public class SubApplicationDto
{
    public string Alias { get; set; } = string.Empty;
    public string PhysicalPath { get; set; } = string.Empty;
    public string? AppPoolName { get; set; }
}
