# 🏁 Getting Started — Manual Initialization

This guide explains the step-by-step process of initializing the application manually. This is useful for understanding how the components interact.

---

## 1. Prerequisites
- **.NET SDK 8.0**: To compile and run the backend.
- **Node.js 18+**: To run the frontend dev server.
- **PostgreSQL**: A running instance (local or Docker).

---

## 2. Infrastructure (The Database)
The system requires a PostgreSQL database named `workplace_tasks`.

**Manual Step**:
```bash
# If using Docker
docker run --name workplace-db -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres
```

---

## 3. Backend Setup
1. **Migrations**: Before running, we must ensure the schema is applied.
   ```bash
   cd backend/WorkplaceTasks.Api
   dotnet ef database update
   ```
2. **Run**:
   ```bash
   dotnet run
   ```
   *The API will be available at `http://localhost:5000` (or `5001` for HTTPS).*
   *Swagger docs: `http://localhost:5000/swagger`*

---

## 4. Frontend Setup
1. **Dependencies**: React needs its node modules.
   ```bash
   cd frontend/workplace-tasks-web
   npm install
   ```
2. **Run**:
   ```bash
   npm run dev
   ```
   *The UI will be available at `http://localhost:5173`.*

---

## 💡 Pro Tip
Use our `start.bat` script in the root directory to automate all these steps in one go!
