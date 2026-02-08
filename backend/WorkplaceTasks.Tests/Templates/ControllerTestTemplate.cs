using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WorkplaceTasks.Api.Application.Dtos;
using WorkplaceTasks.Tests.TestUtils;
using Xunit;

namespace WorkplaceTasks.Tests.Templates;

/// <summary>
/// This template demonstrates how to write integration tests for API Controllers.
/// It uses CustomWebApplicationFactory to launch the API with an in-memory database.
/// </summary>
public class ControllerTestTemplate : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ControllerTestTemplate(CustomWebApplicationFactory factory)
    {
        // One client per test class
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Endpoint_Action_Scenario_ExpectedResult()
    {
        // 1. Authenticate (using a helper or directly)
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", 
            new LoginRequest { Email = "admin@example.com", Password = "Password123!" });
        var auth = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>(CustomWebApplicationFactory.JsonOptions);
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.Token);

        // 2. Perform Request
        var response = await _client.GetAsync("/api/tasks/paged");

        // 3. Assert HTTP Status and Content
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var body = await response.Content.ReadFromJsonAsync<PagedResponse<TaskResponse>>(CustomWebApplicationFactory.JsonOptions);
        Assert.NotNull(body);
    }
}
