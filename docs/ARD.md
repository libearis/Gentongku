# Architecture Reference Document (ARD)

## 1. Shape of the system

Gentongku is a single deployable **modular monolith**, not microservices.
Each business capability is a *module*, internally layered using Clean
Architecture:

```
Modules/<Name>/
  <Name>.Domain/          entities, enums, invariants — no framework deps
  <Name>.Application/     use cases, DTOs, interfaces for anything external
  <Name>.Infrastructure/  EF Core DbContext, repositories, external clients
```

Dependency direction is strict and one-way: `Domain` depends on nothing,
`Application` depends only on `Domain`, `Infrastructure` depends on
`Application` (implementing its interfaces). `Host` (the composition root,
`Gentongku.Api`) is the only project allowed to know about every module at
once — it wires `Infrastructure` implementations to `Application` interfaces
via DI (`ModuleRegistration/`).

Modules: `Identity`, `Catalog`, `Ordering`, `Benchmark`, `Scheduler`,
`Ticketing`. `Benchmark`, `Scheduler`, and `Ticketing` skip a `Domain`
project — they're technical capabilities (a query-strategy engine, a job
runner, a gRPC client), not business domains with rich entities.

`BuildingBlocks/` is the shared kernel used by every module — generic,
framework-agnostic primitives only (`BaseEntity`, `Result<T>`, `PagedResult`,
domain event contracts, log enrichers). It never depends on a module, only
the reverse.

## 2. Database: one Postgres instance, one schema per module

Every module owns its own Postgres schema (`identity.*`, `catalog.*`,
`ordering.*`, `benchmark.*`, `scheduler.*`, `ticketing.*`) inside a single
database. There are **no cross-schema foreign keys** — a module that needs
data owned by another module stores a plain ID and resolves it (if needed)
through that module's public `Application`-layer interface, injected via DI
from `Host`. Example: Identity's `RegisterAsync` needs to create a seller
store profile, which is Catalog's data — it depends on
`ISellerProfileProvisioner` (declared in `Identity.Application`, implemented
in `Catalog.Infrastructure`), never on Catalog directly.

This is a deliberate, cheap way to enforce module boundaries at the database
level (not just by code review convention), and it keeps each module
extractable into a real microservice later without a rewrite — though that
extraction is explicitly out of scope for this project.

## 3. Auth & roles

JWT-based. Three roles: `Admin`, `Buyer`, `Seller`. Admin has no
self-registration path — Admin accounts only come from Admin › Users (or the
one seeded dev account, see `docs/dev-seed-credentials.md`). Buyer and Seller
pick their role at registration; Seller registration also asks for a store
name, provisioned into Catalog as described above. Login accepts either an
email or a username.

The Angular frontend mirrors this with route guards keyed off a signal-based
`AuthStore` (no NgRx/Akita — plain services + `signal()`/`computed()`),
matching each role to its own route tree (`/admin/**`, `/storefront/**`,
`/seller/my-store/**`).

## 4. The Benchmark module (the centerpiece)

Read Benchmark lets an admin pick a table (`Order`, `Produk`, `Kategori`) and
a column, then runs the *same logical query* four ways — Redis-only, DB
index-only, a forced live/full-scan query, and index+Redis together — with
each strategy independently toggleable so the UI reflects which code paths
actually ran. Write Benchmark compares insert throughput between two
structurally identical tables, one indexed and one not. Both exist to make
cache/index trade-offs visible and interactive rather than just described in
a README.

## 5. Scheduler (Hangfire)

Generates dummy data on demand (by row count or estimated storage size),
fire-and-forget from the UI's perspective. Job status is polled from the
frontend (deliberately no SignalR/websockets) and also visible in the
Hangfire dashboard (`/hangfire`) — the two views read the same underlying
job state rather than maintaining a parallel, driftable status table.

## 6. gRPC Ticketing (admin -> external service only)

The only gRPC usage in this app: Admin › Ticketing calls an external
"Issue Intake" service (native gRPC over HTTP/2, plaintext in dev) to file a
support ticket. It is fire-and-forget — no callback when the external
ticket's status changes — so Ticketing keeps a local `issue_reports` table
just to remember the returned `issue_id`. See
`docs/external-issue-intake.md` for the wire contract.

## 7. Explicitly out of scope

Elasticsearch (a disabled "Fase 2" tab exists in the Benchmark UI but nothing
is wired up), SignalR/websockets, a second database engine, full CQRS/event
sourcing/message broker, real payment gateway integration, and Seller KYC.
These are deliberate scope cuts for a portfolio project centered on the
modular monolith, Benchmark, and Scheduler pillars — not gaps to be
quietly filled in later without discussion.
