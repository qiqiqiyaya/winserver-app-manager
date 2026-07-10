<!-- Generated: 2026-07-11 | Files scanned: 139 | Token estimate: ~850 -->

# 系统架构

## 概览

IIS 站点与 Windows 服务管理系统，基于 ABP Framework 8.3.4 (.NET 8) 的 ASP.NET Core MVC 应用。

## 分层架构

```
┌─────────────────────────────────────────────────────────┐
│                    AppManager.Web                        │
│  Razor Pages (IIS/Service/Backup/Logs/Permissions)      │
│  LeptonXLite 主题 · Swagger · OpenIddict 认证           │
├─────────────────────────────────────────────────────────┤
│                    AppManager.HttpApi                    │
│  约定 API 控制器 (ABP Auto API)                          │
├─────────────────────────────────────────────────────────┤
│               AppManager.Application                     │
│  IisSiteAppService · WindowsServiceAppService            │
│  SystemLogAppService · AuditLogAppService                │
│  IisManager (IIS) · WindowsServiceManager (sc.exe)       │
├─────────────────────────────────────────────────────────┤
│               AppManager.Domain                          │
│  IisSite · WindowsService · IisSiteBackup               │
│  WindowsServiceBackup (FullAuditedAggregateRoot)         │
├─────────────────────────────────────────────────────────┤
│   AppManager.EntityFrameworkCore                         │
│   SQL Server · EF Core · 自动仓储 · Migrations          │
└─────────────────────────────────────────────────────────┘
```

## 模块依赖链

```
AppManagerDomainSharedModule
  └─ AppManagerDomainModule
       ├─ AppManagerEntityFrameworkCoreModule
       └─ AppManagerApplicationModule
            └─ AppManagerHttpApiModule
                 └─ AppManagerWebModule
```

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
Razor Page → AppService → Domain Service (IIS/WMI/sc.exe)
                            ↓
                         Repository → EF Core → SQL Server
```

## 关键依赖

- ABP Framework 8.3.4 (模块化 MVC)
- Microsoft.Web.Administration (IIS 管理 API)
- System.ServiceProcess.ServiceController (Windows 服务管理)
- Serilog (结构化日志)
- OpenIddict (OAuth2/OpenID Connect)
- AutoMapper (对象映射)
- Swashbuckle (Swagger API)