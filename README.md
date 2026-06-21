# VerraCloud Bond Navigator — Portfolio Holdings Dashboard

A full-stack portfolio management dashboard for tracking fixed income and equity holdings with real-time P&L calculation.

---

## Setup & Run

**Requirements:** .NET 10, Node 20, npm

### Backend

```bash
cd backend
dotnet restore
dotnet run
```

API runs at `http://localhost:5283`  
Swagger UI at `http://localhost:5283/swagger`

Database is created and seeded automatically on first run. No manual setup required.

### Frontend

```bash
cd frontend
npm install
npm run dev
```

App runs at `http://localhost:5173`

### Run with Docker (alternative)

**Requirements:** Docker Desktop

```bash
docker compose up --build
```

Backend at `http://localhost:5283`  
Frontend at `http://localhost:5173`  
Swagger at `http://localhost:5283/swagger`

**Total setup time: under 5 minutes.**

### Tests

```bash
cd Backend.Tests
dotnet test
```

13 tests, all passing.

---

## Architecture Overview

```
frontend/                          backend/
├── Redux store                    ├── Controllers/
│   └── holdingsSlice              │   ├── HoldingsController
│       ├── async thunks           │   └── PricesController
│       └── SignalR reducer        ├── Services/
├── Components/                    │   ├── HoldingService (P&L calc)
│   ├── PortfolioSummary           │   └── PriceUpdateService (background)
│   ├── HoldingsTable              ├── Repositories/
│   ├── AddHoldingForm             │   ├── HoldingRepository
│   └── ConfirmModal               │   └── PriceRepository
└── Services/                      ├── Hubs/
    └── signalr.js                 │   └── PriceHub (SignalR)
                                   ├── Models/
                                   │   ├── Holding
                                   │   └── Price
                                   └── Data/
                                       └── AppDbContext (EF Core)
```

### Request Flow

```
React component
  → dispatches Redux thunk
  → axios calls REST API
  → Controller receives request
  → Controller calls Service (business logic + P&L)
  → Service calls Repository (data access only)
  → Repository queries SQLite via EF Core
  → Response returns up the chain
```

### Real-Time Flow

```
PriceUpdateService (every 10 seconds)
  → refreshes all prices in SQLite ±2%
  → broadcasts PricesUpdated via SignalR hub
  → frontend receives event
  → Redux dispatches pricesUpdated action
  → P&L recalculated in slice
  → components re-render with new values
```

---

## Design Decisions

### Why did you structure the backend the way you did?

Controller → Service → Repository is a deliberate separation of concerns.

Controllers handle HTTP only — parsing requests, returning correct status codes, nothing else. Services own business logic — the P&L calculation lives here, not in the controller or repository. Repositories handle data access — the only layer that knows about EF Core.

This separation means business logic is independently testable without spinning up HTTP or a real database. It also means swapping SQLite for PostgreSQL in production only requires changing the repository layer — nothing above it changes.

The background price service uses `IHostedService` because it needs to run independently of HTTP requests on a fixed interval. It uses `IServiceScopeFactory` to create a fresh `DbContext` on each tick — required because `DbContext` is Scoped but the background service is a Singleton. Injecting a Scoped service directly into a Singleton causes a captive dependency bug.

### How would you scale this if 10,000 users hit it simultaneously?

**Immediate bottlenecks to address:**

- **SQLite → PostgreSQL** — SQLite doesn't support concurrent writes. PostgreSQL handles thousands of concurrent connections and is production-grade.
- **Single instance → AKS with multiple pods** — horizontal scaling behind a load balancer handles traffic spikes.
- **SignalR in-memory → Redis backplane** — SignalR state is currently per-process. With multiple pods, a client on pod A won't receive a broadcast from pod B. Redis backplane fixes this so any pod can reach any client.
- **Background service runs on every pod** — with multiple instances, prices would be refreshed N times per tick. Fix with a distributed lock (Redis) or move price refresh to a dedicated Azure Function triggered on a schedule.

**Further optimisations:**

- Cache prices in Redis — they change every 10 seconds, no need to hit the database on every holdings request
- Read replicas for GET /api/holdings — separate read and write databases (CQRS pattern)
- Rate limit POST /api/holdings per user to prevent abuse
- Server-side pagination on GET /api/holdings for users with very large portfolios

### What's the one thing you'd improve with another 2 hours?

Authentication. Right now any user can add or delete any holding — there's no concept of ownership. I'd add JWT authentication so each user has their own portfolio, holdings are scoped per user, and the API rejects unauthenticated requests. The layered architecture makes this straightforward to add — an `[Authorize]` attribute on controllers, a `UserId` column on the Holdings table, and a filter in the repository layer so users only see their own data.

A close second would be moving from SQLite to PostgreSQL. SQLite is fine for a take-home but doesn't support concurrent writes — the first production bottleneck would be the write path under real load.

---

## AI Usage Log

We use AI tools as force multipliers. Here is an honest account of how AI was used on this project.

### Tools Used

- **Claude (claude.ai)** — primary tool throughout the project
- **Cursor** — AI-assisted code editing throughout the project

### Where AI Helped

**1. EF Core repository pattern scaffolding**  
Claude generated the initial `IHoldingRepository` interface and `HoldingRepository` implementation including the async query patterns. I reviewed the output and noticed `AsNoTracking()` was missing on read queries — important for performance on GET endpoints since we never modify the returned entities. Added it to all read methods.

**2. SignalR background service pattern**  
Injecting `IHubContext<PriceHub>` into a Singleton background service is a non-obvious pattern. Claude provided the correct implementation and explained why `IServiceScopeFactory` is needed when a Singleton needs to resolve Scoped services. I verified this against Microsoft's SignalR documentation before using it.

**3. Redux slice structure for dual loading states**  
Claude suggested separating `status`/`error` (for fetching holdings) from `addStatus`/`addError` (for adding a holding) in the slice. This allowed the Add button to show its own loading spinner independently from the table's loading state — a detail I wouldn't have caught until it caused a UX bug.

### Where AI Got It Wrong

**DateTime.UtcNow in EF Core seed data**  
Claude initially used `DateTime.UtcNow` inside the `HasData()` seed configuration in `AppDbContext`. EF Core requires static, deterministic values in seed data because migrations are fixed snapshots — dynamic values like `DateTime.UtcNow` change on every build, causing EF Core to detect a model change on every startup and throw a `PendingModelChangesWarning` that crashed the app.

I caught this from the error message on first run, understood why it was happening, and replaced all dynamic values with hardcoded static `DateTime` values. This is a good example of why you can't accept AI output without understanding it — the code looked correct but had a subtle runtime behaviour that only appeared when the app started.

---

## Bonus Features Implemented

| Feature                                 | Signal                                     |
| --------------------------------------- | ------------------------------------------ |
| SignalR instead of polling              | Real-time systems awareness                |
| Unit tests — 13 passing                 | Quality instinct                           |
| Pagination on holdings table            | Production thinking                        |
| Column sorting + P&L filtering          | UX depth                                   |
| Ticker dropdown from live API           | Prevents invalid input at the UX layer     |
| Delete confirmation modal               | No destructive action without confirmation |
| Ticker with no price handled gracefully | Edge case awareness                        |
| Swagger UI                              | API documentation at `/swagger`            |
| Docker + docker-compose                 | DevOps maturity                            |
