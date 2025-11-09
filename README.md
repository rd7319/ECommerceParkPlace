# IplEcommerce

Lightweight e-commerce sample implemented with ASP.NET Core (.NET 8), EF Core, and a Repository + UnitOfWork architecture.

## Project layout

- `src/Presentation/IplEcommerce.API` — API project (controllers, startup)
- `src/Core/IplEcommerce.Application` — DTOs and application-level types
- `src/Core/IplEcommerce.Domain` — Domain entities, enums, repository interfaces
- `src/Infrastructure/IplEcommerce.Infrastructure` — EF Core `DbContext`, repositories, migrations, seeder

## Quickstart

1. Configure connection string in `src/Presentation/IplEcommerce.API/appsettings.json` under `ConnectionStrings:DefaultConnection`.
2. Apply migrations and seed data (Program.cs runs `MigrateAsync()` and seeder on startup) or run:

   ```bash
   cd src/Presentation/IplEcommerce.API
   dotnet run
   ```

3. Open Swagger UI at `https://localhost:{port}/swagger` (development environment).

## Important endpoints (examples)

- `GET /api/products` — list products
- `GET /api/products/{id}` — single product
- `GET /api/cart/{userId}` & cart management endpoints (see `CartController`)
- `POST /api/orders/create` — create order from cart

## Demo script (3–5 min)

1. Start API: `dotnet run` in `src/Presentation/IplEcommerce.API`.
2. `GET /api/products` to view products.
3. Add items to cart (via `CartController`) for a test user (seeded user exists if seeder ran).
4. `POST /api/orders/create` with `{ "UserId": 1, "ShippingAddress": "Your address" }`.
5. Verify order created and product `StockQuantity` decreased.

## Architecture (short)

See `docs/architecture.md` for a one?page diagram and component map. Main ideas:
- Controllers call `IUnitOfWork` which exposes repositories.
- Repositories use `IplEcommerceDbContext` (EF Core) to query/persist data.
- DTOs separate API contract from domain entities.
---