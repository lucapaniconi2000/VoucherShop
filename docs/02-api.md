# API Overview

Swagger is available in Development:
- `https://localhost:7034/swagger`

## Authentication
Bearer tokens are required for protected endpoints.

In Swagger:
1. Call `/api/auth/login`
2. Copy `accessToken`
3. Click **Authorize**
4. Paste:
   `Bearer <accessToken>`

## Endpoints

### Auth
- `POST /api/auth/register-shop`
  - Creates a `Shop` + an `Admin` user.
  - Use only for bootstrap/setup.
- `POST /api/auth/login`
  - Returns `accessToken`.
  - Stores refresh token in HttpOnly cookie.
- `POST /api/auth/refresh`
  - Rotates refresh token and returns a new access token.
- `POST /api/auth/logout`
  - Revokes refresh token.

### Admin (Role = Admin)
- `POST /api/admin/users`
  - Creates a user within the admin’s shop (role User).
- `PUT /api/admin/vouchers/{userId}`
  - Creates/updates voucher for a user within the admin’s shop.
- `GET /api/admin/vouchers/{userId}/history`
  - Returns audit history for that user voucher.

### User (Authenticated)
- `GET /api/me/voucher`
  - Returns the voucher for the current user (uses `ICurrentUser`).

## E2E test flow (Swagger)
1. `POST /api/auth/register-shop`
2. `POST /api/auth/login` as Admin → Authorize
3. `POST /api/admin/users` → copy `userId`
4. `PUT /api/admin/vouchers/{userId}` → set balance
5. `GET /api/admin/vouchers/{userId}/history`
6. `POST /api/auth/login` as User → Authorize
7. `GET /api/me/voucher`
