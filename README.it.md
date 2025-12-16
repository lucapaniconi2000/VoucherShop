
---

## README.it.md (Italiano)

```md
# VoucherShop 🧾🎴

VoucherShop è una Web API in **.NET 8 (LTS)** per gestire **voucher/store credit** per negozi di carte (es. MTG). I voucher sono tipicamente assegnati tramite tornei e vengono tracciati per utente, con scadenza e storico modifiche.

- Un **User** effettua login e visualizza il proprio voucher: **saldo**, **valuta**, **ultima modifica** e **scadenza**.
- Un **Admin** (proprietario) può **creare utenti**, **creare/aggiornare voucher** e consultare lo **storico (audit)**.

Il progetto supporta il **multi-tenant** tramite `ShopId` (tenant = negozio), ma può essere usato anche come **single-shop** creando un solo shop in produzione.

**Lingua:** [English](README.md) | Italiano

📚 Documentazione: vedi [docs/00-index.md](docs/00-index.md)

---

## Funzionalità
- ✅ Clean Architecture (Domain / Application / Infrastructure / Api)
- ✅ Isolamento multi-tenant via claim `shop_id` nel JWT
- ✅ ASP.NET Identity + JWT + Refresh Token (cookie HttpOnly)
- ✅ CQRS con MediatR (Commands + Queries)
- ✅ EF Core + PostgreSQL (Docker Compose)
- ✅ Admin: gestione voucher + audit history
- ✅ User: `/api/me/voucher` per vedere il proprio voucher

---

## Tech stack
- .NET 8 (LTS)
- ASP.NET Core Web API
- EF Core + Npgsql (provider PostgreSQL)
- PostgreSQL (Docker Compose)
- ASP.NET Identity
- JWT + Refresh Token
- MediatR (CQRS)
- Swagger/OpenAPI

---

## Struttura solution
- `VoucherShop.Domain` — entità e value objects (es. `Voucher`, `Money`, `Shop`)
- `VoucherShop.Application` — casi d’uso CQRS, DTO, interfacce
- `VoucherShop.Infrastructure` — DbContext EF Core, Identity, repository, servizi JWT
- `VoucherShop.Api` — controller, Swagger, pipeline, DI composition root

---

## Prerequisiti
- .NET SDK 8.x
- Docker Desktop
- (Opzionale) pgAdmin / DBeaver / DataGrip

---

## Avvio rapido (Development)

### 1) Avvia PostgreSQL
Dalla root del repository:

```bash
docker compose up -d
