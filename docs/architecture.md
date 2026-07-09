# IIS 站点与 Windows 服务管理系统 - 技术架构文档

> **技术栈:** ASP.NET Core MVC + ABP Framework 8.3.4 (.NET 8)
> **创建日期:** 2026-07-09
> **版本:** v1.2

---

## 一、ABP 框架分层架构

按 ABP 官方推荐的分层方式组织项目：

```
AppManager/
├── AppManager.sln
├── src/
│   ├── AppManager.Domain/                    # 领域层
│   ├── AppManager.Domain.Shared/             # 领域共享层（本地化资源）
│   ├── AppManager.Application/               # 应用服务层
│   ├── AppManager.Application.Contracts/     # 应用服务契约（接口/DTO/权限常量）
│   ├── AppManager.EntityFrameworkCore/      # EF Core 实现（DbContext/仓储/Migrations）
│   ├── AppManager.DbMigrator/                # 数据库迁移控制台
│   └── AppManager.Web/                       # Web 表现层（MVC UI）
├── test/
│   ├── AppManager.Domain.Tests/              # 领域层测试
│   ├── AppManager.Application.Tests/         # 应用层测试
│   ├── AppManager.Web.Tests/                 # Web 层测试
│   └── AppManager.TestBase/                  # 测试基类库
```

**项目依赖关系：** `Web → Application → Domain`，`Web → EntityFrameworkCore → Domain`

**模块注册：** 每个项目含 `*Module.cs`，继承 `AbpModule`：

```
AppManagerDomainModule → AppManagerEntityFrameworkCoreModule
                       → AppManagerApplicationModule → AppManagerWebModule
```

---

## 二、ABP CLI 项目创建

### 创建项目

```bash
abp new AppManager \
    --template app \
    --ui mvc \
    --database-provider ef \
    --connection-string "Server=.;Database=AppManager;Trusted_Connection=True;TrustServerCertificate=True" \
    --version 8.3.4 \
    --no-random-port
```

| 参数 | 说明 |
|------|------|
| `--template app` | 默认分层模板 |
| `--ui mvc` | ASP.NET Core MVC (Razor) |
| `--database-provider ef` | Entity Framework Core |
| `--version 8.3.4` | ABP 版本 |
| `--no-random-port` | 使用默认端口 |

---

## 三、NuGet 包版本

| 包名 | 版本 | 用途 |
|------|------|------|
| `Volo.Abp.Core` | 8.3.4 | 核心 |
| `Volo.Abp.AspNetCore.Mvc` | 8.3.4 | MVC |
| `Volo.Abp.Autofac` | 8.3.4 | DI |
| `Volo.Abp.EntityFrameworkCore` | 8.3.4 | EF Core |
| `Volo.Abp.EntityFrameworkCore.SqlServer` | 8.3.4 | SQL Server |
| `Volo.Abp.AuditLogging.Domain` | 8.3.4 | 审计日志 |
| `Volo.Abp.BackgroundJobs` | 8.3.4 | 后台作业 |
| `Volo.Abp.Localization` | 8.3.4 | 本地化 |
| `Volo.Abp.SettingManagement.Domain` | 8.3.4 | 设置管理 |
| `Volo.Abp.PermissionManagement.Domain` | 8.3.4 | 权限管理 |
| `Volo.Abp.PermissionManagement.Domain.Identity` | 8.3.4 | 权限-Identity集成 |
| `Volo.Abp.PermissionManagement.Application` | 8.3.4 | 权限应用层 |
| `Volo.Abp.PermissionManagement.Application.Contracts` | 8.3.4 | 权限契约 |
| `Volo.Abp.PermissionManagement.EntityFrameworkCore` | 8.3.4 | 权限EF实现 |
| `Volo.Abp.Identity.Domain` | 8.3.4 | 用户/角色领域 |
| `Volo.Abp.Identity.Application` | 8.3.4 | 用户/角色应用 |
| `Volo.Abp.Identity.Application.Contracts` | 8.3.4 | 用户/角色契约 |
| `Volo.Abp.Identity.EntityFrameworkCore` | 8.3.4 | 用户/角色EF |
| `Volo.Abp.Identity.AspNetCore` | 8.3.4 | Identity认证 |
| `Volo.Abp.AspNetCore.Authentication.OAuth` | 8.3.4 | OAuth/OIDC |
| `Microsoft.AspNetCore.Authentication.OpenIdConnect` | 8.0.x | OIDC中间件 |
| `Microsoft.AspNetCore.Authentication.Cookies` | 8.0.x | Cookie认证 |
| `Serilog.AspNetCore` | 8.0.x | 结构化日志 |
| `Serilog.Sinks.Console` | 5.0.x | 控制台输出 |
| `Serilog.Sinks.File` | 5.0.x | 文本文件存储 |
| `Serilog.Sinks.MSSqlServer` | 6.7.x | SQL Server 存储 |
| `Serilog.Sinks.Async` | 1.5.x | 异步写入（性能优化） |
| `Serilog.Enrichers.Environment` | 2.3.x | 环境信息富化 |
| `Serilog.Enrichers.Process` | 2.0.x | 进程信息富化 |
| `Serilog.Enrichers.Thread` | 3.1.x | 线程信息富化 |

