# Modular Monolith Skeleton (.NET 10)

This repository contains a production-ready modular monolith skeleton with strict layering, EF Core code-first, and module boundaries.


Here’s a senior-architect view of the solution based on the code in this repo.

**Architecture overview**

-   **Modular monolith**: One executable host ([Program.cs](/# "src/App.Host/Program.cs")) composes three business modules (Identity, Sales, Inventory) living in the same process and solution, but separated by folder and project boundaries (src/Modules/*).
-   **Strict layering inside each module**: Each module is split into  Api,  Application,  Domain, and  Infrastructure  projects, enforcing dependencies inwards (API → Application → Domain; Infrastructure implements outward dependencies). See  [*.*.csproj](/# "src/Modules/*/*.*.csproj").
-   **Host as composition root**:  [App.Host](/# "App.Host")  wires modules into the DI container and maps endpoints via extension methods ([Program.cs](/# "src/App.Host/Program.cs")).
-   **Shared DB with per‑module schemas**: All modules use the same SQL Server connection, but each DbContext sets its own schema and owns its migrations ([*DbContext.cs](/# "src/Modules/*/*.Infrastructure/*DbContext.cs"), migrations in  src/Modules/*/*.Infrastructure/Migrations).
-   **Module‑level boundary with explicit contracts**: Repositories and UoW are defined in  Application  contracts and implemented in  Infrastructure. Cross‑module read access uses an explicit interface (IIdentityReadService) implemented by Identity infrastructure and consumed by Sales application ([Contracts.cs](/# "src/Modules/Sales/Sales.Application/Contracts.cs"),  [IdentityReadService.cs](/# "src/Modules/Identity/Identity.Infrastructure/IdentityReadService.cs")).

**Techniques & patterns used**

-   **Clean Architecture / Onion layering**: Domain models are isolated ([*.cs](/# "src/Modules/*/*.Domain/*.cs")) and contain no infrastructure concerns.
-   **Modular composition via DI**: Each module exposes a registration extension ([*ModuleRegistration.cs](/# "*ModuleRegistration.cs")) to keep composition centralized and explicit ([*ModuleRegistration.cs](/# "src/Modules/*/*.Infrastructure/*ModuleRegistration.cs")).
-   **Minimal APIs**: Lightweight endpoint definitions with route groups and handler injection ([*Endpoints.cs](/# "src/Modules/*/*.Api/*Endpoints.cs")).
-   **CQRS‑style use cases**: Application layer defines command/query use cases rather than controllers/services ([UseCases.cs](/# "src/Modules/*/*.Application/UseCases.cs")).
-   **Repository + Unit of Work**: Persistence contracts in Application with EF Core implementations in Infrastructure ([Contracts.cs](/# "Contracts.cs")  +  [*Repository.cs](/# "*Repository.cs")  +  *UnitOfWork  classes).
-   **EF Core code‑first**: Entity configurations via  IEntityTypeConfiguration<T>  and migrations per module ([*Configuration.cs](/# "*Configuration.cs"),  Migrations/*).
-   **Schema‑per‑module**:  HasDefaultSchema("identity"|"sales"|"inventory")  keeps module data separated in a single DB ([*DbContext.cs](/# "*DbContext.cs")).
-   **Validation**: FluentValidation is used for commands in the Application layer (Create*CommandValidator  in  [UseCases.cs](/# "UseCases.cs")).
-   **Result pattern**:  Result<T>  and  Error  types unify success/failure handling in use cases ([Result.cs](/# "src/BuildingBlocks/Result.cs"),  [Error.cs](/# "Error.cs")).
-   **Global exception handling**: Shared middleware in BuildingBlocks ([ExceptionHandlingMiddleware.cs](/# "src/BuildingBlocks/ExceptionHandlingMiddleware.cs")).
-   **DTOs/records**: Application layer exposes explicit DTOs via records ([Contracts.cs](/# "Contracts.cs")  files).
-   **Shared BuildingBlocks**: Small cross‑cutting abstractions (Entity,  Result, middleware) under  src/BuildingBlocks.

If you want, I can also diagram the dependency graph or review the module boundaries for potential coupling (e.g., the Sales → Identity read service flow).

## Solution Structure

```
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
```

## EF Core Migrations

All modules use the same SQL Server database, each with its own schema and migrations in the Infrastructure project.

### Identity

```
dotnet ef migrations add Initial --project src/Modules/Identity/Identity.Infrastructure --startup-project src/App.Host --context Identity.Infrastructure.IdentityDbContext
dotnet ef database update --project src/Modules/Identity/Identity.Infrastructure --startup-project src/App.Host --context Identity.Infrastructure.IdentityDbContext
```

### Sales

```
dotnet ef migrations add Initial --project src/Modules/Sales/Sales.Infrastructure --startup-project src/App.Host --context Sales.Infrastructure.SalesDbContext
dotnet ef database update --project src/Modules/Sales/Sales.Infrastructure --startup-project src/App.Host --context Sales.Infrastructure.SalesDbContext
```

### Inventory

```
dotnet ef migrations add Initial --project src/Modules/Inventory/Inventory.Infrastructure --startup-project src/App.Host --context Inventory.Infrastructure.InventoryDbContext
dotnet ef database update --project src/Modules/Inventory/Inventory.Infrastructure --startup-project src/App.Host --context Inventory.Infrastructure.InventoryDbContext
```
