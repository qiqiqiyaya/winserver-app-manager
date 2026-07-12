using System;

namespace AppManager.Backups;

public class RestoreBackupDto
{
    public Guid BackupId { get; set; }
    public bool Overwrite { get; set; }
    public string? NewSiteName { get; set; }
}
