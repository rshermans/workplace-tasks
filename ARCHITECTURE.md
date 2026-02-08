# 🏗️ Architecture Decisions

This document details the architectural decisions and directory structure of the WorkPlace Tasks project.

## 📁 Directory Layout / Monorepo

The project follows a strict Monorepo structure with clear separation of concerns:

```
workplace-tasks/
├── README.md               # Entry point
├── docker-compose.yml      # Infrastructure (PostgreSQL)
├── ARCHITECTURE.md         # This document
├── TESTING.md              # Testing strategy
├── API-CONTRACT.md         # API Specification
├── docs/                   # Supporting documentation
├── backend/
│   └── WorkplaceTasks.Api/ # .NET 8 Web API
│       ├── Application/    # Services & DTOs
│       ├── Controllers/    # Thin HTTP Interface
│       ├── Domain/         # Entities & Policies (RBAC)
│       └── Infrastructure/ # EF Core & Repositories
└── frontend/
    └── workplace-tasks-web/# React + Vite + TypeScript (In workpace-tasks directory)
        ├── src/
        │   ├── api/        # Axios Client
        │   ├── auth/       # Auth Context & Hooks
        │   ├── components/ # Reusable UI
        │   └── pages/      # View Composition
```

---

## ⚙️ Backend Architecture (Clean Architecture)

We follow a layered approach to ensure maintainability and testability.

### 1. Presentation Layer (API)
*   **Location**: `backend/WorkplaceTasks.Api/Controllers/`
*   **Responsibility**: Handling HTTP requests/responses, model binding, and basic validation.
*   **Rule**: Controllers must be thin. They delegate all business logic to the Service layer.

### 2. Application Layer (Services)
*   **Location**: `backend/WorkplaceTasks.Api/Application/Services/`
*   **Responsibility**: Orchestrating business flows, enforcing authorization, and mapping domain entities to DTOs.
*   **Pattern**: Uses Dependency Injection to access Repositories and `IAuthorizationService`.

### 3. Domain Layer (Entities & Rules)
*   **Location**: `backend/WorkplaceTasks.Api/Domain/`
*   **Responsibility**: Core data structures (`Task`, `User`, `Role`) and critical business rules.
*   **RBAC**: Security policies (e.g., `TaskAuthorizationPolicy`) are defined here to ensure central enforcement.

### 4. Infrastructure Layer (Data)
*   **Location**: `backend/WorkplaceTasks.Api/Infrastructure/`
*   **Responsibility**: Persistence logic using EF Core, Database Migrations, and communication with PostgreSQL.

---

## ⚛️ Frontend Architecture (React)

The frontend is built for simplicity, performance, and a premium user experience.

### 1. Auth Context & Security
*   A global `AuthContext` manages user sessions and JWT tokens.
*   Tokens are persisted in `localStorage`.
*   Private routes are protected via higher-order components or layout wrappers.

### 2. API Service Layer
*   Centralized `axios` instance configured with interceptors to automatically inject the JWT token and handle `401 Unauthorized` errors globally.

### 3. Visual RBAC
*   The UI is "permission-aware." It fetches the user's role and ownership info to dynamically hide or disable restricted actions (Edit/Delete).
*   **Note**: This is for UX only; security is always re-validated on the backend.

---

## 🗄️ Persistence Strategy

We use **EF Core** with **PostgreSQL** as the primary database. 
The system supports flexible configurations via `.env` (Supabase, Local Postgres, or SQLite for rapid testing).
Migrations are managed strictly via the CLI to ensure consistency across environments.
