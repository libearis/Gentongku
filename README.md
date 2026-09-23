# Gentongku

Gentongku is a portfolio e-commerce app built to demonstrate a **.NET modular
monolith** (Clean Architecture, per-module Postgres schemas), a live
**Benchmark** admin tool for comparing cache/index/query strategies, and a
**Scheduler** (Hangfire) that generates dummy data on demand. The storefront
itself (Buyer/Seller/Admin roles, cart, checkout, seller store management) is
intentionally simple — it exists to give the three showcase features a real
app to live in.

## Tech stack

.NET 10 (ASP.NET Core Web API) · Angular (standalone components, signals) ·
PostgreSQL · Redis · Hangfire · Serilog + Seq · JWT auth · gRPC (admin ->
external ticketing service only)

## Getting started

Needs a local Postgres instance (see `.env.example`) — it isn't part of
`docker-compose.yml`.

1. `cp .env.example .env` and fill in your local Postgres password.
2. Set up the database (creates it if missing, applies every module's schema,
   seeds the admin account + sample catalog data):
   ```bash
   cd backend
   dotnet run --project Host -- migrate
   ```
   Only needed once per database — safe to re-run any time (it no-ops on
   whatever's already applied/seeded).
3. Start everything:
   ```bash
   docker-compose up -d --build
   ```

Login with the seeded admin account — see
[`docs/dev-seed-credentials.md`](docs/dev-seed-credentials.md).

## Learn more

For architecture, module boundaries, and the reasoning behind them, see
[`docs/ARD.md`](docs/ARD.md). For the external gRPC ticketing integration,
see [`docs/external-issue-intake.md`](docs/external-issue-intake.md).
