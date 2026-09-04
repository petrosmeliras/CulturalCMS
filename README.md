# CulturalCMS

A Cultural Content Management System — a full-stack web application for cataloguing, curating, and publicly presenting cultural items (sculptures, architectural elements, artifacts, and more), with user roles, a content-approval workflow, dynamic metadata, and a full audit trail.

---

## Tech Stack

**Backend**
- .NET 8 Web API, layered architecture (Domain → Application → Infrastructure → API)
- Entity Framework Core (Code-First), PostgreSQL
- JWT authentication, role-based authorization (Admin / Curator / Contributor)
- AutoMapper, Repository + Unit of Work pattern
- Global exception handling (RFC 7807 Problem Details), Serilog
- Rate limiting, in-memory caching
- xUnit v3, NSubstitute (unit & integration tests)

**Frontend**
- React 19, TypeScript, Vite
- Material UI (MUI)
- React Router
- React Hook Form + Zod (validation)
- Axios

**Infrastructure**
- Docker Compose (backend + frontend + PostgreSQL)

---

## Roles & Test Users

On the backend's first start, the database is created and populated automatically (migrations + seeding) — no manual step is required.

| Role | Username | Password | Permissions |
|---|---|---|---|
| **Admin** | `admin` | `Admin123!` | Full access — CRUD on items of any status, user/role management, deletion, audit trail |
| **Curator** | `curator` | `Curator123!` | Approve/reject items under review, view all items, audit trail |
| **Contributor** | `contributor` | `Contrib123!` | Create/edit own Draft items, submit them for review |
| **Contributor** (2nd) | `contributor2` | `Contrib123!` | Same as above — useful for testing multiple owners |
| **Public** (anonymous) | — | — | No login required — view/search Published items only |

The database is also seeded with **15+ sample cultural items** (varied categories, historical periods, statuses, and metadata) so search and filtering can be tested immediately.

---

## Requirements

Before you start, make sure you have:
- **Docker Desktop** (for `docker compose up`)
- **.NET 8 SDK** — only if you want to run the tests locally (`CulturalCMS.Tests`); not needed to run the app itself
- Free ports: **3000** (frontend), **8080** (backend), **5432** (PostgreSQL)

---

## Getting Started

### 1. Create your `.env`

From the project root, copy the example file and fill in the values:

```bash
cp .env.example .env
```

