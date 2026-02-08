# 🧪 Testing Strategy - Workplace Tasks

Ensuring the reliability of our Role-Based Access Control (RBAC) and business logic is a top priority. Our test suite follows the **Red-Green-Refactor** pattern and adheres to Rule **R5 - Testes são Parte da Feature**.

## 🏗️ Test Pyramid

1.  **Unit Tests (Fast & Isolated)**:
    - **Policies**: 100% coverage of logic in `TaskAuthorizationPolicy`.
    - **Services**: Business logic in `TaskService`, `AuthService`, and `UserService`. We mock repositories to focus on orchestration and validation.
2.  **Integration Tests (Database & HTTP)**:
    - **Repositories**: Using EF Core In-Memory provider to verify LINQ queries, filtering, and persistence.
    - **Endpoints**: Using `WebApplicationFactory` to test the full stack, including middleware, routing, and JSON serialization.

---

## 🏃 Running the Suite

### Backend
All tests are located in `backend/WorkplaceTasks.Tests`.

```bash
# Run everything
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=normal"

# Run specific project layer
dotnet test --filter "FullyQualifiedName~Integration.Repositories"
```

### Coverage Targets
- **Policies**: 100% (Security & Ownership)
- **Services**: 80%+ (Business Logic)
- **Controllers/Endpoints**: 70%+ (Contract & RBAC)

---

## 🛠️ Tooling & Standards

- **Moq**: Used for mocking interfaces in Service tests.
- **FluentAssertions**: Used for expressive assertions: `result.Should().BeEquivalentTo(expected);`.
- **TestDataBuilder**: Centralized utility for creating test entities (Users, Tasks).
- **CustomWebApplicationFactory**: Configures the API with an isolated in-memory DB per test class.

### Naming Convention
We use the pattern: `[MethodName]_[Scenario]_[ExpectedResult]`
Example: `Admin_Can_CRUD_Users_ReturnsSuccess`

---

## 🛡️ RBAC Test Matrix

| Role | Create Task | Update Task | Delete Task | Manage Users |
|------|-------------|-------------|-------------|--------------|
| **Admin** | ✅ Yes | ✅ Yes (Any) | ✅ Yes (Any) | ✅ Yes |
| **Manager** | ✅ Yes | ✅ Yes (Any) | ✅ Only Own | ❌ No |
| **Member** | ✅ Yes | ✅ Only Own | ❌ No | ❌ No |

---

## 🔮 Future Roadmap
- **Playwright**: Automatic UI testing for the React frontend.
- **Performance**: Load testing with k6 for high-concurrency scenarios.
- **CI/CD**: Integrate `dotnet test` into GitHub Actions or local Git hooks.
