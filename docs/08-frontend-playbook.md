# Frontend Playbook (Angular)

This playbook outlines a senior-level approach to building a modern Angular frontend for VoucherShop. It assumes the existing API, multi-tenancy model (claim `shop_id`), and JWT-based authentication.

## Stack & Project Setup
- Use the latest Angular CLI (or Nx if you prefer enforced boundaries) with **standalone components**; reserve feature modules only for lazy-loading boundaries.
- Enforce quality gates from day one: ESLint, Prettier, stylelint (if using CSS/SCSS), and Husky + lint-staged to block bad commits.
- Adopt a design system: start with Angular Material + custom theming tokens (CSS variables) and document components in Storybook.
- Configure environment files for API base URL and a `tenantHeader` name to propagate `shop_id` to the backend.

## Routing, Auth, and Multi-tenancy
- Structure routes by feature and lazy-load authenticated areas; use route guards for authentication and role/tenant checks.
- Centralize HTTP interceptors: attach the access token, refresh on 401 with a single-flight mechanism, and inject `shop_id` via the configured header.
- Keep the active tenant in a dedicated service (or signal store) and expose an observable/signal for guards and components. Block cross-tenant navigation both in guards and by validating API responses.
- Persist minimal auth state (tokens in sessionStorage with in-memory fallback) and derive the user profile/roles from a `/me` endpoint on bootstrap.

## Data & State Management
- Prefer data-fetching utilities (e.g., TanStack Query for Angular or NgRx Query) for cache, retry, deduplication, and background refresh; keep global store surface small.
- Use **ComponentStore** or signals for local UI state; reserve NgRx Store + Effects + Entity for cross-cutting domain entities (vouchers, shops, users) that need normalization.
- Generate API clients from OpenAPI (e.g., `openapi-typescript-codegen`) to avoid type drift; keep DTOs separate from UI view models when mapping is needed.
- Apply server-driven pagination/sorting/filters for voucher grids; invalidate caches on mutations and use optimistic updates only for low-risk actions.

## UI/UX and Accessibility
- Implement responsive layouts with Material layout utilities or a utility-first layer (e.g., Tailwind) scoped to components. Avoid global styles.
- Support theming (light/dark/brand) via CSS variables; expose tenant branding tokens for logo/colors where appropriate.
- Enforce accessibility: ARIA roles, focus management for dialogs, keyboard navigation, skip links, and automated checks (axe) in CI.
- Use `ChangeDetectionStrategy.OnPush`, `trackBy` in lists, `ngOptimizedImage`, and defer/lazy-load heavy components to keep the UI performant.

## Developer Experience & Testing
- Provide npm scripts for `lint`, `test`, `e2e`, and `build` with budgets; analyze bundles via `ng build --configuration production --stats-json` and a bundle analyzer.
- E2E: use Playwright or Cypress with mockable API fixtures; for component tests prefer Testing Library to keep focus on behavior.
- Observability: send structured logs with request IDs and user/tenant context; integrate web vitals and error reporting (e.g., Sentry) early.

## Onboarding Checklist
- Scaffold the workspace, install linters/formatters/hooks, and set up CI steps mirroring local scripts.
- Add shared shell (layout, nav, tenant selector), authentication pages, and protected feature routes.
- Implement interceptors, tenant service, and generated API client integration.
- Ship a first vertical slice (e.g., voucher list + filters + detail) with tests and Storybook stories to validate the architecture.
