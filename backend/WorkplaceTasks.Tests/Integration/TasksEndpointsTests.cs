using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WorkplaceTasks.Api.Application.Dtos;
using WorkplaceTasks.Tests.TestUtils;
using Xunit;

namespace WorkplaceTasks.Tests.Integration;

public class TasksEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public TasksEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<string> AuthenticateAsync(string email, string password)
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest { Email = email, Password = password });
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadFromJsonAsync<LoginResponse>(CustomWebApplicationFactory.JsonOptions);
        return data!.Token;
    }

    [Fact]
    public async Task Admin_Can_Create_And_Delete_Task()
    {
        // 1. Auth as Admin
        var token = await AuthenticateAsync("admin@example.com", "Password123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 2. Create Task
        var createRequest = new TaskCreateRequest { Title = "Admin Task", Description = "Test" };
        var createResponse = await _client.PostAsJsonAsync("/api/tasks", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var task = await createResponse.Content.ReadFromJsonAsync<TaskResponse>(CustomWebApplicationFactory.JsonOptions);

        // 3. Delete Task
        var deleteResponse = await _client.DeleteAsync($"/api/tasks/{task!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task Member_Cannot_Delete_Own_Task()
    {
        // 1. Auth as Member
        var token = await AuthenticateAsync("member@example.com", "Password123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 2. Create Task
        var createResponse = await _client.PostAsJsonAsync("/api/tasks", new TaskCreateRequest { Title = "My Task", Description = "Desc" });
        var task = await createResponse.Content.ReadFromJsonAsync<TaskResponse>(CustomWebApplicationFactory.JsonOptions);

        // 3. Try Delete
        var deleteResponse = await _client.DeleteAsync($"/api/tasks/{task!.Id}");
        Assert.Equal(HttpStatusCode.Forbidden, deleteResponse.StatusCode);
    }
    
    [Fact]
    public async Task Tasks_GetPaged_Returns_Correct_Structure()
    {
        // 1. Auth as Admin
        var token = await AuthenticateAsync("admin@example.com", "Password123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 2. Get Paged Tasks
        var response = await _client.GetAsync("/api/tasks/paged?page=1&pageSize=5");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<TaskResponse>>(CustomWebApplicationFactory.JsonOptions);
        Assert.NotNull(pagedResponse);
        Assert.True(pagedResponse.Items.Count <= 5);
        Assert.Equal(1, pagedResponse.Page);
    }

    [Fact]
    public async Task Tasks_Filter_By_Status_Works()
    {
        var token = await AuthenticateAsync("admin@example.com", "Password123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/tasks/paged?status=Pending");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<TaskResponse>>(CustomWebApplicationFactory.JsonOptions);
        Assert.All(pagedResponse!.Items, t => Assert.Equal(WorkplaceTasks.Api.Domain.Enums.TaskStatus.Pending, t.Status));
    }
}
