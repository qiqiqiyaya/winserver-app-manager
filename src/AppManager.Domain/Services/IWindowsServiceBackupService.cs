using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AppManager.Services;

public interface IWindowsServiceBackupService : ITransientDependency
{
    Task<Backups.WindowsServiceBackup> CreateBackupAsync(string serviceName, string? description);
    Task RestoreFromBackupAsync(Guid backupId, bool overwrite);
    Task<string> PreviewBackupAsync(Guid backupId);
}
