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

## Next Reference

If you are onboarding to the codebase, start with [docs/PROJECT_ARCHITECTURE.md](docs/PROJECT_ARCHITECTURE.md) and then review `ServiceExtensions.cs` plus the relevant domain folder under `Services/`.
