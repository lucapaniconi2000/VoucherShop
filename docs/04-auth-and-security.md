
---

## docs/04-auth-and-security.md

```md
# Auth & Security

VoucherShop uses:
- ASP.NET Identity
- JWT Access Token (Authorization: Bearer)
- Refresh Token stored in HttpOnly cookie

## JWT requirements
The signing key must be strong enough for HS256:
- `Jwt:Key` must be Base64 representing **>= 32 bytes** (256-bit).

JWT contains:
- `userId` (`NameIdentifier`)
- `email`
- `role`
- `shop_id` (tenant isolation)

## Refresh token cookie
Refresh token is stored in a cookie:
- HttpOnly = true
- Secure = true (requires HTTPS)
- SameSite = None (recommended for Angular SPA)

## Role model
- Admin can manage users/vouchers within the same shop.
- Users can only read their own voucher.

## Production notes
- Protect or disable `POST /api/auth/register-shop` in production.
- Store secrets as environment variables (not in appsettings.json).
- Enable HTTPS and configure CORS for the Angular frontend.
