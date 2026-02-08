using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WorkplaceTasks.Api.Application.Dtos;
using WorkplaceTasks.Api.Domain.Entities;
using WorkplaceTasks.Tests.TestUtils;
using Xunit;

namespace WorkplaceTasks.Tests.Integration;

public class UsersEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UsersEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> AuthenticateAsync(string email, string password)
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest { Email = email, Password = password });
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return data!.Token;
    }

    [Fact]
    public async Task Admin_Can_CRUD_Users()
    {
        // 1. Auth as Admin
        var token = await AuthenticateAsync("admin@example.com", "Password123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 2. Create User
        var createRequest = new CreateUserDto { Email = "newuser@example.com", Password = "Password123", Role = Role.Member };
        var createResponse = await _client.PostAsJsonAsync("/api/users", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var user = await createResponse.Content.ReadFromJsonAsync<UserDto>(CustomWebApplicationFactory.JsonOptions);

        // 3. Get All Users
        var getResponse = await _client.GetAsync("/api/users");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var users = await getResponse.Content.ReadFromJsonAsync<IEnumerable<UserDto>>(CustomWebApplicationFactory.JsonOptions);
        Assert.Contains(users!, u => u.Email == "newuser@example.com");

        // 4. Update Role
        var updateRoleRequest = new UpdateUserRoleDto(Role.Manager);
        var updateResponse = await _client.PutAsJsonAsync($"/api/users/{user!.Id}/role", updateRoleRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        // 5. Delete User
        var deleteResponse = await _client.DeleteAsync($"/api/users/{user.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task Member_Cannot_Access_Users_Endpoint()
    {
        // 1. Auth as Member
        var token = await AuthenticateAsync("member@example.com", "Password123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 2. Try Get Users
        var response = await _client.GetAsync("/api/users");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
