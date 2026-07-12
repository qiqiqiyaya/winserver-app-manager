using System;
using System.Threading.Tasks;
using AppManager.IisSite;
using IisSiteEntity = AppManager.IisSite.IisSite;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace AppManager.Backups;

public class IisBackupService : IIisBackupService, ITransientDependency
{
    private readonly IIisManager _iisManager;
    private readonly IRepository<IisSiteEntity, Guid> _siteRepository;
    private readonly IRepository<IisInstance, Guid> _instanceRepository;
    private readonly IRepository<IisSiteBackup, Guid> _backupRepository;
    private readonly ILogger<IisBackupService> _logger;

    public IisBackupService(
        IIisManager iisManager,
        IRepository<IisSiteEntity, Guid> siteRepository,
        IRepository<IisInstance, Guid> instanceRepository,
        IRepository<IisSiteBackup, Guid> backupRepository,
        ILogger<IisBackupService> logger)
    {
        _iisManager = iisManager;
        _siteRepository = siteRepository;
        _instanceRepository = instanceRepository;
        _backupRepository = backupRepository;
        _logger = logger;
    }

    private async Task<string> GetConfigPathAsync(Guid iisInstanceId)
    {
        var instance = await _instanceRepository.GetAsync(iisInstanceId);
        return instance.ConfigPath;
    }

    public async Task<IisSiteBackup> CreateBackupAsync(string siteName, string? description)
    {
        var site = await _siteRepository.FindAsync(s => s.SiteName == siteName)
            ?? throw new InvalidOperationException($"Site {siteName} not found in database");

        var backup = new IisSiteBackup
        {
            SiteName = site.SiteName,
            BackupData = System.Text.Json.JsonSerializer.Serialize(new
            {
                site.SiteName, site.PhysicalPath, site.Port, site.Status,
                site.BindingsJson, site.AppPoolName, site.AppPoolConfigJson,
                site.SubApplicationsJson, site.VirtualDirectoriesJson, site.NtfsPermissionsJson,
                site.IisInstanceId
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

        // 获取 IisInstanceId（如果备份中有，否则使用默认实例）
        Guid iisInstanceId;
        if (backupData.TryGetProperty("IisInstanceId", out var instanceIdProp))
        {
            iisInstanceId = instanceIdProp.GetGuid();
        }
        else
        {
            var defaultInstance = await _instanceRepository.FindAsync(i => i.ConfigPath == "")
                ?? throw new InvalidOperationException("Default IIS instance not found");
            iisInstanceId = defaultInstance.Id;
        }
        var configPath = await GetConfigPathAsync(iisInstanceId);

        if (overwrite)
        {
            try
            {
                await _iisManager.DeleteSiteAsync(configPath, siteName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete existing site before restore");
                throw new UserFriendlyException($"备份还原前删除现有站点失败: {ex.Message}", details: ex.Message);
            }
        }

        try
        {
            var site = new IisSiteEntity
            {
                SiteName = siteName,
                PhysicalPath = physicalPath,
                Status = "Unknown",
                IisInstanceId = iisInstanceId
            };
            await _iisManager.CreateSiteAsync(configPath, site);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to restore IIS site {SiteName} from backup", siteName);
            throw new UserFriendlyException($"还原 IIS 站点 {siteName} 失败: {ex.Message}", details: ex.Message);
        }

        _logger.LogInformation("Restored IIS site {SiteName} from backup {BackupId}", siteName, backupId);
    }

    public async Task<string> PreviewBackupAsync(Guid backupId)
    {
        var backup = await _backupRepository.GetAsync(backupId);
        return backup?.BackupData ?? "{}";
    }
}
