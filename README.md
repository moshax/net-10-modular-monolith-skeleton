# Modular Monolith Skeleton (.NET 10)

This repository contains a modular monolith backend with strict layering plus an Angular frontend workspace.

## Architecture Overview

- Modular monolith host in `src/App.Host`.
- Business modules in `src/Modules`: `Identity`, `Sales`, `Inventory`.
- Per-module layering: `Api -> Application -> Domain`, with `Infrastructure` implementing contracts.
- Shared SQL Server database with schema-per-module.
- CQRS-style application use cases (commands/queries), repository + unit of work.
- Global exception middleware in `src/BuildingBlocks`.

## API Conventions

- All endpoints are versioned under `/api/v1`.
- Swagger/OpenAPI is enabled in Development at `/swagger`.
- CORS policy `Frontend` allows:
  - `http://localhost:4200`
  - `https://localhost:4200`

Examples:

- `POST /api/v1/identity/users`
- `GET /api/v1/identity/users/{id}`
- `POST /api/v1/sales/orders`
- `GET /api/v1/sales/orders/{id}`
- `POST /api/v1/inventory/products`
- `GET /api/v1/inventory/products/{id}`

## Solution Structure

```text
src/
  App.Host/
  BuildingBlocks/
  Modules/
    Identity/
      Identity.Api/
      Identity.Application/
      Identity.Domain/
      Identity.Infrastructure/
    Sales/
      Sales.Api/
      Sales.Application/
      Sales.Domain/
      Sales.Infrastructure/
    Inventory/
      Inventory.Api/
      Inventory.Application/
      Inventory.Domain/
      Inventory.Infrastructure/
frontend/
  web/ (Angular)
```

## Run Locally

Backend:

```bash
dotnet run --project src/App.Host
```

Default Development URLs from launch settings:

- `https://localhost:61571`
- `http://localhost:61572`

Frontend:

```bash
cd frontend/web
npm start
```

Angular dev server runs on `http://localhost:4200` and proxies `/api/*` to `https://localhost:61571` via `frontend/web/proxy.conf.json`.

## OpenAPI Client Generation (Angular)

The Angular app is configured with `ng-openapi-gen`:

```bash
cd frontend/web
npm run generate:api-client
```

Prerequisite: backend must be running in Development so `https://localhost:61571/swagger/v1/swagger.json` is available.

Generated client output:

- `frontend/web/src/app/api-client`

## EF Core Migrations

All modules use one SQL Server database, each with its own schema and migrations in each module's Infrastructure project.

Identity:

```bash
dotnet ef migrations add Initial --project src/Modules/Identity/Identity.Infrastructure --startup-project src/App.Host --context Identity.Infrastructure.IdentityDbContext
dotnet ef database update --project src/Modules/Identity/Identity.Infrastructure --startup-project src/App.Host --context Identity.Infrastructure.IdentityDbContext
```

Sales:

```bash
dotnet ef migrations add Initial --project src/Modules/Sales/Sales.Infrastructure --startup-project src/App.Host --context Sales.Infrastructure.SalesDbContext
dotnet ef database update --project src/Modules/Sales/Sales.Infrastructure --startup-project src/App.Host --context Sales.Infrastructure.SalesDbContext
```

Inventory:

```bash
dotnet ef migrations add Initial --project src/Modules/Inventory/Inventory.Infrastructure --startup-project src/App.Host --context Inventory.Infrastructure.InventoryDbContext
dotnet ef database update --project src/Modules/Inventory/Inventory.Infrastructure --startup-project src/App.Host --context Inventory.Infrastructure.InventoryDbContext
```
