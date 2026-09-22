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

## Running it

**Backend** (needs a local Postgres — see `.env.example`):

```bash
cp .env.example .env   # fill in your local Postgres password
cd backend
dotnet run --project Host
```

**Frontend**:

```bash
cd frontend
npm install
npm start
```

**Or everything via Docker** (Postgres still runs locally, not in Docker —
see `docker-compose.yml` comments):

```bash
docker-compose up -d --build
```

## Learn more

For architecture, module boundaries, and the reasoning behind them, see
[`docs/ARD.md`](docs/ARD.md). For the external gRPC ticketing integration,
see [`docs/external-issue-intake.md`](docs/external-issue-intake.md). Dev-only
seed credentials are in [`docs/dev-seed-credentials.md`](docs/dev-seed-credentials.md).
