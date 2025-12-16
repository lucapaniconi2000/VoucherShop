# VoucherShop 🧾🎴

Gestione voucher per negozi di carte (es. MTG): ogni utente autenticato visualizza il proprio voucher (saldo + scadenza), mentre l’Admin può aggiornare il voucher degli utenti e consultare lo storico modifiche (audit).

## Features
- ✅ Multi-tenant (Shop/Tenant) con `ShopId` nei JWT (`shop_id`)
- ✅ Auth con ASP.NET Identity + JWT + Refresh Token (cookie HttpOnly)
- ✅ CQRS con MediatR (Query/Command)
- ✅ EF Core + PostgreSQL (Docker)
- ✅ Admin: update voucher + audit history
- ✅ User: pagina `/me/voucher` per vedere saldo/scadenza

---

## Tech Stack
- **.NET 8 (LTS)**
- **ASP.NET Core Web API**
- **EF Core + Npgsql**
- **PostgreSQL (Docker Compose)**
- **ASP.NET Identity**
- **JWT + Refresh Token**
- **MediatR (CQRS)**
- **Swagger/OpenAPI**

---

## Struttura Solution (Clean Architecture)
- `VoucherShop.Domain` → Entities + Value Objects (es. `Voucher`, `Money`, `Shop`)
- `VoucherShop.Application` → Use cases (Commands/Queries), Interfaces
- `VoucherShop.Infrastructure` → EF Core, Identity, Repositories, Auth services
- `VoucherShop.Api` → Controllers, Swagger, Pipeline, wiring DI

---

## Prerequisiti
- .NET SDK 8.x
- Docker Desktop (per PostgreSQL)
- (Opzionale) pgAdmin o DBeaver per ispezionare il DB

---

## Setup rapido (Dev)

### 1) Avvia Postgres con Docker
Dalla root:
```bash
docker compose up -d