> **.NET 版本要求:** .NET 8.0 SDK，所有项目 `TargetFramework` 为 `net8.0`

---

## 四、数据存储 (SQL Server)

### 连接字符串

```json
{
  "ConnectionStrings": {
    "Default": "Server=.;Database=AppManager;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

| 环境 | 方式 |
|------|------|
| 开发 | 本地 SQL Server，Windows 集成认证 |
| 生产 | 环境变量 / 配置中心注入 |

### EF Core 配置

```csharp
Configure<AbpDbContextOptions>(options => options.UseSqlServer());
```

### 数据库表

**系统表（ABP 自动创建）：** `AbpPermissions`、`AbpPermissionGrants`、`AbpRoles`、`AbpUserRoles`、`AbpUsers`、`AbpSettings`、`AbpAuditLogs`、`AbpAuditLogActions`、`AbpEntityChanges`、`AbpEntityPropertyChanges`、`AbpBackgroundJobs`

**业务表（手动创建）：** `IisSites`、`WindowsServices`、`IisSiteBackups`、`WindowsServiceBackups`

**日志表（Serilog 自动创建）：** `SerilogLogs`

### 迁移

```bash
dotnet run --project src/AppManager.DbMigrator
```

---

## 五、身份认证 (OIDC + 本地登录)

### 双模式策略

| 环境 | 方案 | 依赖 |
|------|------|------|
| **开发** | Cookie 认证 + 表单登录 | 无需外部 SSO |
| **生产** | OpenID Connect SSO | 统一认证平台 |

### 开发环境

简单表单登录，数据库账户验证，种子管理员：`admin / 1q2w3E*`

```csharp
// Cookie 认证，登录页 /Account/Login
context.Services.AddAuthentication(...)
    .AddCookie("Cookies", o => { o.LoginPath = "/Account/Login"; });
```

### 生产环境

```csharp
.AddOpenIdConnect("oidc", o =>
{
    o.Authority = config["SSO:Authority"];
    o.ClientId = config["SSO:ClientId"];
    o.ClientSecret = config["SSO:ClientSecret"];
    o.ResponseType = "code";
});
```

```json
{ "SSO": { "Authority": "https://sso.example.com", "ClientId": "AppManager", ... } }
```

---

## 六、权限控制

### 存储表

| 表 | 说明 |
|----|------|
| `AbpPermissionGroups` | 权限分组 |
| `AbpPermissions` | 权限项 |
| `AbpPermissionGrants` | 授权记录（角色/用户 → 权限） |

### 权限树

```
AppManager
├── AppManager.IisSites              # IIS站点管理
│   ├── .Create                      # 创建站点
│   ├── .Edit                        # 编辑站点
│   ├── .Delete                      # 删除站点
│   ├── .ManageBinding               # 管理绑定
│   ├── .ManageAppPool               # 管理应用程序池
│   ├── .ManagePermissions           # 管理NTFS权限
│   ├── .Backup                      # 备份站点
│   └── .Restore                     # 还原站点
├── AppManager.WindowsServices       # Windows服务管理
│   ├── .Create                      # 创建服务
│   ├── .Edit                        # 编辑服务
│   ├── .Delete                      # 删除服务
│   ├── .Start                       # 启动服务
│   ├── .Stop                        # 停止服务
│   ├── .Restart                     # 重启服务
│   ├── .Backup                      # 备份服务配置
│   └── .Restore                     # 还原服务配置
├── AppManager.SystemLogs            # 系统日志
│   └── .View                        # 查看日志
├── AppManager.AuditLogs             # 审计日志
│   └── .View                        # 查看审计
└── AppManager.PermissionManagement  # 权限管理
    └── .ManagePermissions           # 管理角色/用户权限
