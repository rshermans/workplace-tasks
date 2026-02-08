# Workplace Tasks - Test Suite

This project contains the comprehensive test suite for the Workplace Tasks backend, following the project's testing rules (R5).

## Test Structure

The tests are organized into three main categories:

1.  **Unit Tests (`/Unit`)**: 
    - **Services**: Business logic validation using Moq and FluentAssertions.
    - **Policies**: Pure function validation for RBAC (Role-Based Access Control).
2.  **Integration Tests (`/Integration`)**:
    - **Repositories**: Database interaction tests using an In-Memory database.
    - **Endpoints**: Full HTTP request/response cycle tests using `CustomWebApplicationFactory`.

## Tools & Libraries

- **xUnit**: The core testing framework.
- **Moq**: Used for mocking service dependencies and repositories.
- **FluentAssertions**: For readable and expressive assertion statements.
- **Microsoft.AspNetCore.Mvc.Testing**: For hosting the API in-memory during integration tests.

## Running Tests

### Using .NET CLI
Run all tests:
```bash
dotnet test
```

Run specific tests (e.g., only Unit tests):
```bash
dotnet test --filter "FullyQualifiedName~WorkplaceTasks.Tests.Unit"
```

### Coverage
To collect code coverage (requires `coverlet.collector`):
```bash
dotnet test /p:CollectCoverage=true
```

## Naming Conventions
Follow the pattern: `[MethodName]_[Scenario]_[ExpectedResult]`
Example: `DeleteAsync_AsAdmin_DeletesTask`

## Adding New Tests
- **Service Tests**: Use the `ServiceTestTemplate.cs` in the `Templates` folder.
- **Endpoint Tests**: Use the `ControllerTestTemplate.cs` in the `Templates` folder.
- **TestDataBuilder**: Use the `TestDataBuilder` utility to create consistent test entities.

## Best Practices
- **Isolation**: Each test class uses a unique In-Memory database Guid to prevent state leakage.
- **RBAC**: Always include a "negative" test case for unauthorized roles (403 Forbidden).
- **Mocks**: Mock only external dependencies or complex infrastructure.
