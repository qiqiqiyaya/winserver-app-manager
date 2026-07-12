using System;
using System.Text.Json;
using System.Threading.Tasks;
using AppManager.WindowsService;
using WindowsServiceEntity = AppManager.WindowsService.WindowsService;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace AppManager.Backups;

public class WindowsServiceBackupService : IWindowsServiceBackupService, ITransientDependency
{
    private readonly IWindowsServiceManager _svcMgr;
    private readonly IRepository<WindowsServiceEntity, Guid> _repo;
    private readonly IRepository<WindowsServiceBackup, Guid> _backupRepo;
    private readonly ILogger<WindowsServiceBackupService> _log;

    public WindowsServiceBackupService(
        IWindowsServiceManager svcMgr,
        IRepository<WindowsServiceEntity, Guid> repo,
        IRepository<WindowsServiceBackup, Guid> backupRepo,
        ILogger<WindowsServiceBackupService> log)
    {
        _svcMgr = svcMgr; _repo = repo; _backupRepo = backupRepo; _log = log;
    }

    public async Task<WindowsServiceBackup> CreateBackupAsync(string serviceName, string? description)
    {
        var svc = await _repo.FindAsync(s => s.ServiceName == serviceName)
            ?? throw new InvalidOperationException($"Service {serviceName} not found");

        var backup = new WindowsServiceBackup
        {
            ServiceName = svc.ServiceName,
            DisplayName = svc.DisplayName,
            BackupData = JsonSerializer.Serialize(new
            {
                svc.ServiceName, svc.DisplayName, svc.Description, svc.ExecutablePath,
                svc.StartType, svc.Account, svc.FailureActionsJson, svc.DependenciesJson, svc.Status
            }),
            Description = description,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = ""
        };
        _log.LogInformation("Created service backup for {ServiceName}", serviceName);
        return backup;
    }

    public async Task RestoreFromBackupAsync(Guid backupId, bool overwrite)
    {
        var backup = await _backupRepo.GetAsync(backupId)!;
        var data = JsonSerializer.Deserialize<JsonElement>(backup!.BackupData);
        var name = data.GetProperty("ServiceName").GetString()!;

        if (overwrite) { try { await _svcMgr.DeleteServiceAsync(name); } catch { } }

        var svc = new WindowsServiceEntity
        {
            ServiceName = name,
            DisplayName = data.GetProperty("DisplayName").GetString()!,
            ExecutablePath = data.GetProperty("ExecutablePath").GetString()!,
            StartType = data.TryGetProperty("StartType", out var st) ? st.GetString()! : "Manual",
            Account = data.TryGetProperty("Account", out var a) ? a.GetString()! : "LocalSystem"
        };
        await _svcMgr.CreateServiceAsync(svc);
        _log.LogInformation("Restored service {ServiceName} from backup", name);
    }

    public async Task<string> PreviewBackupAsync(Guid backupId)
    {
        var backup = await _backupRepo.GetAsync(backupId);
        return backup?.BackupData ?? "{}";
    }
}
