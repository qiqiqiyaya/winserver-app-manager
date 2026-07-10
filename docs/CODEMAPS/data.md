<!-- Generated: 2026-07-11 | Files scanned: 139 | Token estimate: ~850 -->

# Data Architecture

## Database

- **Provider**: SQL Server (Windows Integrated Auth in dev)
- **Connection**: `Default` connection string
- **Table Prefix**: `App` (via `AppManagerConsts.DbTablePrefix`)
- **Schema**: `dbo` (null = default)

## Business Tables

### AppIisSites
| Column | Type | Notes |
|---|---|---|
| Id | uniqueidentifier | PK, FullAuditedAggregateRoot |
| SiteName | nvarchar(256) | Unique index |
| PhysicalPath | nvarchar(1024) | Required |
| Status | nvarchar(32) | Running/Stopped/Unknown |
| Port | int | Nullable |
| BindingsJson | nvarchar(max) | JSON array |
| AppPoolName | nvarchar(256) | |
| AppPoolConfigJson | nvarchar(max) | JSON object |
| SubApplicationsJson | nvarchar(max) | JSON array |
| VirtualDirectoriesJson | nvarchar(max) | JSON array |
| NtfsPermissionsJson | nvarchar(max) | JSON array |
| *Audit fields* | | FullAudited (CreationTime, CreatorId, LastModificationTime, LastModifierId, IsDeleted, DeleterId, DeletionTime) |

### AppWindowsServices
| Column | Type | Notes |
|---|---|---|
| Id | uniqueidentifier | PK, FullAuditedAggregateRoot |
| ServiceName | nvarchar(256) | |
| DisplayName | nvarchar(256) | |
| Description | nvarchar(max) | |
| ExecutablePath | nvarchar(1024) | |
| StartType | nvarchar(32) | Automatic/Manual/Disabled/DelayedAuto |
| Account | nvarchar(256) | LocalSystem / custom account |
| Password | nvarchar(max) | Encrypted? (plaintext currently) |
| FailureActionsJson | nvarchar(max) | JSON |
| DependenciesJson | nvarchar(max) | JSON |
| Status | nvarchar(32) | Running/Stopped/Unknown |
| *Audit fields* | | FullAudited |

### AppIisSiteBackups
| Column | Type | Notes |
|---|---|---|
| Id | uniqueidentifier | PK, FullAuditedAggregateRoot |
| SiteName | nvarchar(256) | |
| BackupData | nvarchar(max) | JSON (IIS config snapshot) |
| Description | nvarchar(max) | Nullable |
| CreatedAt | datetime2 | |
| CreatedBy | nvarchar(256) | |
| *Audit fields* | | FullAudited |

### AppWindowsServiceBackups
| Column | Type | Notes |
|---|---|---|
| Id | uniqueidentifier | PK, FullAuditedAggregateRoot |
| ServiceName | nvarchar(256) | |
| DisplayName | nvarchar(256) | |
| BackupData | nvarchar(max) | JSON (service config snapshot) |
| Description | nvarchar(max) | Nullable |
| CreatedAt | datetime2 | |
| CreatedBy | nvarchar(256) | |
| *Audit fields* | | FullAudited |

## ABP System Tables (auto-managed)

| Module | Tables |
|---|---|
| Identity | AbpUsers, AbpRoles, AbpRoleClaims, AbpUserClaims, AbpUserLogins, AbpUserTokens, AbpOrganizationUnits, AbpSecurityLogs, AbpLinkUsers, AbpUserDelegations, AbpSessions |
| Permission Management | AbpPermissions, AbpPermissionGrants |
| Audit Logging | AbpAuditLogs, AbpAuditLogActions, AbpEntityChanges, AbpEntityPropertyChanges |
| OpenIddict | AbpOpenIddictApplications, AbpOpenIddictAuthorizations, AbpOpenIddictScopes, AbpOpenIddictTokens |
| Feature Management | AbpFeatures, AbpFeatureValues |
| Setting Management | AbpSettings |
| Tenant Management | AbpTenants, AbpTenantConnectionStrings |
| Background Jobs | AbpBackgroundJobs |

## Serilog Tables

| Table | Purpose |
|---|---|
| SerilogLogs | Structured log entries (auto-created by Serilog) |

## Migration History

| Migration | Date | Status |
|---|---|---|
| `20260709063542_Initial` | 2026-07-09 | Applied (single migration) |

## Entity Relationships

```
IisSite ──1:N──> IisSiteBackup  (by SiteName)
WindowsService ──1:N──> WindowsServiceBackup  (by ServiceName)
```

Backups store full JSON snapshots; no FK constraints — linked by name.