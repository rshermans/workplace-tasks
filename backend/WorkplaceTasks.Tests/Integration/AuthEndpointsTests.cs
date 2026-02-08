using System.Net;
using System.Net.Http.Json;
using WorkplaceTasks.Api.Application.Dtos;
using WorkplaceTasks.Tests.TestUtils;
using Xunit;

namespace WorkplaceTasks.Tests.Integration;

public class AuthEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_With_Valid_Credentials_Returns_Token()
    {
        // Credentials from DbSeeder
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest { Email = "admin@example.com", Password = "Password123!" });
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginResponse);
        Assert.NotEmpty(loginResponse!.Token);
        Assert.Equal("Admin", loginResponse.Role);
    }

    [Fact]
    public async Task Login_With_Invalid_Credentials_Returns_401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest { Email = "admin@example.com", Password = "WrongPassword" });
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
