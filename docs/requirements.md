# IIS 站点与 Windows 服务管理系统 - 需求文档

> **技术栈:** ASP.NET Core MVC + ABP Framework 8.3.4 (.NET 8)
> **创建日期:** 2026-07-09
> **版本:** v2.1
>
> 📐 技术架构详见: [docs/architecture.md](docs/architecture.md)

---

## 一、项目概述

构建基于 ABP Framework 的 Web 管理系统，实现对 IIS 站点和 Windows Service 的统一管理。遵循 ABP 分层架构与 DDD 模式。

### 核心特性

| 特性 | 说明 |
|------|------|
| **模块化架构** | ABP Module 系统，按功能拆分为独立模块 |
| **权限管理** | ABP Permission 系统，权限数据存数据库 |
| **多语言** | ABP Localization，中英文界面 |
| **结构化日志** | Serilog 日志系统，支持文本文件 + SQL Server 双存储 |
| **审计日志** | ABP 自动记录实体变更与操作审计 |
| **后台作业** | ABP BackgroundJob 异步状态轮询 |

---

## 二、基础设施要求

| # | 需求 | 说明 |
|---|------|------|
| 1 | **ABP CLI 创建项目** | 通过 `abp new AppManager --version 8.3.4 --ui mvc` 创建，包含 xUnit / Shouldly / NSubstitute 测试框架 |
| 2 | **SQL Server 数据存储** | 本地开发用 Windows 集成认证；系统表 + 业务表（`IisSites`、`WindowsServices`） |
| 3 | **OIDC 身份认证** | 开发：Cookie + 表单登录（种子账户 `admin / 1q2w3E*`）；生产：SSO 单点登录 |
| 4 | **严格权限控制** | 权限存数据库表（`AbpPermissions` / `AbpPermissionGrants`），共 16 项功能权限 |
| 5 | **Serilog 日志** | 结构化日志，支持文本文件存储（按天滚动）和 SQL Server 数据存储 |

---

## 三、IIS 站点管理

### 3.1 CRUD 操作

| 功能 | 描述 |
|------|------|
| 创建站点 | 新建 IIS 站点，配置名称、物理路径、端口 |
| 查询站点 | 列表展示所有站点及运行状态 |
| 删除站点 | 删除指定站点（含确认） |

### 3.2 站点绑定

- **HTTP 绑定** — IP、端口（默认 80）、主机名（可选）
- **HTTPS 绑定** — IP、端口（默认 443）、主机名（可选）、SSL 证书

### 3.3 站点设置

| 设置项 | 描述 |
|--------|------|
| Edit Permissions | 修改物理路径 NTFS 权限 |
| Add Application | 添加子应用程序（别名、路径、应用程序池） |
| Add Virtual Directory | 添加虚拟目录（别名、路径） |
| 修改站点名称 | 重命名站点 |
| 应用程序池管理 | 管理关联的应用池 |

### 3.4 应用程序池配置

- 基础设置：.NET CLR Version、Managed Pipeline Mode
- 高级设置：Start Mode、Idle Timeout、Maximum Worker Processes、Recycling、Process Model、Rapid-Fail Protection

### 3.5 站点备份与还原

| 功能 | 描述 |
|------|------|
| 备份站点 | 备份 IIS 站点全部配置信息（绑定、应用池、子应用、虚拟目录等）到数据库 |
| 查看备份列表 | 按站点名称、时间筛选查看历史备份记录 |
| 预览备份 | 查看备份中的站点配置详情（JSON 格式） |
| 还原站点 | 选择历史备份进行还原，支持覆盖现有站点或创建新站点 |
| 删除备份 | 手动删除不再需要的备份记录 |
| 还原前快照 | 还原操作前自动创建当前状态快照，支持回滚 |

---

## 四、Windows Service 管理

### 4.1 服务操作

| 操作 | 描述 |
|------|------|
| 创建 | `sc create` 或 Win32 API |
| 删除 | 删除指定服务（含确认） |
| 启动 / 停止 / 重启 | 服务状态控制 |

### 4.2 服务属性

| 属性 | 描述 |
|------|------|
| 服务名称 / 显示名称 / 描述 | 标识与说明 |
| 可执行文件路径 | Binary Path |
| 启动类型 | Automatic / Manual / Disabled / Delayed Start |
| 服务账户 | LocalSystem / LocalService / NetworkService / 自定义 |
| 故障恢复 | 首次/二次/后续失败的恢复操作 |
| 依赖关系 | 依赖的服务列表 |

### 4.3 服务备份与还原

| 功能 | 描述 |
|------|------|
| 备份服务配置 | 备份 Windows 服务全部配置信息（启动类型、账户、故障恢复、依赖关系等）到数据库 |
| 查看备份列表 | 按服务名称、时间筛选查看历史备份记录 |
| 预览备份 | 查看备份中的服务配置详情（JSON 格式） |
| 还原服务配置 | 选择历史备份进行还原，服务已存在时支持覆盖配置 |
| 删除备份 | 手动删除不再需要的备份记录 |

---

## 五、权限模型

```
AppManager
├── AppManager.IisSites
│   ├── .Create / .Edit / .Delete
│   ├── .ManageBinding
│   ├── .ManageAppPool
│   ├── .ManagePermissions
│   ├── .Backup / .Restore
├── AppManager.WindowsServices
│   ├── .Create / .Edit / .Delete
│   ├── .Start / .Stop / .Restart
│   ├── .Backup / .Restore
├── AppManager.SystemLogs
│   └── .View
├── AppManager.AuditLogs
│   └── .View
└── AppManager.PermissionManagement        # 权限管理界面权限
    └── .ManagePermissions                  # 控制菜单可见性
```

---

## 六、页面架构

```
├── 首页 Dashboard
│   ├── IIS 站点概览卡片
│   └── Windows 服务概览卡片
│
├── IIS 站点管理
│   ├── 站点列表（状态/绑定/操作按钮/搜索筛选）
│   ├── 创建站点表单
│   ├── 站点编辑（基本信息/绑定/应用池/子应用/虚拟目录）
│   └── 备份与还原（备份列表/创建备份/预览备份/还原站点）
│
├── Windows 服务管理
│   ├── 服务列表（状态/操作按钮/搜索筛选）
│   ├── 创建服务表单
│   ├── 服务详情（基本信息/启动类型/登录账户/故障恢复）
│   └── 备份与还原（备份列表/创建备份/预览备份/还原服务）
│
├── 权限管理 (PermissionManagement)
│   ├── 角色权限分配（角色列表 + 权限树勾选）
│   └── 用户权限分配（用户列表 + 权限树勾选）
│
├── 系统日志 (Serilog)
│   ├── 日志列表（筛选：级别/时间/关键词 + 分页）
│   ├── 日志详情弹窗（完整属性 + 异常堆栈）
│   └── 导出日志 / 清空日志
│
├── 审计日志 (ABP AuditLogging)
│   ├── 审计列表（筛选：用户/操作类型/时间/结果 + 分页）
│   ├── 审计详情（请求信息 + 实体变更记录）
│   └── 导出审计报告
```

---

## 七、后续规划

- [x] ~~操作审计日志~~ — 已实现 (ABP AuditLogging + Serilog)
- [x] ~~Serilog 结构化日志~~ — 已实现（文本文件 + SQL Server 双存储）
- [x] ~~IIS 站点备份与还原~~ — 已实现（数据库存储 JSON 配置快照）
- [x] ~~Windows 服务备份与还原~~ — 已实现（数据库存储 JSON 配置快照）
- [ ] 批量操作支持
- [ ] 导入/导出配置
- [ ] 远程服务器管理
