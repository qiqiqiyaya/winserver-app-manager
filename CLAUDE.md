# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**IIS 站点与 Windows 服务管理系统** — 基于 ABP Framework 8.3.4 的 ASP.NET Core MVC Web 应用，实现对本地 IIS 站点和 Windows Service 的统一管理。

- **技术栈:** .NET 8 + ASP.NET Core MVC + ABP Framework 8.3.4 + EF Core + SQL Server
- **语言:** C# 为主，中英文多语言界面（默认中文）
- **关键依赖:** `Microsoft.Web.Administration`, `System.ServiceProcess.ServiceController`, Serilog, xUnit/Shouldly/NSubstitute

## Build & Run Commands

```bash
# 数据库迁移
dotnet run --project src/AppManager.DbMigrator

# 运行 Web 应用（需管理员权限，用于管理 IIS/Windows Service）
dotnet run --project src/AppManager.Web

# 运行全部测试
dotnet test

# 运行特定测试项目
dotnet test test/AppManager.Domain.Tests
dotnet test test/AppManager.Application.Tests
dotnet test test/AppManager.Web.Tests

# 运行单个测试
dotnet test --filter "FullyQualifiedName=Namespace.Class.Method"
```

## Project Architecture

项目遵循 ABP 分层架构 + DDD 模式，按以下层级组织：

```
src/
├── AppManager.Domain/                  # 领域层：实体、领域服务、仓储接口
├── AppManager.Domain.Shared/           # 领域共享层：本地化资源 (zh-Hans/en)
├── AppManager.Application/             # 应用服务层：业务逻辑实现
├── AppManager.Application.Contracts/   # 应用契约层：DTO、AppService 接口、权限常量
├── AppManager.EntityFrameworkCore/      # EF Core 实现：DbContext、仓储、Migrations
├── AppManager.DbMigrator/              # 数据库迁移控制台程序
└── AppManager.Web/                     # Web 层：MVC Controller、Razor Pages

test/
├── AppManager.TestBase/                # 测试基类库（ABP 集成测试基础设施）
├── AppManager.Domain.Tests/            # 领域层单元测试
├── AppManager.Application.Tests/       # 应用层测试
└── AppManager.Web.Tests/               # Web 层测试
```

**依赖方向:** Web → Application → Domain，Web → EntityFrameworkCore → Domain  
**模块注册:** 每个项目含 `*Module.cs`，继承 `AbpModule`  
**DI:** Autofac，`ITransientDependency` / `ISingletonDependency` 接口自动注册

## Key Design Decisions

### 身份认证 — 双模式策略

| 环境 | 方案 |
|------|------|
| 开发 | Cookie 认证 + 表单登录，种子账户 `admin / 1q2w3E*` |
| 生产 | OpenID Connect SSO（通过 `appsettings.json` 中 `SSO:*` 配置） |

### 权限模型 — 20 项功能权限，数据库存储

```
AppManager.*
├── IisSites.Create / .Edit / .Delete / .ManageBinding / .ManageAppPool / .ManagePermissions / .Backup / .Restore
├── WindowsServices.Create / .Edit / .Delete / .Start / .Stop / .Restart / .Backup / .Restore
├── SystemLogs.View
├── AuditLogs.View
└── PermissionManagement.ManagePermissions
```

- 权限常量定义在 `Application.Contracts` 项目
- 权限注册在 `AppManagerPermissionDefinitionProvider`
- 种子数据通过 `AppManagerPermissionDataSeedContributor` 初始化
- 权限校验使用 `[Authorize(AppManagerPermissions.IisSites.Create)]` 方式

### 数据存储

- **业务表:** `IisSites`, `WindowsServices`, `IisSiteBackups`, `WindowsServiceBackups`
- **系统表:** ABP 自动管理（`AbpPermissions`, `AbpPermissionGrants`, `AbpAuditLogs`, `AbpEntityChanges` 等）
- **日志表:** `SerilogLogs`（Serilog 自动创建）
- 备份数据以 JSON 格式存储在 `BackupData (nvarchar(max))` 字段

### 日志 — 双层层级

1. **Serilog 结构化日志** — 控制台 + 文本文件（按天滚动，保留30天）+ SQL Server
2. **ABP 审计日志** — HTTP 请求追踪 + 实体变更记录（`AbpAuditLogs` + `AbpEntityChanges`）

### IIS 站点备份与还原

- 使用 `Microsoft.Web.Administration.ServerManager` 读取/写入 IIS 配置
- 序列化为 JSON 存入 `IisSiteBackups` 表
- 还原支持：覆盖现有站点 或 创建新站点
- 还原前自动创建还原点快照

### Windows 服务备份与还原

- 使用 `System.ServiceProcess.ServiceController` + WMI 读取服务配置
- 序列化为 JSON 存入 `WindowsServiceBackups` 表
- 还原时通过 `sc.exe` 或 Win32 API 重新创建服务配置

## Important Constraints

- **应用需以管理员权限运行**（管理 IIS 和 Windows Service 需要）
- **必须在 Windows 环境下运行**（IIS/Windows Service 是 Windows 特有功能）
- SQL Server 作为唯一数据库，开发环境使用 Windows 集成认证
- 所有数据库迁移通过 `AppManager.DbMigrator` 项目执行
- 遵循 [ABP 模块化开发规范](https://docs.abp.io/zh-Hans/abp/8.3)
- 权限管理界面使用 ABP PermissionManagement 模块自带的后端 API（`IPermissionAppService`），自定义 Razor Page 前端

## Reference Documents

- `docs/requirements.md` — 完整需求文档
- `docs/architecture.md` — 技术架构详情
- `docs/ui-prototype.md` — UI 原型图（所有页面的 ASCII 布局）
- `docs/ENV.md` — 环境配置说明
- `docs/CONTRIBUTING.md` — 贡献指南
- `docs/RUNBOOK.md` — 运维手册
- `docs/rules/modal-page-creation-checklist.md` — ABP 模态框创建规范（Layout=null、ModelState、ModalManager）
- `docs/rules/database-migration-command.md` — 数据库迁移命令触发词
- `docs/CODEMAPS/architecture.md` — 系统架构 codemap
- `docs/CODEMAPS/backend.md` — 后端结构 codemap
- `docs/CODEMAPS/frontend.md` — 前端结构 codemap
- `docs/CODEMAPS/data.md` — 数据层 codemap
- `docs/CODEMAPS/dependencies.md` — 依赖关系 codemap
