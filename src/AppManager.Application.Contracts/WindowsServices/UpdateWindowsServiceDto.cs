namespace AppManager.WindowsServices;

public class UpdateWindowsServiceDto
{
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public string? StartType { get; set; }
    public string? Account { get; set; }
    public string? Password { get; set; }
    public string? ExecutablePath { get; set; }
}
