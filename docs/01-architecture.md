# Architecture

VoucherShop follows **Clean Architecture** and **CQRS** to keep business rules independent from frameworks and infrastructure.

## Projects
- **VoucherShop.Domain**
  - Entities and Value Objects.
  - Business rules (e.g., voucher expiration).
- **VoucherShop.Application**
  - CQRS: Commands/Queries and handlers (MediatR).
  - DTOs, projections, application interfaces.
- **VoucherShop.Infrastructure**
  - EF Core + PostgreSQL implementation.
  - ASP.NET Identity (AppUser, roles), JWT services, repositories.
- **VoucherShop.Api**
  - Controllers, Swagger, middleware, dependency injection wiring.

## Multi-tenant model (Shop/Tenant)
The model supports multiple shops using `ShopId`:
- `AppUser` has a required `ShopId`.
- `Voucher` is unique per `(ShopId, UserId)`.
- JWT includes `shop_id` claim.
- Admin endpoints operate only within the authenticated admin’s shop.

### Running as single-shop
You can run the system for one shop by creating a single `Shop` and disabling `register-shop` in production.

## CQRS guidelines used
- **Queries** are read-only and optimized for projections (DTOs).
- **Commands** modify state through domain methods and repositories.
- Controllers remain thin: they forward requests to MediatR.

## Key abstractions (Application layer)
- `ICurrentUser` → provides `UserId` and `ShopId` from JWT claims.
- `IVoucherRepository` → write-side operations.
- `IVoucherReadContext` → read-side EF access (exposes `IQueryable<Voucher>`).
- `IAuditRepository / IAuditReadRepository` → write/read audit history.
- `IShopRepository` → shop lookups.

## Domain concepts
- `Money` value object: amount + currency (non-negative, currency normalized).
- `Voucher` entity: has `Balance: Money`, `CreatedAt/UpdatedAt/ExpiresAt`, expires after 3 months.
- `Shop` entity: has default `Currency`.
- `AuditLog`: stores changes as JSON for history.
