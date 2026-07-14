# AppManager — IIS 站点与 Windows 服务管理系统

> 基于 ABP Framework 8.3.4 的 ASP.NET Core MVC Web 应用，实现对本地 IIS 站点和 Windows Service 的统一管理。

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![ABP](https://img.shields.io/badge/ABP-8.3.4-orange)](https://abp.io/)

## About

This application provides a unified web-based management interface for:

- **IIS Sites** — Create, edit, delete sites; manage bindings (HTTP/HTTPS), application pools, sub-applications, virtual directories, and NTFS permissions
- **Windows Services** — Create, edit, delete services; start/stop/restart; configure startup type, service accounts, failure recovery, and dependencies
- **Backup & Restore** — Full configuration snapshots (JSON) for both IIS sites and Windows Services, with pre-restore rollback support
- **Permission Management** — Role-based and user-based permission assignment via ABP Permission Management
- **Logging & Auditing** — Serilog structured logging (Console + File + SQL Server) + ABP audit logging for full operation traceability
- **Multilingual** — Chinese (zh-Hans, default) and English (en) interface

## Quick Start

### Pre-requirements

* [.NET 8.0+ SDK](https://dotnet.microsoft.com/download/dotnet)
* [Node v18 or 20](https://nodejs.org/en)
* SQL Server (LocalDB for development)
* Windows OS (IIS + Windows Service management APIs)
* Administrator privileges

### Setup

```bash
# 1. Restore dependencies
dotnet restore

# 2. Install client-side libraries
abp install-libs

# 3. Run database migration (creates DB + seeds initial data)
dotnet run --project src/AppManager.DbMigrator

# 4. Start the application (requires admin)
dotnet run --project src/AppManager.Web
```

The web application starts at `https://localhost:44303`.
Default login: **admin** / **1q2w3E\***

### Commands Reference

<!-- AUTO-GENERATED: commands -->
| Command | Description |
|---------|-------------|
| `dotnet restore` | Restore all NuGet dependencies |
| `dotnet build` | Build the entire solution |
| `dotnet run --project src/AppManager.DbMigrator` | Run database migration and seed data |
| `dotnet run --project src/AppManager.Web` | Start the web application (requires admin) |
| `dotnet test` | Run all tests |
| `dotnet test test/AppManager.Domain.Tests` | Run domain layer tests |
| `dotnet test test/AppManager.Application.Tests` | Run application layer tests |
| `dotnet test --filter "FullyQualifiedName=..."` | Run a single test by name |
<!-- END AUTO-GENERATED: commands -->

## Configurations

### Database Connection

Check `ConnectionStrings` in `appsettings.json` under `src/AppManager.Web/` and `src/AppManager.DbMigrator/`:

```json
{
  "ConnectionStrings": {
    "Default": "Server=(localdb)\\MSSQLLocalDB;Database=AppManager;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

For production, use environment variables or a secure configuration provider.

### Authentication

| Environment | Mode | Configuration |
|-------------|------|---------------|
| Development | Cookie + Form Login | Default (no config needed) |
| Production | OpenID Connect SSO | Set `SSO:Authority`, `SSO:ClientId`, `SSO:ClientSecret` |

### OpenIddict Certificate

The application uses an `openiddict.pfx` certificate for signing and encryption. For production:

```bash
dotnet dev-certs https -v -ep openiddict.pfx -p <your-password>
```

Place the certificate in the `src/AppManager.Web/` directory and update the password in the code.

## Solution Structure

```
src/
├── AppManager.Domain/                  # Domain layer: entities, domain services, repository interfaces
├── AppManager.Domain.Shared/           # Domain shared: localization (zh-Hans/en)
├── AppManager.Application/             # Application layer: business logic, AppServices
├── AppManager.Application.Contracts/   # Application contracts: DTOs, interfaces, permission constants
├── AppManager.EntityFrameworkCore/      # EF Core: DbContext, repositories, migrations
├── AppManager.HttpApi/                 # HTTP API: auto conventional controllers
├── AppManager.HttpApi.Client/          # HTTP API client proxies
├── AppManager.DbMigrator/              # Database migration console app
└── AppManager.Web/                     # Web layer: MVC controllers, Razor Pages, Serilog

test/
├── AppManager.TestBase/                # Shared test infrastructure
├── AppManager.Domain.Tests/            # Domain unit tests
├── AppManager.Application.Tests/       # Application layer tests
├── AppManager.EntityFrameworkCore.Tests/ # EF Core integration tests (SQLite)
└── AppManager.Web.Tests/              # Web integration tests
```

**Architecture:** [ABP Layered Architecture](https://abp.io/docs/latest/framework/architecture/domain-driven-design) + DDD

## Documentation

| Document | Description |
|----------|-------------|
| [docs/requirements.md](docs/requirements.md) | Full feature requirements (v2.1) |
| [docs/architecture.md](docs/architecture.md) | Technical architecture details (v1.2) |
| [docs/ui-prototype.md](docs/ui-prototype.md) | UI prototypes (ASCII layouts) |
| [docs/CONTRIBUTING.md](docs/CONTRIBUTING.md) | Development setup & contributing guide |
| [docs/RUNBOOK.md](docs/RUNBOOK.md) | Deployment & operations runbook |
| [docs/ENV.md](docs/ENV.md) | Environment variables & configuration reference |
| [docs/CODEMAPS/](docs/CODEMAPS/) | Auto-generated code maps |

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ABP Framework 8.3.4 (.NET 8) |
| UI | ASP.NET Core MVC + Razor Pages + LeptonXLite Theme |
| ORM | Entity Framework Core + SQL Server |
| DI | Autofac |
| Auth | OpenIddict (OAuth2/OIDC) |
| Logging | Serilog (Console + File + SQL Server) |
| Testing | xUnit + Shouldly + NSubstitute |
| API Docs | Swagger / Swashbuckle |

## Permissions

20 functional permissions across 5 groups:

| Group | Permissions |
|-------|------------|
| IIS Sites | Create, Edit, Delete, ManageBinding, ManageAppPool, ManagePermissions, Backup, Restore |
| Windows Services | Create, Edit, Delete, Start, Stop, Restart, Backup, Restore |
| System Logs | View |
| Audit Logs | View |
| Permission Management | ManagePermissions |

## Additional Resources

* [ABP Framework Documentation](https://abp.io/docs/latest)
* [ABP Web Application Development Tutorial](https://abp.io/docs/latest/tutorials/book-store/part-01)
* [LeptonX Lite MVC UI](https://abp.io/docs/latest/ui-themes/lepton-x-lite/asp-net-core)
* [OpenIddict Documentation](https://documentation.openiddict.com/)