The example file ships with working demo values, so you can run the project without changing anything. `DB_PASSWORD` and `JWT_SECRET` are demo credentials meant for local evaluation only. See [Environment Variables](#environment-variables) below for the full list.

> **Note:** `DB_HOST` must be `db` (the name of the database service inside the Docker network), **not** `localhost`.

### 2. Run everything

```bash
docker compose up --build
```

This starts all three containers together:
- **PostgreSQL** — the database
- **Backend API** — `http://localhost:8080` (Swagger UI at `http://localhost:8080/swagger`)
- **Frontend** — `http://localhost:3000`

Migrations and database seeding run automatically on first start.

### Environment Variables

Defined in `.env` at the project root (not committed — see `.env.example`):

| Variable | Description | Example |
|---|---|---|
| `DB_HOST` | PostgreSQL hostname inside the Docker network | `db` |
| `DB_PORT` | PostgreSQL port | `5432` |
| `DB_NAME` | Database name | `CulturalCMSDb` |
| `DB_USER` | PostgreSQL user | `postgres` |
| `DB_PASSWORD` | PostgreSQL password | `CulturalCMS_Demo_Db_Pass_2026` |
| `JWT_ISSUER` | JWT token issuer | `https://localhost:8080` |
| `JWT_AUDIENCE` | JWT token audience | `https://localhost:8080` |
| `JWT_SECRET` | JWT signing key (32+ chars) | `CulturalCMS_Demo_Secret_Key_1234567890_abcdef` |
| `CORS_ORIGIN` | Allowed CORS origin (the frontend URL) | `http://localhost:3000` |
| `ALLOWED_HOSTS` | Allowed hosts | `*` |
| `APP_PORT` | Backend API port | `8080` |
| `FRONTEND_PORT` | Frontend port | `3000` |
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | `Development` |

### Useful Commands

```bash
docker compose down                    # Stop all containers
docker compose logs -f webapp          # Follow backend logs in real time
docker compose up --build frontend     # Rebuild only the frontend
docker compose down -v                 # Stop + delete DB data (clean restart)
```

---

## API Overview

Base path: `/api/v1`. Full, interactive documentation is available in **Swagger UI** at `http://localhost:8080/swagger`.

### Authentication

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/auth/register` | Public | Register a new user (created as Contributor) |
| POST | `/auth/login` | Public | Authenticate and receive a JWT |

Use the token in subsequent requests: `Authorization: Bearer <token>`

### Cultural Items

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/cultural-items` | Public | List published items |
| GET | `/cultural-items/search` | Public | Search/filter published items (paginated) |
| GET | `/cultural-items/{id}` | Public | Get an item by id (non-published visible only to owner/Curator/Admin) |
| GET | `/cultural-items/all` | Curator, Admin | List items of any status |
| GET | `/cultural-items/search/all` | Curator, Admin | Search/filter items of any status (paginated) |
| GET | `/cultural-items/my-items` | Contributor, Admin | Search the current user's own items (paginated) |
| POST | `/cultural-items` | Contributor, Admin | Create a new item (starts as Draft) |
| PUT | `/cultural-items/{id}` | Contributor, Admin | Update an item |
| DELETE | `/cultural-items/{id}` | Admin | Delete an item (soft delete) |
| POST | `/cultural-items/{id}/submit` | Contributor, Admin | Submit a Draft for review |
| POST | `/cultural-items/{id}/approve` | Curator, Admin | Approve an item under review (publish it) |
| POST | `/cultural-items/{id}/reject` | Curator, Admin | Reject an item under review (back to Draft) |

### Users

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/users/{id}` | Bearer | Get a user by id |
| GET | `/users/by-username/{username}` | Bearer | Get a user by username |
| GET | `/users` | Admin | List users (paginated + filtered) |
| PUT | `/users/{id}/role` | Admin | Update a user's role |

### Images & Audit

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/images/upload` | Any authenticated | Upload an item image (jpg/png, max 5 MB) |
| GET | `/cultural-items/{itemId}/audit-logs` | Curator, Admin | Get the change timeline for an item |

---

## Error Responses

All errors return a structured JSON body (RFC 7807 Problem Details). Validation failures include per-field messages.

| HTTP Status | Cause |
|---|---|
| 400 | Validation error (invalid input) |
| 401 | Missing or invalid JWT |
| 403 | Insufficient permissions for the role |
| 404 | Resource not found |
| 409 | Conflict (already exists, or invalid workflow state transition) |
| 500 | Unexpected server error |

---

## Testing

The test project (`CulturalCMS.Tests`) contains:
- **Unit tests** for business logic (`CulturalItemService` — state machine & permissions; `AuthService` — registration & password hashing) and query building (`CulturalItemQueryBuilder` — filtering/sorting), using NSubstitute mocks and the EF Core InMemory provider
- **Integration tests** for the search endpoints, using `WebApplicationFactory`

```bash
cd CulturalCMS.Tests
dotnet test
```

> The integration tests exercise the full HTTP pipeline against a running database, so make sure `docker compose up --build` is running first.

---

## Architecture Decisions

### Metadata

Each item supports dynamic metadata via a **relational Key-Value table** (`ItemMetadata`, with a foreign key to `CulturalItem`) rather than a JSONB column. This allows server-side filtering and search over metadata using ordinary SQL queries, with an index on the key/value pair, without needing special JSON query syntax.

**Tags** have no separate mechanism — they are implemented as ordinary metadata entries with a fixed `Key = "Tag"`, which allows multiple tags per item (each stored as its own row with the same key).

### Search

Metadata search accepts an optional `MetadataKey` and/or `MetadataValue`:
- **Both together** → exact match on a specific property (e.g. Material = Bronze)
- **Value only** → find the value under **any** key of the item (e.g. find items that mention "Marble" anywhere, regardless of property)
- **Key only** → find items that **have** the given property recorded, regardless of value

This flexibility keeps search user-friendly: a user doesn't need to know the exact property name before searching.

### Audit Trail

Every action (Create, Update, Delete, status changes) is recorded automatically in the `AuditLogs` table, with a full before/after JSON snapshot (`OldValues`/`NewValues`) and the list of changed fields (`ChangedColumns`). The frontend timeline (visible to Curators/Admins on each item's page) parses this data and renders human-readable changes (e.g. "Title: Old Title → New Title"), including metadata additions and removals.

### Roles & Permissions

Three roles (Admin / Curator / Contributor), stored as a relational entity (`Role`) referenced by `User` via a foreign key — which allows future extension without a schema change. The item workflow follows a state machine: `Draft → ForReview → Published`, with a `Reject` transition that returns an item to `Draft`.

---

## Code Manual

### Request Flow

Example: a Contributor creates a new cultural item.

1. The frontend sends `POST /api/v1/cultural-items` with the form data and the user's JWT.
2. The request passes through the middleware pipeline: CORS check, JWT authentication, role-based authorization, and rate limiting.
3. `CulturalItemsController` contains no logic — it receives the DTO and forwards it to the appropriate service through a shared `IApplicationService` that acts as a single entry point to all services.
4. `CulturalItemService` does the real work: it checks the user's permissions, maps the DTO to an entity via AutoMapper, sets the initial status to Draft, and prepares the audit-log entry.
5. The service calls the repository through `IUnitOfWork` to persist both the item and its audit-log entry together, so either both succeed or neither does.
6. The saved entity is mapped to a read-only DTO before being returned, so unnecessary fields are never exposed.
7. On error, nothing is handled locally in the service or controller — a specific exception type is thrown and caught centrally by `GlobalExceptionHandler`, which returns it in a standardized RFC 7807 shape.
8. The controller returns JSON with the appropriate HTTP status code.

### The Layers

- **`CulturalCMS.Domain`** — Entities, Enums, custom Exceptions. No dependency on the other layers.
- **`CulturalCMS.Application`** — DTOs, Services (business logic), Interfaces, Query Builders, AutoMapper configuration.
- **`CulturalCMS.Infrastructure`** — EF Core `DbContext`, Repositories, automatic audit-field population, image storage.
- **`CulturalCMS.API`** — Controllers, middleware pipeline, `Program.cs` (service configuration).

### Key Patterns

**Repository & Unit of Work.** Each entity has its own repository, accessed through a shared `IUnitOfWork`. This guarantees that changes which must complete together (e.g. an item plus its audit-log entry) never leave the database in an inconsistent, half-written state.

**Central Error Handling.** No error is caught locally in a controller or service — every expected problem is expressed as an exception and handled in a single, central place.

**JWT Authentication.** On login, the user receives a token carrying their identity and role — this accompanies every subsequent request, so they never resend their credentials.

**Two-Layer Validation.** Input is validated both on the frontend (immediate user feedback) and independently on the backend (security, even if the frontend is bypassed).

### Where to Find Specific Things

| I want to see... | Look in... |
|---|---|
| Item state-transition rules | `CulturalItemService.cs` — `SubmitItemAsync`, `ApproveItemAsync`, `RejectItemAsync` |
| Search filtering/sorting logic | `CulturalItemQueryBuilder.cs` |
| How change history is recorded | `CulturalItemService.cs` (`AuditLog` creation) and frontend `auditFormat.ts` |
| Each role's permissions per action | `[Authorize(Roles = ...)]` above each controller method |
| Field validation rules | Backend: Data Annotations on the DTOs. Frontend: `schemas/*.ts` |
| Full documentation of each endpoint | Swagger UI at `http://localhost:8080/swagger` |

---

## Additional Features

- **Caching** — `IMemoryCache` on search results (public + all-statuses), with automatic invalidation whenever an item is created, updated, or changes status.
- **Automated testing** — unit tests (business logic and query filtering) with xUnit v3 + NSubstitute, and integration tests on the search endpoints with `WebApplicationFactory`.
- **Rate limiting** — protection middleware on the auth and search endpoints.
- **API documentation** — Swagger/OpenAPI with XML comments and JWT bearer support.

---

## Project Structure

**`CulturalCMS.API/`**
- `Controllers/` — `AuthController`, `CulturalItemsController`, `UsersController`, `ImagesController`, `AuditLogsController`
- `Helpers/` — `GlobalExceptionHandler` (RFC 7807), `AuthorizeOperationFilter` (Swagger)
- `Program.cs` — entry point, DI container, middleware pipeline

**`CulturalCMS.Application/`**
- `BusinessServices/` — `CulturalItemService`, `UserService`, `AuthService`, `AuditLogService`
- `DTO/` — Create/Update/ReadOnly DTOs per entity (with Data Annotations)
- `Interfaces/` — `IUnitOfWork`, service & repository interfaces
- `SearchQueries/` — `ItemSearchQuery`
- `Common/` — `PaginatedResult<T>`
- `Configuration/` — AutoMapper profile

**`CulturalCMS.Domain/`**
- `Entities/` — `CulturalItem`, `ItemMetadata`, `User`, `Role`, `AuditLog`, `BaseEntity`
- `Enums/` — `ItemStatus`, `AuditAction`
- `Exceptions/` — `EntityNotFoundException`, `EntityForbiddenException`, `InvalidItemStateException`, and others

**`CulturalCMS.Infrastructure/`**
- `Data/` — `CulturalDbContext`, `DbSeeder`, `Migrations/`
- `Repositories/` — `BaseRepository`, `CulturalItemRepository`, `UserRepository`, `RoleRepository`, `AuditLogRepository`, `UnitOfWork`
- `QueryBuilders/` — `CulturalItemQueryBuilder` (search filtering/sorting)
- `Interceptors/` — `AuditInterceptor` (auto-populates `CreatedAt`/`UpdatedAt`)
- `FileStorage/` — image storage handling
- `Security/` — password hashing (BCrypt)

**`CulturalCMS.Tests/`**
- `Services/` — unit tests for business logic (`CulturalItemServiceTests`, `CulturalItemServiceUpdateTests`, `AuthServiceTests`)
- `QueryBuilders/` — unit tests for search/filtering
- `Integration/` — end-to-end tests on the search endpoints (`WebApplicationFactory`)
- `TestHelpers/` — `TestDbContextFactory` (InMemory DB setup)

**`cultural-cms-frontend/src/`**
- `features/auth/` — login/signup, `AuthProvider`, `ProtectedRoute`
- `features/culturalItems/` — CRUD, search, audit trail (the largest feature)
  - `components/` — `ItemCard`, `ItemTable`, `SearchFilters`, `CulturalItemForm`, `MetadataEditor`, `AuditTimeline`, and others
  - `hooks/` — `useItemSearch`
  - `utils/` — `auditFormat.ts` (turns audit logs into readable form)
  - `schemas/` — zod validation schemas
  - `types/` — `domain.ts` (data models), `props.ts` (component props)
- `features/users/` — user/role management (Admin only)
- `shared/` — `ui/`, `layout/`, `api/` (axios client), `utils/`, shared types
- `locales/el.ts` — dictionary for all UI text (i18n)
