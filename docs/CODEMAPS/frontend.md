<!-- Generated: 2026-07-11 | Files scanned: 139 | Token estimate: ~800 -->

# Frontend Architecture

## Page Tree

```
/ (Index) — 首页
├── /IisSites — IIS 站点列表 + CRUD
│   ├── /IisSites/CreateModal — 创建站点
│   ├── /IisSites/EditModal — 编辑站点
│   └── /IisSites/Manage — 站点管理（绑定/应用池/子应用/虚拟目录/NTFS）
├── /WindowsServices — Windows 服务列表 + CRUD
│   ├── /WindowsServices/CreateModal — 创建服务
│   └── /WindowsServices/EditModal — 编辑服务
├── /Backups/IisSiteBackups — IIS 备份管理
├── /Backups/WindowsServiceBackups — Windows 服务备份管理
├── /SystemLogs — 系统日志查看
├── /AuditLogs — 审计日志查看
└── /Permissions — 角色权限管理
```

## Page Model Mapping

| Razor Page | PageModel | Depends On |
|---|---|---|
| `/Pages/Index` | `IndexModel` | — |
| `/Pages/IisSites/Index` | `IndexModel` | `IIisSiteAppService` |
| `/Pages/IisSites/CreateModal` | `CreateModalModel` | `IIisSiteAppService` |
| `/Pages/IisSites/EditModal` | `EditModalModel` | `IIisSiteAppService` |
| `/Pages/IisSites/Manage` | `ManageModel` | `IIisSiteAppService` |
| `/Pages/WindowsServices/Index` | `IndexModel` | `IWindowsServiceAppService` |
| `/Pages/WindowsServices/CreateModal` | `CreateModalModel` | `IWindowsServiceAppService` |
| `/Pages/WindowsServices/EditModal` | `EditModalModel` | `IWindowsServiceAppService` |
| `/Pages/Backups/IisSiteBackups/Index` | `IndexModel` | `IIisSiteBackupAppService` |
| `/Pages/Backups/WindowsServiceBackups/Index` | `IndexModel` | `IWindowsServiceBackupAppService` |
| `/Pages/SystemLogs/Index` | `IndexModel` | `ISystemLogAppService` |
| `/Pages/AuditLogs/Index` | `IndexModel` | `IAuditLogAppService` |
| `/Pages/Permissions/Index` | `IndexModel` | `IPermissionAppService` (ABP) |

## JS Modules (per-page)

```
Pages/Index.js
Pages/IisSites/Index.js
Pages/WindowsServices/Index.js
Pages/Backups/IisSiteBackups/Index.js
Pages/Backups/WindowsServiceBackups/Index.js
Pages/SystemLogs/Index.js
Pages/AuditLogs/Index.js
Pages/Permissions/Index.js
```

## UI Framework

- **Theme**: ABP LeptonXLite (Bootstrap-based)
- **Rendering**: Server-side Razor Pages (no SPA)
- **AJAX**: jQuery + ABP dynamic JavaScript proxies
- **Auth UI**: ABP Account module (login/register)

## Navigation

`AppManagerMenus` defines menu constants; `AppManagerMenuContributor` registers entries for:
- Home
- IIS Sites
- Windows Services
- Backups (IIS + Service)
- System Logs
- Audit Logs
- Permissions