```

### 权限注册

```csharp
public class AppManagerPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup("AppManager", L("AppManager"));
        // group.AddPermission(...)
    }
}
```

### 权限校验

```csharp
[Authorize(AppManagerPermissions.IisSites.Create)]
public async Task<IisSiteDto> CreateAsync(CreateIisSiteDto input) { ... }
```

### 种子数据

```csharp
public class AppManagerPermissionDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    public async Task SeedAsync(DataSeedContext context)
    {
        await _permissionDataSeeder.SeedAsync(
            ClientPermissionValueProvider.ProviderName, "admin",
            new[] { AppManagerPermissions.IisSites.Default, ... }, context.TenantId);
    }
}
```

### 权限管理界面

权限管理模块使用 ABP `Volo.Abp.PermissionManagement` 模块自带的权限管理 API，通过自定义 Razor Page 渲染权限分配界面：

| 页面 | Razor Page 路由 | 说明 |
|------|-----------------|------|
| 角色权限分配 | `/PermissionManagement/Roles` | 左侧角色列表 + 右侧权限树，支持按角色勾选权限 |
| 用户权限分配 | `/PermissionManagement/Users` | 左侧用户列表（含搜索筛选）+ 右侧权限树，支持按用户勾选权限 |

> **说明**：ABP PermissionManagement 模块提供后端 API（`IPermissionAppService`），前端 Razor Page 调用 ABP API 实现权限的增删改查。权限数据存储在 `AbpPermissionGrants` 表中。

### 菜单注册

```csharp
// AppManagerMenuContributor.cs
public class AppManagerMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            // ... 其他菜单项

            context.Menu.AddItem(
                new ApplicationMenuItem(
                    "AppManager.PermissionManagement",
                    l["Menu:PermissionManagement"],
                    icon: "fas fa-shield-alt",
                    permission: AppManagerPermissions.PermissionManagement.ManagePermissions
                )
                .AddItem(new ApplicationMenuItem(
                    "AppManager.PermissionManagement.Roles",
                    l["Menu:RolePermissions"],
                    url: "/PermissionManagement/Roles",
                    permission: AppManagerPermissions.PermissionManagement.ManagePermissions
                ))
                .AddItem(new ApplicationMenuItem(
                    "AppManager.PermissionManagement.Users",
                    l["Menu:UserPermissions"],
                    url: "/PermissionManagement/Users",
                    permission: AppManagerPermissions.PermissionManagement.ManagePermissions
                ))
            );
        }
    }
}
```

---

## 八、备份与还原

### IIS 站点备份与还原

**备份内容：**
- 站点配置（`applicationHost.config` 中站点定义）
- 站点绑定信息（IP、端口、主机名、SSL 证书引用）
- 应用程序池配置（.NET CLR 版本、托管管道模式、回收设置等）
- 子应用程序与虚拟目录结构

**还原策略：**
- 可选择覆盖现有站点或创建新站点
- 支持预览备份内容后再还原
- 还原前自动创建还原点（快照），支持回滚

**存储方式：**
- 备份数据存储在数据库表 `IisSiteBackups` 中
- 支持按站点名称、创建时间筛选备份记录
- 允许手动删除过期备份

### Windows 服务备份与还原

**备份内容：**
- 服务配置（服务名、显示名、描述、可执行文件路径）
- 启动类型（Automatic / Manual / Disabled / Delayed Start）
- 服务账户（LocalSystem / LocalService / NetworkService / 自定义账户）
- 故障恢复策略（首次/二次/后续失败操作及延迟时间）
- 依赖关系列表

**还原策略：**
- 服务配置直接还原（不涉及二进制文件）
- 支持预览备份内容后再还原
- 还原时如果服务已存在，可选择覆盖配置

**存储方式：**
- 备份数据存储在数据库表 `WindowsServiceBackups` 中
- 支持按服务名称、创建时间筛选备份记录
- 允许手动删除过期备份

### 备份数据表结构

**IisSiteBackups 表：**

| 列名 | 类型 | 说明 |
|------|------|------|
| `Id` | `uniqueidentifier` (PK) | 主键 |
| `SiteName` | `nvarchar(256)` | 站点名称 |
| `BackupData` | `nvarchar(max)` | JSON 格式的完整站点配置 |
| `Description` | `nvarchar(512)` | 备份说明（可选） |
| `CreatedAt` | `datetime2` | 备份创建时间 |
| `CreatedBy` | `nvarchar(256)` | 备份创建人 |
| `ExtraProperties` | `nvarchar(max)` | ABP 扩展属性 |

**WindowsServiceBackups 表：**

| 列名 | 类型 | 说明 |
|------|------|------|
| `Id` | `uniqueidentifier` (PK) | 主键 |
| `ServiceName` | `nvarchar(256)` | Windows 服务名称 |
| `DisplayName` | `nvarchar(256)` | 服务显示名称 |
| `BackupData` | `nvarchar(max)` | JSON 格式的完整服务配置 |
| `Description` | `nvarchar(512)` | 备份说明（可选） |
| `CreatedAt` | `datetime2` | 备份创建时间 |
| `CreatedBy` | `nvarchar(256)` | 备份创建人 |
| `ExtraProperties` | `nvarchar(max)` | ABP 扩展属性 |

### 技术实现

**IIS 备份实现：**
- 使用 `Microsoft.Web.Administration.ServerManager` 读取站点配置
- 序列化为 JSON 存入数据库
- 还原时反序列化 JSON 并通过 `ServerManager` 重新创建站点

**Windows 服务备份实现：**
- 使用 `System.ServiceProcess.ServiceController` + WMI 读取服务配置
- 序列化为 JSON 存入数据库
- 还原时通过 `sc.exe` 或 Win32 API 重新创建服务配置

---

## 九、测试框架

ABP CLI 自动包含：

| 组件 | 用途 |
|------|------|
| **xUnit** | 单元测试框架 |
| **Shouldly** | 流式断言 |
| **NSubstitute** | Mock 框架 |
| **Abp.TestBase** | ABP 集成测试基类 |

| 测试项目 | 范围 |
|----------|------|
| `TestBase` | 共享基类、测试构建器 |
| `Domain.Tests` | 实体、领域服务、仓储 |
| `Application.Tests` | AppService、DTO映射、权限校验 |
| `Web.Tests` | Controller、Razor Page |

---

## 十、核心 ABP 特性

| 特性 | 说明 |
|------|------|
| **模块化** | 基于 AbpModule 拆分为独立模块 |
| **DI** | Autofac，`ITransientDependency` / `ISingletonDependency` 自动注册 |
| **权限管理** | Permission 系统，数据库存储 |
| **本地化** | 中文(zh-Hans) / 英文(en)，`Domain.Shared/Localization/` |
| **审计日志** | 自动记录实体变更（`IAuditingStore`） |
| **异常处理** | 统一异常处理与友好错误页 |
| **后台作业** | `IBackgroundJobManager` 异步状态轮询 |
| **Setting** | 应用配置项管理 |

### 后台作业示例

```csharp
public class ServiceStatusPollingJob : AsyncBackgroundJob<ServiceStatusPollingArgs>, ITransientDependency
{
    public override async Task ExecuteAsync(ServiceStatusPollingArgs args) { ... }
}
```

### 本地化资源

- 默认语言：中文（zh-Hans），支持英文（en）
- 位置：`Domain.Shared/Localization/AppManager/{zh-Hans,en}.json`

---

## 十一、技术实现要点

### IIS 管理
- 使用 `Microsoft.Web.Administration` (ServerManager)
- 需管理员权限

### Windows 服务管理
- 使用 `System.ServiceProcess.ServiceController`
- 创建/删除用 `sc.exe` 或 WMI
- 需管理员权限

### 安全
- 应用需以管理员权限运行
- 敏感操作需二次确认
- 操作日志记录

### Serilog 日志

使用 Serilog 作为结构化日志框架，替代默认的 Microsoft.Extensions.Logging。

```csharp
// Program.cs - Serilog 配置
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithProcessId()
    .Enrich.WithThreadId()
    .WriteTo.Async(a => a.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"))
    .WriteTo.Async(a => a.File(
        path: "Logs/app-manager-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"))
    .WriteTo.Async(a => a.MSSqlServer(
        connectionString: configuration.GetConnectionString("Default"),
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "SerilogLogs",
            AutoCreateSqlTable = true,
            BatchPostingLimit = 50,
            BatchPeriod = TimeSpan.FromSeconds(5)
        },
        columnOptions: new ColumnOptions
        {
            TimeStamp = { ColumnName = "Timestamp", ConvertToUtc = true },
            AdditionalColumns = new List<SqlColumn>
            {
                new() { ColumnName = "SourceContext", DataType = SqlDbType.NVarChar, DataLength = 512 },
                new() { ColumnName = "RequestPath", DataType = SqlDbType.NVarChar, DataLength = 1024 },
                new() { ColumnName = "UserName", DataType = SqlDbType.NVarChar, DataLength = 256 }
            }
        }))
    .CreateLogger();

