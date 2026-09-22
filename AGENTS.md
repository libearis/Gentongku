# AGENTS.md — Gentongku

This file orients any AI coding agent (or human contributor) working in this
repository. Read it before generating code, scaffolding folders, or making
architectural decisions. If a decision isn't covered here, prefer the option
that keeps modules independently testable and swappable, per the philosophy
below.

## 1. What this project is

Gentongku is a portfolio e-commerce application built primarily to
demonstrate .NET backend depth (modular monolith, Clean Architecture,
Hangfire scheduling, Serilog, gRPC integration) alongside a from-scratch
Angular frontend. It is **not** a microservice project (that already exists
elsewhere in the author's portfolio) and it is **not** a full CQRS project
(same reason) — CQRS is referenced only as a teaching moment inside the
Write Benchmark module, not as the app's overall architecture.

Three pillars the codebase must showcase clearly:

1. A clean, DB-agnostic **modular monolith** in .NET.
2. A **Benchmark** admin tool that makes cache/index/query trade-offs visible
   and interactive, not just described in a README.
3. A **Scheduler** (Hangfire) that generates realistic dummy data on demand
   and is observable both in-app and in the Hangfire dashboard.

Everything else (auth, roles, storefront, seller tools, gRPC ticketing) exists
to give those three pillars a believable app to live in.

## 2. Tech stack

| Layer | Choice | Notes |
|---|---|---|
| Backend | .NET 10, ASP.NET Core Web API | Primary language of expertise; prioritize correctness and idiomatic patterns over cleverness |
| Frontend | Angular (latest stable) | State via Angular **signals** + plain services — **no NgRx, no Akita** |
| Database | PostgreSQL | Chosen for Docker-friendliness and no licensing friction; must stay swappable (see Section 3) |
| Cache | Redis | Used for real caching *and* as a first-class subject of the Benchmark module |
| Background jobs | Hangfire (Postgres storage) | Powers the Scheduler module; dashboard exposed at `/hangfire` |
| Inter-app integration | gRPC (native HTTP/2) | Used only for the admin → external support app "Issue Intake" ticket flow |
| Auth | JWT | Angular manages session state client-side via a signal-based auth store (the Pinia-equivalent) |
| Logging | Serilog → Postgres sink **and** Seq | Postgres sink feeds the in-app Log Viewer (business/job logs); Seq (own container) is for technical/infra logs — see Section 9 |
| Containerization | Docker + docker-compose | Single command spins up api, frontend, postgres, redis, seq |
| Search (phase 2, not yet built) | Elasticsearch | Slot already reserved in the Benchmark UI as a disabled "Fase 2" tab — do not wire it up until explicitly asked |

Explicitly **not** used: NgRx/Akita, SignalR (polling is used instead for job
status), microservices, a second database/NoSQL store for the write
benchmark (it uses two tables in the same Postgres database).

## 3. Architecture: modular monolith + Clean Architecture

Single deployable unit. Each business module is internally layered
(Domain → Application → Infrastructure) and modules never reference each
other's Infrastructure or Domain directly — only through Application-layer
contracts (interfaces) registered in the composition root. This is what
makes "reuse any DB" viable: swapping Postgres for SQL Server means touching
only each module's Infrastructure project.

```
backend/
  Modules/
    Identity/            # users, roles (Admin/Buyer/Seller), JWT issuing
      Identity.Domain/
      Identity.Application/
      Identity.Infrastructure/
    Catalog/              # products, categories, seller store profile
      Catalog.Domain/
      Catalog.Application/
      Catalog.Infrastructure/
    Ordering/              # cart, checkout, orders, order status
      Ordering.Domain/
      Ordering.Application/
      Ordering.Infrastructure/
    Benchmark/             # read/write benchmark engine, health checks
      Benchmark.Application/
      Benchmark.Infrastructure/
    Scheduler/             # Hangfire jobs: dummy data generation, job log
      Scheduler.Application/
      Scheduler.Infrastructure/
    Ticketing/             # gRPC client to the external IssueIntake service
      Ticketing.Application/
      Ticketing.Infrastructure/
  BuildingBlocks/          # shared kernel: base entity, Result<T>, paging,
                            # domain events, Serilog enrichers
  Host/                    # Gentongku.Api — composition root
    Program.cs
    appsettings.json
    ModuleRegistration/     # one *.AddXModule() extension per module
frontend/
  src/app/
    core/
      auth/                # signal-based auth store, guards, interceptors
    features/
      auth/                # login/register (role picker: Buyer/Seller)
      storefront/          # home, product detail, cart, checkout, order history
      seller/
        my-store/
        shopping/          # re-exports storefront components
      admin/
        users/
        ticketing/
        benchmark/
          read/
          write/
          search/          # phase 2, route present but disabled
          health/
        scheduler/
          generate-data/
          job-monitor/
    shared/                # design tokens, buttons, cards, layout shells
docker-compose.yml
AGENTS.md
```

**Database strategy**: one Postgres instance, one schema per module
(`identity.*`, `catalog.*`, `ordering.*`, `benchmark.*`, `scheduler.*`).
No cross-schema foreign keys — cross-module references are stored as plain
IDs and resolved through the owning module's Application layer if needed.
This keeps modules extractable into real microservices later without a
rewrite, even though that's explicitly out of scope for this project.

**Module communication**: in-process interface calls only (no message bus —
that's microservice territory). If a module needs data from another, it
depends on that module's public `IXQueries` interface, injected via DI from
the Host.

## 4. Auth & roles

- JWT-based auth. Token issued by `Identity` module on login/register.
- Angular stores the decoded session in a **signal-based auth store**
  (`AuthStore` service using `signal()`/`computed()`), analogous to a Pinia
  store — not NgRx. Token itself lives in memory + a refresh mechanism；
  avoid `localStorage` for the raw JWT if feasible, otherwise document the
  trade-off inline.
- Three roles: `Admin`, `Buyer`, `Seller`. No self-registration for Admin —
  Admin accounts are created from inside the Admin › Users module only.
- Buyer and Seller choose their role at `/auth` (register tab, segmented
  control). Seller registration additionally asks for a store name (kept
  intentionally simple — no KYC, no document upload).
- Route guards by role:
  - `Admin` → `/admin/**` (Admin Menu, Benchmark, Scheduler) + read-only
    `/storefront/**` (no cart/checkout routes activated for this role).
  - `Buyer` → `/storefront/**` only.
  - `Seller` → `/seller/my-store/**` and `/storefront/**` (rendered under a
    `Shopping` tab, but it is the exact same components as Buyer's — no
    duplicate implementation).

## 5. Storefront & seller flows

Keep e-commerce flows intentionally simple — they exist to host the
benchmark/scheduler features, not to be the deep focus of the project.

- **Buyer**: home (product grid) → product detail (variant + qty) → cart →
  checkout (mock payment methods: transfer / QRIS / COD, no real gateway) →
  order history (Pending → Paid → Shipped → Completed) → review on
  completed orders.
- **Seller**: `My Store` (store profile card: name, address, category tags;
  incoming orders with status updates; product CRUD) and `Shopping` (alias
  of the Buyer flow). Dummy products generated by the Scheduler can be
  randomly assigned to any seller, or to a reserved "System Seller" —
  decide at implementation time, not a hard requirement.
- Product "Populer" badges are driven by Redis hit-count — this is a
  deliberate tie-in to the Benchmark theme, not just decoration.

## 6. Benchmark module (the centerpiece)

Two independent sub-modules plus one reserved slot:

### 6.1 Read Benchmark
Lets the admin **choose what to query**, not just view a fixed demo:

- **Table picker** (max 3 tables, ordered by size): `Order` (~2.4M rows),
  `Produk` (~86k rows), `Kategori` (~42 rows).
- **Column picker**, scoped to the selected table, reflecting real indexes
  actually created in the migration (e.g. `Order.status`, `.created_at`,
  `.user_id` indexed; `Order.notes` intentionally left unindexed).
- Running the benchmark executes the **same logical query** four ways:
  1. Redis only (cache hit, bypasses DB)
  2. DB index only (no cache)
  3. Live query (forced full scan / cache+index bypass — for teaching, not
     representative of a real code path you'd ship elsewhere)
  4. Index + Redis (the realistic "production" path)
- Each strategy card has an **independent on/off toggle**. Turning a
  strategy off must actually skip executing that code path server-side
  (don't just hide the UI number) and the chart must reflect "not run" for
  that bar.
- **Important teaching behavior**: if the selected column has no index,
  the "DB Index Only" result must degrade to the live-query result (there's
  no index to hit), and "Index + Redis" must degrade to the Redis-only
  result (caching still helps, indexing doesn't). Implement this as a real
  consequence of the query plan, not a hardcoded UI branch, where feasible.

### 6.2 Write Benchmark
Two structurally identical tables in the same Postgres database:
`orders_indexed` (many indexes) and `orders_plain` (zero indexes). Insert
the same batch into both and report rows/sec and avg ms/insert for each.
This is the entire CQRS-adjacent teaching point — do not build a real
CQRS read/write split for this; two tables and a comparison view are
sufficient and intentional.

### 6.3 Search Benchmark (phase 2 — reserved, not built yet)
UI already has a disabled tab/nav entry labeled "Search (Elasticsearch)"
with a "Fase 2" badge. When this phase starts: compare `ILIKE` search on
`Produk.name` against Elasticsearch full-text search. Do not scaffold
Elasticsearch infrastructure (containers, indices, sync jobs) until this
phase is explicitly greenlit — premature infra here just adds
docker-compose weight for no visible benefit yet.

### 6.4 Health Check
Standard ASP.NET Core health checks (`/health`, DB + Redis + Hangfire
storage checks) surfaced as a status item inside the Benchmark section of
the Admin sidebar (not a separate top-level section).

## 7. Scheduler module (Hangfire)

- **Generate Dummy Data**: admin picks either a target **row count** or a
  target **storage size** (estimated backward into a row count using an
  average row size constant — exact byte-precision isn't the goal, a
  documented estimate is fine). Submitting enqueues a Hangfire job
  immediately ("fire and forget" from the UI's perspective — the request
  returns success right away; the job's own lifecycle is tracked
  separately).
- **Job Monitor**: a list of past/queued jobs with status
  (`Processing` / `Success` / `Error`), polled from the frontend (no
  SignalR). The same jobs must be visible in the Hangfire dashboard at
  `/hangfire` — do not build a parallel job-tracking table that drifts from
  Hangfire's own state; either read Hangfire's storage directly or persist
  a thin `job_runs` row keyed by the Hangfire job ID and keep it in sync
  from within the job itself (`OnStateApplied` or explicit status updates
  at start/success/failure).
- Two more module cards are reserved for future scheduler work: **Report
  Maker** and **Test-Data Cleanup**. Stub these as disabled/"coming soon"
  cards in the UI; don't implement until asked.

## 8. gRPC Issue Intake (admin → external support app)

Scope is narrow and must stay narrow: this gRPC client is used **only**
from Admin › Ticketing, when an admin files a support ticket against the
separate external application. It is not a general-purpose RPC layer for
this app's own frontend-backend communication (that stays plain HTTP/REST).

- Proto contract, connection details, and field validation rules are fixed
  by the external service's own spec (see `EXTERNAL_ISSUE_INTAKE.md` if
  present in the repo, or request it again if not) — do not invent new
  fields or change `CreateIssueRequest`/`CreateIssueResponse` shapes.
- Native gRPC over HTTP/2, plaintext in dev (no TLS) — do not add TLS
  config unless the external service's environment changes. Do not use
  grpc-web or Envoy for this call path; that indirection exists only for
  browser clients, and this call always originates from the .NET backend.
- Fire-and-forget by design: there is no callback when the external
  ticket's status changes. Don't build polling against the external
  service to fake a callback — just store the returned `issue_id` locally
  and surface it in the ticket history list.
- Failure handling: map `INVALID_ARGUMENT` and `INTERNAL` gRPC statuses to
  a clear inline error in the ticket form; never swallow the error as a
  generic "something went wrong."

## 9. Logging & observability

- Serilog throughout the backend, writing to **two sinks**:
  1. A Postgres sink (`logs.application_log`, its own schema) feeding the
     custom **Log Viewer** module inside Admin Menu — this covers
     business-level events (job runs, ticket submissions, benchmark runs).
  2. A **Seq** sink, pointed at a `seq` container (`datalust/seq` image) —
     this covers technical/infra-level logs (request pipeline, exceptions,
     slow-query warnings). Seq needs no real config beyond the connection
     string; don't build anything custom on top of it, it's a dev tool.
- The two overlap by design at the "something went wrong" level, but the
  in-app Log Viewer is the one non-technical stakeholders would ever see;
  Seq is strictly a developer tool, not linked from the app's own UI.
- Health checks live under the Benchmark section per Section 6.4, not duplicated
  under Admin Menu.

## 10. Docker & local development

`docker-compose.yml` at the repo root should bring up, at minimum:

| Service | Image / build | Port |
|---|---|---|
| `api` | build from `backend/Host` | `5000` (REST) + gRPC client only, no inbound gRPC server needed here |
| `frontend` | build from `frontend/` (nginx serving the Angular build) | `4200` or `80` |
| `postgres` | `postgres:16` | `5432` |
| `redis` | `redis:7` | `6379` |
| `seq` | `datalust/seq:latest` | `5341` (UI + ingestion) |

Environment variables (names indicative, finalize during implementation):

```
ConnectionStrings__Default=Host=postgres;Database=gentongku;...
Redis__ConnectionString=redis:6379
Jwt__Issuer=gentongku
Jwt__Secret=<dev-only-secret>
Seq__ServerUrl=http://seq:5341
IssueIntake__Host=<external-app-host>
IssueIntake__Port=50051
Hangfire__DashboardPath=/hangfire
```

`docker-compose up` should be sufficient to get a fully working stack
(migrations applied automatically on `api` startup in dev, or via a
one-shot migration container — pick one and document it in the root
README once implementation starts).

## 11. Frontend conventions

- Angular standalone components, signals for local + shared state.
- One `AuthStore` (signal-based) as the single source of truth for the
  current user/role/token; components read it via `computed()`, never
  duplicate role logic in individual components.
- Role-based layout shells: `AdminShell`, `BuyerShell` (bare storefront),
  `SellerShell` (My Store / Shopping tab switcher). Keep these thin —
  layout only, no business logic.
- Design language (carried over from the approved mockups):
  - Typeface: **Fraunces** (display/headings) + **IBM Plex Sans** (UI) +
    **IBM Plex Mono** (metrics/numbers, used heavily in Benchmark/Scheduler).
  - Two visual registers: warm/light theme for buyer-facing storefront,
    dark/technical theme for Admin, Benchmark, Scheduler.
  - Brand accent: terracotta (`#C1502E`), used sparingly per CDS-style
    restraint (one primary accent per view).
  - The brand mark is the wordmark "Gentongku" with the "o" in "gentong"
    rendered as a small pot glyph (inline SVG, `currentColor` fill) — reuse
    the existing glyph markup rather than re-deriving it.
  - Login page: a single centered card ("Gentongku" title, tabs, form)
    layered on top of one large gentong (barrel + ridged lid) illustration
    that fills the page background. The card is semi-transparent
    (`rgba(255,255,255,0.85)`, no heavy drop shadow) so the gentong subtly
    shows through — this is the approved final direction; don't revert to
    the earlier split left/right panel layout.
  - The cart icon across the app is a small gentong/jug glyph, not a
    shopping-cart/basket icon — this is a deliberate brand choice, keep it
    consistent anywhere a "cart" affordance is added later.

## 12. Coding conventions

- **Backend**: standard .NET naming (PascalCase types/members, `I`-prefixed
  interfaces), nullable reference types enabled, `Result<T>`-style returns
  from Application layer handlers rather than throwing for expected
  failure paths (e.g. validation). Prefer explicit DTOs over exposing
  EF entities across layer boundaries.
- **Tests**: xUnit for unit tests; Testcontainers (Postgres + Redis) for
  integration tests, scoped per module so a module's test suite can run in
  isolation. Benchmark and Scheduler modules especially need integration
  tests that assert on actual query plans / actual Hangfire job state, not
  just mocked timings — the whole point of this project is that the
  numbers are real.
- **Frontend**: keep components small and role-agnostic where possible;
  business rules (what a Buyer vs Seller can do) belong in guards/services,
  not scattered `*ngIf`-style role checks across templates.
- **Commits/PRs**: one module per PR where feasible, given the modular
  monolith boundaries — makes review and portfolio narrative easier.

## 13. Things to explicitly NOT build (yet)

- Elasticsearch infra (Section 6.3) — UI slot only, reserved for phase 2.
- SignalR / any websocket-based real-time layer — polling is the accepted
  trade-off.
- Any second database engine for the write benchmark — it's two tables,
  one Postgres instance.
- Full CQRS pipeline, event sourcing, or a message broker — out of scope,
  already covered by another portfolio project.
- Payment gateway integration — checkout payment methods are mocked.
- Seller KYC / document verification — registration stays minimal.
