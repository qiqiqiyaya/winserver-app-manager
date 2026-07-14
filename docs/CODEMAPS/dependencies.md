<!-- Generated: 2026-07-14 | Files scanned: ~700 | Token estimate: ~600 -->

# Dependencies

## NuGet Packages (Key)

| Package | Purpose | Project |
|---|---|---|
| Volo.Abp.* 8.3.4 | ABP Framework modules | All |
| Microsoft.Web.Administration 7.0.0 | IIS site/pool management | **Domain** |
| System.ServiceProcess.ServiceController 8.0.0 | Windows service management | **Domain** |
| System.Management 8.0.0 | WMI queries (unused, reserved) | Application |
| Serilog.* | Structured logging | Web, DbMigrator |
| AutoMapper | Object mapping | Application, Web |
| Swashbuckle.AspNetCore | Swagger/OpenAPI | Web |
| OpenIddict | OAuth2/OpenID Connect | Web |

## External Integrations

| System | Interface | Technology |
|---|---|---|
| IIS Manager | `IIisManager` (Domain/IisSite/) | Microsoft.Web.Administration.ServerManager |
| Windows Service Control | `IWindowsServiceManager` (Domain/WindowsService/) | ServiceController + sc.exe process |
| NTFS Permissions | `IIisManager` (NTFS methods) | System.Security.AccessControl |
| SQL Server | EF Core DbContext | Microsoft.Data.SqlClient |

## External Services

| Service | Type | Purpose |
|---|---|---|
| SQL Server | Database | Primary data store |
| OpenID Connect Provider | Auth (prod) | Production SSO |

## Build Dependencies

| Tool | Purpose |
|---|---|
| .NET 8 SDK | Build & run |
| xUnit + Shouldly + NSubstitute | Testing |
| Autofac | DI container (ABP default) |

## Architecture Constraints

- **Windows-only**: IIS + ServiceController APIs are Windows-specific
- **Admin rights**: Required for IIS and service management operations
- **No Redis/No caching layer**: All reads go directly to SQL Server
- **No message queue**: Background jobs run in-process (ABP Background Jobs)
- **No CDN/static hosting**: Static files served directly by ASP.NET Core
- **Domain services**: Interfaces + implementations both in Domain project (per-domain folder organization)