builder.Host.UseSerilog();
```

#### 存储方式

| 存储方式 | Sink | 说明 |
|---------|------|------|
| **文本文件** | `Serilog.Sinks.File` | 按天滚动，保留30天，路径 `Logs/` 目录 |
| **SQL Server** | `Serilog.Sinks.MSSqlServer` | 自动建表 `SerilogLogs`，批量写入（50条/5秒） |
| **控制台** | `Serilog.Sinks.Console` | 开发环境彩色输出 |

#### 日志表结构 (SQL Server)

| 列名 | 类型 | 说明 |
|------|------|------|
| `Id` | `int` (PK, IDENTITY) | 自增主键 |
| `Message` | `nvarchar(max)` | 日志消息 |
| `MessageTemplate` | `nvarchar(max)` | 消息模板 |
| `Level` | `nvarchar(128)` | 日志级别 |
| `TimeStamp` | `datetime2` | 时间戳 (UTC) |
| `Exception` | `nvarchar(max)` | 异常详情 |
| `Properties` | `nvarchar(max)` | JSON 属性 |
| `SourceContext` | `nvarchar(512)` | 日志来源类名 |
| `RequestPath` | `nvarchar(1024)` | HTTP 请求路径 |
| `UserName` | `nvarchar(256)` | 当前用户 |

#### 日志级别

| 级别 | 用途 | 环境 |
|------|------|------|
| `Verbose` | 详细追踪 | 仅在本地调试 |
| `Debug` | 调试信息 | 开发环境 |
| `Information` | 正常运行时信息 | 所有环境 |
| `Warning` | 警告信息 | 所有环境 |
| `Error` | 错误信息 | 所有环境 |
| `Fatal` | 致命错误 | 所有环境 |

#### 审计日志

使用 ABP 内置审计日志模块 (`Volo.Abp.AuditLogging`) 记录业务操作审计信息：

```csharp
// ABP 自动记录审计信息
Configure<AbpAuditingOptions>(options =>
{
    options.IsEnabled = true;                        // 启用审计日志
    options.IsEnabledForGetRequests = false;         // GET 请求不记录
    options.IsEnabledForAnonymousUsers = false;      // 匿名用户不记录
    options.ApplicationName = "AppManager";
});
```

| 审计表 | 说明 |
|--------|------|
| `AbpAuditLogs` | 审计日志主体（请求时间、用户、IP、浏览器等） |
| `AbpAuditLogActions` | 审计操作详情（Controller/Action、参数、执行时间） |
| `AbpEntityChanges` | 实体变更记录（新增/修改/删除的字段值） |
| `AbpEntityPropertyChanges` | 实体属性变更详情（旧值→新值） |

#### 日志需求层级

```
┌─────────────────────────────────────────────────────┐
│                    应用日志层                        │
│   Serilog 结构化日志                                  │
│   ├── 控制台 (Console Sink) — 开发                     │
│   ├── 文本文件 (File Sink) — 按天滚动                   │
│   └── SQL Server (MSSqlServer Sink) — 持久化查询        │
├─────────────────────────────────────────────────────┤
│                    审计日志层                        │
│   ABP AuditLogging 审计模块                          │
│   ├── HTTP 请求追踪                                   │
│   ├── 实体变更追踪 (ORM)                               │
│   └── 用户操作行为记录                                  │
└─────────────────────────────────────────────────────┘
```
