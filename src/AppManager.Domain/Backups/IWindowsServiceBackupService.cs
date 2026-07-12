using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AppManager.Backups;

public interface IWindowsServiceBackupService : ITransientDependency
{
    Task<WindowsServiceBackup> CreateBackupAsync(string serviceName, string? description);
    Task RestoreFromBackupAsync(Guid backupId, bool overwrite);
    Task<string> PreviewBackupAsync(Guid backupId);
}