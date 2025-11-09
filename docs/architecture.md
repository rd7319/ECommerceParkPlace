# IplEcommerce — One-Page Architecture

```mermaid
flowchart LR
  Client["Client - Web / Mobile / Postman"] -->|HTTP JSON| API["API - IplEcommerce.API (ASP.NET Core)"]
  API --> Controllers["Controllers"]
  Controllers -->|uses| UnitOfWork["UnitOfWork / IUnitOfWork"]
  UnitOfWork -->|coordinates| Repos["Repositories - GenericRepository, ProductRepository, CartRepository, OrderRepository"]
  Repos -->|EF Core| DbContext["IplEcommerceDbContext - EF Core"]
  DbContext -->|SQL| SQL["SQL Server"]

  Domain["Domain Layer - Entities, Enums, Interfaces"] --- Repos
  Application["Application Layer - DTOs"] --- API
  Infrastructure["Infrastructure Layer - EF, Repos, Migrations, Seeder"] --- DbContext
  API
```
Key components (file pointers):

- Presentation (API): `src/Presentation/IplEcommerce.API`
  - Controllers: `OrdersController.cs`, `ProductsController.cs`, `CartController.cs`
  - Program/startup: `Program.cs`

- Application (DTOs): `src/Core/IplEcommerce.Application`
  - DTO definitions: `ApplicationDtos.cs`

- Domain: `src/Core/IplEcommerce.Domain`
  - Entities, enums and repository interfaces: `Entities/`, `Interfaces/`

- Infrastructure: `src/Infrastructure/IplEcommerce.Infrastructure`
  - EF DbContext: `Data/IplEcommerceDbContext.cs`
  - Repositories: `Repositories/GenericRepository.cs`, `ProductRepository.cs`, `OrderRepository.cs`, `CartRepository.cs`
  - Unit of Work: `UnitOfWork/UnitOfWork.cs`
  - Migrations & Seeder: `Migrations/`, `Data/DatabaseSeeder.cs`

Data flow summary:
- Client -> API controllers -> UnitOfWork -> repository calls -> EF Core (`IplEcommerceDbContext`) -> SQL Server.
- DTOs are used by controllers to return API payloads; domain entities live in Domain layer and are persisted by EF Core in Infrastructure.

Important design points & notes:
- Patterns used: Repository (generic + specific), Unit of Work, DTOs, Dependency Injection, (optionally) Mediator.
- Concurrency: stock decrement is implemented using an atomic SQL UPDATE in `ProductRepository.TryDecrementStockAsync` to avoid oversell.
- Performance considerations: prefer DTO projection (`Select` / `ProjectTo`) instead of materializing full entities; use `.AsNoTracking()` for read-only queries; batch `SaveChanges()` calls.

(End of one?page architecture)