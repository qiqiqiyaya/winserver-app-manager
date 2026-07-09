using System;
using System.Threading.Tasks;
using AppManager.IisSite;
using IisSiteEntity = AppManager.IisSite.IisSite;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace AppManager.Services;

public class IisBackupService : IIisBackupService, ITransientDependency
{
    private readonly IIisManager _iisManager;
    private readonly IRepository<IisSiteEntity, Guid> _siteRepository;
    private readonly IRepository<Backups.IisSiteBackup, Guid> _backupRepository;
    private readonly ILogger<IisBackupService> _logger;

    public IisBackupService(
        IIisManager iisManager,
        IRepository<IisSiteEntity, Guid> siteRepository,
        IRepository<Backups.IisSiteBackup, Guid> backupRepository,
        ILogger<IisBackupService> logger)
    {
        _iisManager = iisManager;
        _siteRepository = siteRepository;
        _backupRepository = backupRepository;
        _logger = logger;
    }

    public async Task<Backups.IisSiteBackup> CreateBackupAsync(string siteName, string? description)
    {
        var site = await _siteRepository.FindAsync(s => s.SiteName == siteName)
            ?? throw new InvalidOperationException($"Site {siteName} not found in database");

        var backup = new Backups.IisSiteBackup
        {
            SiteName = site.SiteName,
            BackupData = System.Text.Json.JsonSerializer.Serialize(new
            {
                site.SiteName, site.PhysicalPath, site.Port, site.Status,
                site.BindingsJson, site.AppPoolName, site.AppPoolConfigJson,
                site.SubApplicationsJson, site.VirtualDirectoriesJson, site.NtfsPermissionsJson
            }),
            Description = description,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = ""
        };
        _logger.LogInformation("Created IIS site backup for {SiteName}", siteName);
        return backup;
    }

    public async Task RestoreFromBackupAsync(Guid backupId, bool overwrite, string? newSiteName)
    {
        var backup = await _backupRepository.GetAsync(backupId)
            ?? throw new InvalidOperationException($"Backup {backupId} not found");

        var backupData = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(backup.BackupData);
        var siteName = newSiteName ?? backupData.GetProperty("SiteName").GetString()!;
        var physicalPath = backupData.GetProperty("PhysicalPath").GetString()!;

        if (overwrite)
        {
            try { await _iisManager.DeleteSiteAsync(siteName); }
            catch (Exception ex) { _logger.LogWarning(ex, "Failed to delete existing site before restore"); }
        }

        var site = new IisSiteEntity { SiteName = siteName, PhysicalPath = physicalPath, Status = "Unknown" };
        await _iisManager.CreateSiteAsync(site);
        _logger.LogInformation("Restored IIS site {SiteName} from backup {BackupId}", siteName, backupId);
    }

    public async Task<string> PreviewBackupAsync(Guid backupId)
    {
        var backup = await _backupRepository.GetAsync(backupId);
        return backup?.BackupData ?? "{}";
    }
}
