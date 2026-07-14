<!-- Generated: 2026-07-14 | Files scanned: ~700 | Token estimate: ~950 -->

# Backend Architecture

## API Layer (ABP Auto Conventional Controllers)

```
GET    /api/app/iis-site               → IIisSiteAppService.GetListAsync
GET    /api/app/iis-site/{id}          → IIisSiteAppService.GetAsync
POST   /api/app/iis-site               → IIisSiteAppService.CreateAsync
PUT    /api/app/iis-site/{id}          → IIisSiteAppService.UpdateAsync
DELETE /api/app/iis-site/{id}          → IIisSiteAppService.DeleteAsync
POST   /api/app/iis-site/{id}/start    → IIisSiteAppService.StartAsync
POST   /api/app/iis-site/{id}/stop     → IIisSiteAppService.StopAsync
GET    /api/app/iis-site/{id}/bindings → IIisSiteAppService.GetBindingsAsync
POST   /api/app/iis-site/{id}/bindings → IIisSiteAppService.AddBindingAsync
DELETE /api/app/iis-site/{id}/bindings → IIisSiteAppService.RemoveBindingAsync
GET    /api/app/iis-site/{id}/app-pool → IIisSiteAppService.GetAppPoolConfigAsync
PUT    /api/app/iis-site/{id}/app-pool → IIisSiteAppService.UpdateAppPoolConfigAsync
GET    /api/app/iis-site/{id}/sub-apps → IIisSiteAppService.GetSubApplicationsAsync
POST   /api/app/iis-site/{id}/sub-apps → IIisSiteAppService.AddSubApplicationAsync
DELETE /api/app/iis-site/{id}/sub-apps → IIisSiteAppService.RemoveSubApplicationAsync
GET    /api/app/iis-site/{id}/vdirs    → IIisSiteAppService.GetVirtualDirectoriesAsync
POST   /api/app/iis-site/{id}/vdirs    → IIisSiteAppService.AddVirtualDirectoryAsync
DELETE /api/app/iis-site/{id}/vdirs    → IIisSiteAppService.RemoveVirtualDirectoryAsync
GET    /api/app/iis-site/{id}/ntfs     → IIisSiteAppService.GetNtfsPermissionsAsync
POST   /api/app/iis-site/{id}/ntfs     → IIisSiteAppService.SetNtfsPermissionAsync
DELETE /api/app/iis-site/{id}/ntfs     → IIisSiteAppService.RemoveNtfsPermissionAsync

GET    /api/app/windows-service                → IWindowsServiceAppService.GetListAsync
GET    /api/app/windows-service/{id}           → IWindowsServiceAppService.GetAsync
POST   /api/app/windows-service                → IWindowsServiceAppService.CreateAsync
PUT    /api/app/windows-service/{id}           → IWindowsServiceAppService.UpdateAsync
DELETE /api/app/windows-service/{id}           → IWindowsServiceAppService.DeleteAsync
POST   /api/app/windows-service/{id}/start     → IWindowsServiceAppService.StartAsync
POST   /api/app/windows-service/{id}/stop      → IWindowsServiceAppService.StopAsync
POST   /api/app/windows-service/{id}/restart   → IWindowsServiceAppService.RestartAsync

GET    /api/app/iis-site-backup        → IIisSiteBackupAppService.GetListAsync
POST   /api/app/iis-site-backup        → IIisSiteBackupAppService.CreateAsync
GET    /api/app/iis-site-backup/{id}   → IIisSiteBackupAppService.PreviewAsync
POST   /api/app/iis-site-backup/restore → IIisSiteBackupAppService.RestoreAsync
DELETE /api/app/iis-site-backup/{id}   → IIisSiteBackupAppService.DeleteAsync

GET    /api/app/windows-service-backup        → IWindowsServiceBackupAppService.GetListAsync
POST   /api/app/windows-service-backup        → IWindowsServiceBackupAppService.CreateAsync
GET    /api/app/windows-service-backup/{id}   → IWindowsServiceBackupAppService.PreviewAsync
POST   /api/app/windows-service-backup/restore → IWindowsServiceBackupAppService.RestoreAsync
DELETE /api/app/windows-service-backup/{id}   → IWindowsServiceBackupAppService.DeleteAsync

GET    /api/app/system-log      → ISystemLogAppService.GetListAsync
GET    /api/app/system-log/{id} → ISystemLogAppService.GetAsync
DELETE /api/app/system-log      → ISystemLogAppService.ClearAsync

GET    /api/app/audit-log          → IAuditLogAppService.GetListAsync
GET    /api/app/audit-log/{id}     → IAuditLogAppService.GetDetailAsync
```

