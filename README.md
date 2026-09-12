# Workout Tracker API

ASP.NET Core backend for logging workouts and viewing weekly and monthly progress.

## Architecture

Clean Architecture with thin controllers. Each request is dispatched through [Wolverine](https://wolverinefx.net/) to a feature handler. Validation runs automatically via FluentValidation.

| Project | Role |
| --- | --- |
| `WorkoutTracker.Domain` | Entities, policies, repository contracts. No framework dependencies. |
| `WorkoutTracker.Application` | Use cases, validators, DTOs. Organized by feature: Auth, Workouts, Dashboard. |
| `WorkoutTracker.Infrastructure` | EF Core + PostgreSQL, ASP.NET Identity, JWT, repositories. |
| `WorkoutTracker.Api` | HTTP layer: controllers, cookies, Problem Details. |
| `WorkoutTracker.Application.Tests` | Handler and validator tests (xUnit, NSubstitute). |

```
Api  →  Application  →  Domain
              ↑
        Infrastructure
```

Endpoints require a JWT unless marked anonymous. Errors map to RFC 7807 Problem Details (`400`, `401`, `404`, `409`).

## Features

**Auth** — register, login, refresh, logout, and `GET /api/auth/me`. Access tokens are JWTs. Refresh tokens are hashed in the database and stored in an `HttpOnly` cookie (`SameSite=Strict`).

**Workouts** — per-user CRUD for cardio, strength, flexibility, and mixed sessions. Each workout stores a local `performedAt` (no time zone), duration, calories, difficulty and fatigue (1–10), and optional notes. Lists are paginated and can be filtered by type.

**Dashboard** — current-week summary (count, duration, average difficulty/fatigue), last workout, and whether the user trained today. Progress returns the same stats week-by-week for a given month.

## Stack

- .NET 10, ASP.NET Core
- Wolverine + FluentValidation
- EF Core 10, PostgreSQL (Npgsql), snake_case columns
- ASP.NET Identity + JWT Bearer
- Central package versions in `Directory.Packages.props`

## Getting started

Requires the .NET 10 SDK, Make, and a running PostgreSQL instance. `make help` lists every target.

```bash
make setup                       # .env + restore + build
# Set Jwt__SigningKey in .env (at least 32 bytes, e.g. openssl rand -hex 32)
make sql                         # write artifacts/migration.sql — apply it, then delete it
make run                         # http://localhost:8080
make test
```

Overrides: `make run PORT=9000 CONFIG=Release`. New schema: `make migration name=AddSomething`, then `make sql`.

CI on `develop` and `main` restores, builds, and tests on .NET 10.

## API

| Method | Path | Auth |
| --- | --- | --- |
| `POST` | `/api/auth/register` | public |
| `POST` | `/api/auth/login` | public |
| `POST` | `/api/auth/refresh` | cookie |
| `POST` | `/api/auth/logout` | cookie |
| `GET` | `/api/auth/me` | JWT |
| `GET` | `/api/workouts` | JWT |
| `GET` | `/api/workouts/{id}` | JWT |
| `POST` | `/api/workouts` | JWT |
| `PUT` | `/api/workouts/{id}` | JWT |
| `DELETE` | `/api/workouts/{id}` | JWT |
| `GET` | `/api/dashboard` | JWT |
| `GET` | `/api/dashboard/progress` | JWT |

Send `Authorization: Bearer <access_token>`. Refresh and logout read the `refresh_token` cookie.
