# MssBase.Service

ASP.NET Core API host for the MssBase solution, with a Security domain organized into controller, service, logic, data, contract, and DTO projects.

## Overview

This repository is structured so each layer has a clear responsibility:

- `Controllers` expose HTTP endpoints.
- `Service` projects handle orchestration and caching.
- `Logic` projects enforce validation and business rules.
- `Data` projects define persistence models and EF Core mappings.
- `Contract` and `Dto` projects define the interfaces and models shared between layers.
- `Shared` projects provide cross-cutting infrastructure used across domains.

For a fuller breakdown of how the layers fit together, see [docs/PROJECT_ARCHITECTURE.md](docs/PROJECT_ARCHITECTURE.md).

## Documentation

- [Project Architecture](docs/PROJECT_ARCHITECTURE.md)

## Project Layout

Key folders in the solution:

- `MssBase.Service/`: API host, controllers, application startup, configuration, and HTTP surface.
- `Services/Security/`: Security-specific contracts, DTOs, logic, EF Core data model, and service implementations.
- `Services/Common/`: Common domain projects following the same layered pattern.
- `Services/Logger/`: Logging service implementation.
- `Shared/`: shared contracts, models, logic helpers, data helpers, and service utilities.
- `Tests/`: integration and unit tests for service and shared layers.

## Development Notes

The solution uses:

- ASP.NET Core for the API host.
- Entity Framework Core for database access and schema management.
- FluentValidation for request validation.
- Redis-backed caching through shared cache abstractions.
- Layered class library projects to separate API concerns from business logic and persistence.

## Logging

The API uses Serilog as its logging provider and reads its main configuration from `MssBase.Service/appsettings.json`. Application code should use the standard `Microsoft.Extensions.Logging.ILogger<T>` abstraction; Serilog routes those events to the configured sinks.

The logger starts with a bootstrap Console logger so startup failures can be recorded before application configuration is loaded. Once the host is built, `AddSerilog` loads the configured levels, sinks, and enrichers. The base configuration writes to Console, daily rolling compact JSON files under `MssBase.Service/logs/` (up to 100 MB per file, retaining 14 files), and Seq. Development configuration sets the default level to `Debug` and enables more detailed Entity Framework Core command logging.

Configured enrichers add log context, machine name, process and thread IDs, and expanded exception details. Framework log levels are overridden in appsettings to reduce noise while keeping selected hosting and database events visible.

`Program.cs` uses `UseSerilogRequestLogging()` to emit one summary event per HTTP request, including its method, path, status code, and elapsed time. It does not record request or response bodies. Controller exceptions passed to `ApiBaseController.HandleControllerException` are logged at `Error` level with the exception, method, path, and trace ID; the API returns a generic Problem Details response rather than exposing exception details. The controller exception path uses Serilog through `ILoggerFactory`, not the legacy Redis-backed `ILoggerService`. The separate `ILoggerService` registration remains available for other existing code.

For application events, inject `ILogger<T>` and use message templates so properties remain searchable in Seq:

```csharp
public sealed class ApplicationLogic(ILogger<ApplicationLogic> logger)
{
  public void RecordCreated(int applicationId, string applicationName)
  {
    logger.LogInformation(
      "Application {ApplicationId} ({ApplicationName}) created",
      applicationId,
      applicationName);
  }
}
```

Avoid logging passwords, tokens, connection strings, request bodies, or other sensitive values. Prefer logging identifiers and operationally useful context. Existing database audit records remain the source for durable business change history; Serilog is for operational diagnostics.

### Browsing Seq locally

Start the Docker stack, including Seq, from the repository root:

```bash
cd docker && docker compose --env-file .env.dev -p mssbase-dev up -d --build
```

Open `http://localhost:8081` to browse Seq when running on the same machine. In a remote VS Code/dev-container workspace, open or forward port `8081` from the **Ports** panel and use the forwarded URL. Seq accepts log events on port `5341`; the API container is configured to send to `http://seq:5341`, while an API running directly on the host uses `http://localhost:5341`. The Compose configuration disables Seq authentication for local development only; do not use that setting for a shared or production deployment.

## Building the Project

From the solution root, build the API project with:

```bash
dotnet build MssBase.Service/MssBase.Service.csproj
```

## Entity Framework Helpers

These commands target the Security data project and use the API host as the startup project so configuration is loaded correctly.

### Add a migration

Open a terminal in the root of the solution and run:

```bash
dotnet ef migrations add InitialMigration \
  --project Services/Security/Data.Security/Data.Security.csproj \
  --startup-project MssBase.Service/MssBase.Service.csproj \
  --output-dir Migrations
```

Notes:

- `--project` points to the data project where `SecurityDBContext` lives.
- `--startup-project` points to the API project where configuration is loaded.
- `--output-dir` specifies where the migration files are created, relative to the data project.

### Update the database

To apply the latest migrations to the database, run:

```bash
dotnet ef database update \
  --project Services/Security/Data.Security/Data.Security.csproj \
  --startup-project MssBase.Service/MssBase.Service.csproj
```

### Drop the database

Use this only for local or disposable environments.

```bash
dotnet ef database drop \
  --project Services/Security/Data.Security/Data.Security.csproj \
  --startup-project MssBase.Service/MssBase.Service.csproj
```

### Docker Commands

```bash
cd docker && docker compose up -d      # start
cd docker && docker compose down       # stop (data persists in volumes)
cd docker && docker compose down -v    # stop and wipe data
```

## API Urls:

- GET /openapi/v1.json (Open API File)
- /Scalar (Scalar UI)

## Next Reference

If you are onboarding to the codebase, start with [docs/PROJECT_ARCHITECTURE.md](docs/PROJECT_ARCHITECTURE.md) and then review `ServiceExtensions.cs` plus the relevant domain folder under `Services/`.