## Service → Repository Mapping

| AppService (Application/) | Domain Service (Domain/) | Repository |
|---|---|---|
| `IisSites/IisSiteAppService` | `IisSite/IIisManager` (Microsoft.Web.Administration) | `IRepository<IisSite, Guid>` |
| `WindowsServices/WindowsServiceAppService` | `WindowsService/IWindowsServiceManager` (sc.exe/ServiceController) | `IRepository<WindowsService, Guid>` |
| `Backups/IisSiteBackupAppService` | `Backups/IIisBackupService` | `IRepository<IisSiteBackup, Guid>` |
| `Backups/WindowsServiceBackupAppService` | `Backups/IWindowsServiceBackupService` | `IRepository<WindowsServiceBackup, Guid>` |
| `SystemLogs/SystemLogAppService` | — | `IRepository<SerilogLog, long>` |
| `AuditLogs/AuditLogAppService` | — | `IRepository<AuditLog, Guid>` |

## Error Handling Flow

```
Domain Service 抛异常
  → AppService catch → Logger.LogWarning + throw new BusinessException(errorCode, details: ex.Message)
    → ABP AbpExceptionFilter → JSON { error: { message, details, code }, _AbpErrorFormat: true }
      → 客户端 abp.ajax → showError() → alert 或 notification
```

## Middleware Pipeline (Web)

```
UseDeveloperExceptionPage / UseErrorPage
  → UseAbpRequestLocalization → UseCorrelationId → UseStaticFiles
    → UseRouting → UseAuthentication (OpenIddict) → UseAbpOpenIddictValidation
      → UseMultiTenancy → UseUnitOfWork → UseDynamicClaims → UseAuthorization
        → UseSwagger → UseAbpSwaggerUI → UseAuditing → UseAbpSerilogEnrichers
          → UseConfiguredEndpoints
```

## Key Files

| File | Purpose | Namespace |
|---|---|---|
| `src/AppManager.Application/IisSites/IisSiteAppService.cs` | IIS CRUD + management (编排层) | `AppManager.Application.IisSites` |
| `src/AppManager.Application/WindowsServices/WindowsServiceAppService.cs` | Service CRUD + lifecycle | `AppManager.Application.WindowsServices` |
| `src/AppManager.Domain/IisSite/IisManager.cs` | IIS API 包装 (ServerManager) | `AppManager.IisSite` |
| `src/AppManager.Domain/IisSite/IIisManager.cs` | IIS 领域服务接口 | `AppManager.IisSite` |
| `src/AppManager.Domain/WindowsService/WindowsServiceManager.cs` | sc.exe + ServiceController 包装 | `AppManager.WindowsService` |
| `src/AppManager.Domain/WindowsService/IWindowsServiceManager.cs` | 服务领域服务接口 | `AppManager.WindowsService` |
| `src/AppManager.Domain/Backups/IisBackupService.cs` | IIS 备份/还原逻辑 | `AppManager.Backups` |
| `src/AppManager.Domain/Backups/WindowsServiceBackupService.cs` | 服务备份/还原逻辑 | `AppManager.Backups` |
| `src/AppManager.Web/AppManagerWebModule.cs` | Web startup, middleware pipeline | `AppManager.Web` |
| `src/AppManager.Web/Program.cs` | Entry point, Serilog config | — |