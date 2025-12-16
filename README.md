# VoucherShop 🧾🎴

VoucherShop is a **.NET 8 (LTS)** Web API to manage **store credit vouchers** for trading card shops (e.g., MTG). Vouchers are typically awarded from tournaments and can be tracked per user, including expiration and history.

- A **User** logs in and views their voucher: **balance**, **currency**, **last update**, and **expiration**.
- An **Admin** (shop owner) can **create users**, **create/update vouchers**, and inspect **audit history**.

The project supports **multi-tenancy** via `ShopId` (tenant = shop), but it can be used as **single-shop** by creating only one shop in production.

**Language:** English | [Italiano](README.it.md)

📚 Documentation: see [docs/00-index.md](docs/00-index.md)

---

## Features
- ✅ Clean Architecture (Domain / Application / Infrastructure / Api)
- ✅ Multi-tenant isolation via `shop_id` claim in JWT
- ✅ ASP.NET Identity + JWT + Refresh Token (HttpOnly cookie)
- ✅ CQRS with MediatR (Commands + Queries)
- ✅ EF Core + PostgreSQL (Docker Compose)
- ✅ Admin: manage vouchers + audit history
- ✅ User: `/api/me/voucher` to view own voucher

---

## Tech stack
- .NET 8 (LTS)
- ASP.NET Core Web API
- EF Core + Npgsql (PostgreSQL provider)
- PostgreSQL (Docker Compose)
- ASP.NET Identity
- JWT + Refresh Token
- MediatR (CQRS)
- Swagger/OpenAPI

---

## Solution structure
- `VoucherShop.Domain` — entities and value objects (e.g., `Voucher`, `Money`, `Shop`)
- `VoucherShop.Application` — CQRS use cases, DTOs, interfaces
- `VoucherShop.Infrastructure` — EF Core DbContext, Identity, repositories, JWT services
- `VoucherShop.Api` — controllers, Swagger, pipeline, DI composition root

---

## Prerequisites
- .NET SDK 8.x
- Docker Desktop
- (Optional) pgAdmin / DBeaver / DataGrip

---

## Quickstart (Development)

### 1) Start PostgreSQL
From repository root:

```bash
docker compose up -d
