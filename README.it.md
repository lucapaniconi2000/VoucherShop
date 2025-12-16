
---

## `README.it.md` (Italiano)

```md
# VoucherShop 🧾🎴

VoucherShop è una Web API in .NET per negozi di carte (es. MTG) per gestire **voucher/store credit** generati dai tornei.

- Un **User** fa login e vede **saldo**, **ultima modifica** e **scadenza** del voucher.
- Un **Admin** (proprietario) può **creare/aggiornare** voucher e consultare lo **storico modifiche (audit)**.

Il progetto supporta la separazione per **Shop/Tenant** (multi-tenant) tramite claim `shop_id` nei JWT. Può essere usato anche come **single-shop** creando un solo shop.

**Lingua:** [English](README.md) | Italiano

---

## Cosa fa il progetto

### Dominio
- Ogni `Shop` ha una `Currency` (es. `EUR`).
- Ogni `User` appartiene a uno `Shop` (`ShopId`).
- Ogni `Voucher` è unico per `(ShopId, UserId)` (1 voucher per utente per shop).
- Il saldo è un value object `Money`. La scadenza è gestita nel dominio.

### Ruoli
- **Admin**
  - crea utenti nel proprio shop
  - crea/aggiorna voucher
  - legge audit history dei voucher
- **User**
  - legge il proprio voucher via `/api/me/voucher`

---

## Features
- ✅ Clean Architecture (Domain / Application / Infrastructure / Api)
- ✅ Multi-tenant via claim `shop_id` nel JWT
- ✅ ASP.NET Identity + JWT + Refresh Token (cookie HttpOnly)
- ✅ CQRS con MediatR (Commands + Queries)
- ✅ EF Core + PostgreSQL (Docker)
- ✅ Swagger/OpenAPI
- ✅ Audit history dei voucher

---

## Stack
- .NET 8 (LTS)
- ASP.NET Core Web API
- EF Core + Npgsql
- PostgreSQL (docker-compose)
- ASP.NET Identity
- JWT + Refresh Token
- MediatR
- Swagger

---

## Struttura Solution
- `VoucherShop.Domain` — Entità + Value Objects (`Voucher`, `Money`, `Shop`, `AuditLog`)
- `VoucherShop.Application` — Use cases (CQRS), Interfacce, DTO
- `VoucherShop.Infrastructure` — DbContext EF, Identity, Repository, JWT services
- `VoucherShop.Api` — Controller, Swagger, pipeline, DI

---

## Prerequisiti
- .NET SDK 8.x
- Docker Desktop
- (Opzionale) pgAdmin / DBeaver / DataGrip

---

## Avvio rapido (Development)

### 1) Avvia Postgres con Docker
Dalla root:

```bash
docker compose up -d
