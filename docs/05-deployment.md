# Deployment (Provider-agnostic)

This guide describes what you need to deploy VoucherShop without committing secrets.

## Minimum requirements
- A host capable of running a .NET 8 Web API (container or native)
- A PostgreSQL database (managed or self-hosted)
- HTTPS termination (recommended)

## Configuration via environment variables
You should set:

### Database
- `ConnectionStrings__DefaultConnection`

### JWT
- `Jwt__Key` (Base64, >= 32 bytes)
- `Jwt__Issuer`
- `Jwt__Audience`
- `Jwt__ExpiresMinutes`

## Build & run
Typical approaches:
- Build on CI and deploy artifacts
- Dockerize the API and deploy a container
- Use a PaaS that supports .NET

## Docker image (optional)
If you containerize:
- Provide env vars at runtime
- Expose port 8080 (or provider default)
- Use health checks

## Database migrations in production
Options:
1. Run `dotnet ef database update` as part of a deployment job
2. Run migrations at startup (not recommended unless controlled)
3. Manual migrations with versioning (advanced)

Recommendation:
- Use a deployment step to run migrations once per release.

## Security checklist
- Do not commit secrets (JWT key, db password)
- Use HTTPS
- Restrict CORS to your frontend domain
- Disable bootstrap endpoints (`register-shop`) after initial setup
