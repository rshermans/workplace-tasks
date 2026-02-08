# Architecture & Directory Structure

This project follows a strict Monorepo structure with clear separation of concerns.

## Directory Layout / Monorepo

```
workplace-tasks/
├── README.md               # Entry point
├── docker-compose.yml      # Infrastructure (PostgreSQL)
├── docs/                   # Documentation (RBAC Matrix, Manual, Rules)
├── backend/
│   └── WorkplaceTasks.Api/ # .NET 8 Web API
│       ├── Controllers/    # Thin HTTP Interface
│       ├── Services/       # Business Logic & RBAC
│       ├── Domain/         # Entities & Policies
│       ├── Infrastructure/ # EF Core & Repositories
│       └── Tests/          # Unit & Integration Tests
└── frontend/
    └── workplace-tasks-web/# React + Vite + TypeScript
        ├── src/
        │   ├── api/        # Axios Client
        │   ├── auth/       # Auth Context & Hooks
        │   ├── components/ # Reusable UI
        │   └── pages/      # View Composition
```

## Backend Architecture (Clean Architecture)

### 1. Presentation Layer (API)
*   **Responsibility**: Parsing HTTP requests, returning HTTP responses.
*   **Rule**: NO Business Logic here. Just call Service -> Return DTO.

### 2. Application Layer (Services)
*   **Responsibility**: Orchestrating business flows, validating input, enforcing Authorization.
*   **Pattern**: Services inject Repositories and `IAuthorizationService`.

### 3. Domain Layer (Entities & Rules)
*   **Responsibility**: Defining the data structures (`Task`, `User`, `Role`) and core business rules.
*   **RBAC**: Defined via `TaskAuthorizationPolicy`.

### 4. Infrastructure Layer (Data)
*   **Responsibility**: Database communication (EF Core), Migrations, External services.

## Frontend Architecture (React)

### 1. Auth Context
*   Global state management for `User` and `Token`.
*   Persists token in `localStorage`.

### 2. API Service
*   Centralized `axios` instance with interceptors.
*   Auto-adds `Authorization: Bearer ...` header.

### 3. Smart vs. Dumb Components
*   **Pages (Smart)**: Fetch data, manage state, pass props.
*   **Components (Dumb)**: Render UI based on props.
