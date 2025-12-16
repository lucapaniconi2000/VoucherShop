# Database & Migrations

PostgreSQL runs locally using Docker Compose.

## Docker Compose

Current compose:

```yaml
services:
  postgres:
    image: postgres:16
    container_name: vouchershop-postgres
    environment:
      POSTGRES_DB: vouchershop
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"
    volumes:
      - vouchershop_pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres -d vouchershop"]
      interval: 5s
      timeout: 5s
      retries: 10

volumes:
  vouchershop_pgdata:
