using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AppManager.Services;

public interface IIisBackupService : ITransientDependency
{
    Task<Backups.IisSiteBackup> CreateBackupAsync(string siteName, string? description);
    Task RestoreFromBackupAsync(Guid backupId, bool overwrite, string? newSiteName = null);
    Task<string> PreviewBackupAsync(Guid backupId);
}
