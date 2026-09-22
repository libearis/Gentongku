# Backend scaffold notes (this pass)

## Local Postgres, not dockerized

Postgres runs as a **local instance already installed on this machine**, not
as a `postgres` service in `docker-compose.yml` (intentionally omitted).

The real password lives only in the repo-root `.env` file (gitignored — copy
`.env.example` to `.env` and fill it in; see also "Secrets & `.env`" below).
`backend/Host/appsettings.json`'s `ConnectionStrings:Default` only has a
`CHANGE_ME` placeholder password committed — `Program.cs` loads `.env` at
startup (via `DotNetEnv`) so the real value overrides it through ASP.NET
Core's standard env-var config convention (`ConnectionStrings__Default` env
var -> `ConnectionStrings:Default` config key). When running the `api`
service through `docker-compose up`, the same `.env` is read natively by
Compose for `${POSTGRES_PASSWORD}` substitution, but the connection string's
*host* is overridden in `docker-compose.yml` to `host.docker.internal`
instead of `localhost`, since inside a container `localhost` refers to the
container itself. On native Linux Docker (no Docker Desktop), you may need to
point that host entry at the real host IP instead of relying on
`host.docker.internal`.

## Secrets & `.env`

This repo is public, so no real credential is committed. `.env.example` at
the repo root documents every variable; copy it to `.env` (gitignored) and
fill in your actual local Postgres password. Both local `dotnet run` and
`docker-compose` read that same file — see the comments inside
`.env.example` and the top of `Program.cs` for how each one consumes it.

## CORS for local (non-Docker) dev

The API has no CORS policy by default, which blocks the Angular dev server
(a different origin) from calling it. `Cors:FrontendOrigins` in
`appsettings.Development.json` allows `http://localhost:4200` (Angular's
default dev port) to call the API when both are run locally outside Docker
(`dotnet run` on port `5000` per `launchSettings.json` + `ng serve`).

If `docker-compose up` was previously run for this project, its `api` and
`frontend` containers bind those same host ports (`5000`/`4200`) while
running, which will conflict with a local `dotnet run`/`ng serve` started at
the same time — stop the compose stack first (`docker-compose down`) if you
want to run natively instead.

## .NET SDK

`.NET 10` SDK (`10.0.400`) was already installed and is used throughout
(`net10.0` target framework for every project). `dotnet-ef` global tool
`10.0.11` was used to author/apply migrations.

## Migrations applied

Six real EF Core migrations (`InitialCreate`) were generated and applied
against the local `gentongku` database, one per module DbContext:
`identity`, `catalog`, `ordering`, `benchmark`, `scheduler`, `ticketing`
schemas — each with its own `__ef_migrations_history` table. Hangfire adds its
own tables (`hangfire.*`-equivalent, actually created under the default
`public` schema by `Hangfire.PostgreSql` unless configured otherwise) the
first time the API process starts and `UsePostgreSqlStorage` runs its own
internal migration.

## Deviations from a literal reading of AGENTS.md

- **Seller store name** is persisted in `catalog.seller_profiles`
  (`Catalog.Domain.Entities.SellerProfile`), not in `identity.users`. Identity
  stays purely about auth/roles; Catalog owns all storefront-facing data. The
  two modules are connected only through
  `Identity.Application.Abstractions.ISellerProfileProvisioner`, implemented in
  Catalog.Infrastructure and wired together from the Host — Identity never
  references Catalog directly (AGENTS.md section 3).
- **Ticketing got one table** (`ticketing.issue_reports`) storing the
  `issue_id` returned by the external TaskFlow gRPC call, per the "reasonable
  addition" called out in the task brief and AGENTS.md section 8's "store the
  returned issue_id locally and surface it in the ticket history list".
- Benchmark's four read strategies and the Hangfire dummy-data job body are
  intentionally **TODO-stubbed** (see `BenchmarkService.cs` and
  `GenerateDummyDataJob.cs`) per the explicit scope control in this task's
  instructions — the modules are fully wired (DbContext migrated, service
  registered, reachable via a trivial endpoint) but the deep logic described in
  AGENTS.md sections 6.1/6.2/7 is left for the next implementation pass.
- This pass is backend-only; it did not touch the existing Angular app already
  present under `frontend/` (it was scaffolded separately/previously). The
  `frontend/Dockerfile` added here just does a standard `npm ci && npm run
  build` + nginx multi-stage build against that existing app so
  `docker-compose build` succeeds — no frontend feature code was added.

## Seeded Admin credentials

See `docs/dev-seed-credentials.md`.
