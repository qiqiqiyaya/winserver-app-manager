<!-- Generated: 2026-07-12 | Files scanned: 698 | Token estimate: ~850 -->

# 系统架构

## 概览

IIS 站点与 Windows 服务管理系统，基于 ABP Framework 8.3.4 (.NET 8) 的 ASP.NET Core MVC 应用。

## 分层架构

```
┌──────────────────────────────────────────────────────────────────┐
│                       AppManager.Web                              │
│  Razor Pages (IIS/Service/Backup/Logs/Permissions)               │
│  LeptonXLite 主题 · Swagger · OpenIddict 认证                    │
├──────────────────────────────────────────────────────────────────┤
│                       AppManager.HttpApi                         │
│  约定 API 控制器 (ABP Auto API Controllers)                       │
├──────────────────────────────────────────────────────────────────┤
│                  AppManager.Application                           │
│  AppServices: IisSite · WindowsService · Backup · Log · Audit    │
│  (编排层 — 调用领域服务，不包含基础设施代码)                        │
├──────────────────────────────────────────────────────────────────┤
│                    AppManager.Domain                              │
│  Entities: IisSite · WindowsService · IisSiteBackup · ...         │
│  Domain Services (per-domain folders):                           │
│    IisSite/     → IIisManager + IisManager (Microsoft.Web.Admin) │
│    WindowsService/ → IWindowsServiceManager + impl (sc.exe)      │
│    Backups/     → IIisBackupService + IWindowsServiceBackupService│
├──────────────────────────────────────────────────────────────────┤
│              AppManager.EntityFrameworkCore                      │
│  SQL Server · EF Core · 仓储实现 · Migrations                    │
└──────────────────────────────────────────────────────────────────┘
```

## 模块依赖链

```
AppManagerDomainSharedModule
  └─ AppManagerDomainModule (领域服务实现 + 外部 API 接口)
       ├─ AppManagerEntityFrameworkCoreModule
       └─ AppManagerApplicationModule (AppServices 编排 + DTO 映射)
            └─ AppManagerHttpApiModule
                 └─ AppManagerWebModule
```

## 领域服务组织

每个领域文件夹包含：实体 + 接口 + 实现

| 领域文件夹 | 命名空间 | 实体 | 领域服务接口 | 领域服务实现 |
|---|---|---|---|---|
| `Domain/IisSite/` | `AppManager.IisSite` | IisSite | IIisManager | IisManager |
| `Domain/WindowsService/` | `AppManager.WindowsService` | WindowsService | IWindowsServiceManager | WindowsServiceManager |
| `Domain/Backups/` | `AppManager.Backups` | IisSiteBackup, WindowsServiceBackup | IIisBackupService, IWindowsServiceBackupService | IisBackupService, WindowsServiceBackupService |

## 认证 — 双模式

| 环境 | 方案 |
|------|------|
| 开发 | Cookie + 表单登录 (admin/1q2w3E*) |
| 生产 | OpenID Connect SSO |

## 权限模型

20 项功能权限，按 `AppManager.*` 分组：
- **IIS 站点**: Create / Edit / Delete / ManageBinding / ManageAppPool / ManagePermissions / Backup / Restore
- **Windows 服务**: Create / Edit / Delete / Start / Stop / Restart / Backup / Restore
- **系统日志**: View
- **审计日志**: View
- **权限管理**: ManagePermissions

## 数据流

```
Razor Page → AppService (编排) → Domain Service (IIS/WMI/sc.exe 操作)
                                       ↓
                                  Repository → EF Core → SQL Server
```

错误处理: Domain Service 异常 → AppService catch → `BusinessException` → ABP `_AbpErrorFormat` → 客户端 `abp.ajax.showError()`

## 关键依赖

- ABP Framework 8.3.4 (模块化 MVC)
- Microsoft.Web.Administration 7.0.0 (IIS 管理，Domain 项目)
- System.ServiceProcess.ServiceController 8.0.0 (Windows 服务，Domain 项目)
- Serilog (结构化日志)
- OpenIddict (OAuth2/OpenID Connect)
- AutoMapper (对象映射)
- Swashbuckle (Swagger API)