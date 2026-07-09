namespace AppManager.Models;

public class NtfsPermission
{
    public string Identity { get; set; } = string.Empty;
    public string AccessRights { get; set; } = "ReadAndExecute";
}
