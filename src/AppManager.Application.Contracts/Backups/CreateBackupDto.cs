namespace AppManager.Backups;

public class CreateBackupDto
{
    public string SiteOrServiceName { get; set; } = string.Empty;
    public string? Description { get; set; }
}
