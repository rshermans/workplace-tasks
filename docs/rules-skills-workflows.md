# Rules, Skills, and Workflows — Team Workplace Tasks

This document defines the working agreements, roles, and processes for the "Workplace Tasks" project. It simulates a mature team environment, ensuring consistency, quality, and speed.

## 🧠 LEVEL 1 — Rules (Constitution)

These rules are global and non-negotiable. They ensure the architectural integrity of the project.

### Global Rules
1.  **Controllers are Dumb**: Controllers must not contain business logic. They are responsible only for request parsing, response formatting, and status codes.
2.  **RBAC/Ownership in Domain**: Authorization rules (Role-Based Access Control and Ownership) live in the Domain/Service layer, never in the Controller or UI.
3.  **Global Error Handling**: All exceptions are handled by a global pipeline (Middleware/Filter), returning a consistent error envelope (Code, Message, TraceId).
4.  **Security Testing**: Every endpoint must have automated tests verifying permissions for all roles (Admin, Manager, Member).
5.  **Documentation First**: The README is part of the product and must be kept up-to-date with setup instructions and architectural decisions.

### Backend Rules (.NET)
6.  **EF Core & Migrations**: Database changes are managed exclusively via Entity Framework Core Migrations.
7.  **Dependency Injection**: All dependencies are injected via interfaces to facilitate testing and loose coupling.
8.  **DTOs & Validation**: Data Transfer Objects (DTOs) are used for all inputs and outputs. Validation logic (DataAnnotations or FluentValidation) is mandatory.
9.  **Security Standard**: JWT (JSON Web Tokens) with Claims and Roles is the standard for authentication and authorization.

---

## 🧩 LEVEL 2 — Skills (Roles & Responsibilities)

These "skills" represent the specialized hats worn by team members during development.

### Backend (.NET/C#)
*   **API Architect Skill**: Defines the layered architecture (Controllers → Services → Repositories → EF Core). Responsible for DTO mapping and clean code structure.
*   **RBAC & Ownership Guardian Skill**: Enforces the authorization rules:
    *   **Admin**: Full access.
    *   **Manager**: Can delete only their own tasks.
    *   **Member**: Can update only their own tasks.
*   **Error & Validation Skill**: Ensures robust request validation and standardized error responses (400, 401, 403, 404, 500).
*   **Testing Skill**: Writes Unit Tests for services and Integration Tests for API endpoints and database interactions.

### Frontend (React)
*   **UX Minimal & Clear Skill**: Focuses on a clean, functional interface (List, Form, Actions) without unnecessary "fluff".
*   **RBAC Visual Policy Skill**: Implements visual cues for permissions (showing/hiding buttons based on role/ownership), acting as a mirror to the backend logic.
*   **API Client Skill**: Manages token storage, request interception, and centralized handling of loading and error states.

### Database (PostgreSQL)
*   **Data Sentinel Skill**: Manages secure migrations, schema consistency, integrity checks, and ensures rollback/backup strategies are considered.

---

## 🔁 LEVEL 3 — Workflows (Execution Strategy)

Standardized procedures to deliver high-quality features quickly.

### Workflow A — "Vertical Slice" (Current Focus)
**Goal**: Deliver a fully functional feature across all layers.
1.  **Auth**: Seed Users/Roles and implement JWT issuance.
2.  **Entity**: Create Task entity and generate Migration.
3.  **Endpoint**: Implement `POST /tasks` with full RBAC and Tests.
4.  **Expand**: Implement GET/PUT/DELETE only after the critical path is secure.

### Workflow B — "RBAC Test Matrix" (Verification)
**Goal**: Verify authorization logic exhaustively.
Create and maintain a matrix in the README:
*   **Admin**: CRUD any task ✅
*   **Manager**: Delete own ✅ / Delete other ❌ (403)
*   **Member**: Update own ✅ / Update other ❌ (403)
*   Include test credentials and token generation steps.

### Workflow C — "Docs Sprint" (Delivery)
**Goal**: Ensure the project is hand-off ready.
1.  **Setup Guide**: How to run API and React apps (Docker vs. Local).
2.  **Decisions**: Document technical choices (Architecture, Libraries).
3.  **Manual**: functional guide for testing RBAC.
4.  **Roadmap**: Future improvements (Pagination, Filters, CI/CD).

---

## checklist for Pull Requests (PR)

- [ ] **Rules Check**:
    - [ ] Controller is dumb?
    - [ ] Business logic in Service?
    - [ ] Global Error Handling used?
- [ ] **Skills Check**:
    - [ ] Unit Tests for Service logic?
    - [ ] Integration Tests for RBAC?
    - [ ] Frontend handles Loading/Error states?
- [ ] **Workflow Check**:
    - [ ] RBAC Matrix updated?
    - [ ] Documentation updated?
