<!-- Generated: 2026-07-11 | Files scanned: 139 | Token estimate: ~950 -->

# Backend Architecture

## API Layer (Auto-generated Conventional Controllers)

ABP Auto API maps `IApplicationService` interfaces to REST endpoints automatically.

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

| AppService | Domain Service | Repository |
|---|---|---|
| `IisSiteAppService` | `IIisManager` (Microsoft.Web.Administration) | `IRepository<IisSite, Guid>` |
| `WindowsServiceAppService` | `IWindowsServiceManager` (sc.exe/ServiceController) | `IRepository<WindowsService, Guid>` |
| `IisSiteBackupAppService` | `IIisBackupService` | `IRepository<IisSiteBackup, Guid>` |
| `WindowsServiceBackupAppService` | `IWindowsServiceBackupService` | `IRepository<WindowsServiceBackup, Guid>` |
| `SystemLogAppService` | (not implemented) | — |
| `AuditLogAppService` | (not implemented) | — |

## Middleware Pipeline (Web)

```
UseDeveloperExceptionPage / UseErrorPage
  → UseAbpRequestLocalization
    → UseCorrelationId
      → UseStaticFiles
        → UseRouting
          → UseAuthentication (OpenIddict)
            → UseMultiTenancy
              → UseUnitOfWork
                → UseDynamicClaims
                  → UseAuthorization
                    → UseSwagger
                      → UseAuditing
                        → UseConfiguredEndpoints
```

## Key Files

| File | Purpose | Lines |
|---|---|---|
| `src/AppManager.Application/IisSites/IisSiteAppService.cs` | IIS CRUD + management | 241 |
| `src/AppManager.Application/WindowsServices/WindowsServiceAppService.cs` | Service CRUD + lifecycle | 120 |
| `src/AppManager.Application/Services/WindowsServiceManager.cs` | sc.exe wrapper | 152 |
| `src/AppManager.Web/AppManagerWebModule.cs` | Web startup, pipeline config | 240 |
| `src/AppManager.Web/Program.cs` | Entry point, Serilog | 56 |
| `src/AppManager.Application.Contracts/Permissions/AppManagerPermissions.cs` | 20 permission constants | 50 |
| `src/AppManager.Domain/Services/IIisManager.cs` | IIS management interface | 29 |