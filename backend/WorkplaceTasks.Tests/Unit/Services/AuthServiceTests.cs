using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using WorkplaceTasks.Api.Application.Dtos;
using WorkplaceTasks.Api.Application.Services;
using WorkplaceTasks.Api.Auth;
using WorkplaceTasks.Api.Domain.Entities;
using WorkplaceTasks.Api.Infrastructure.Data;
using WorkplaceTasks.Tests.TestUtils;
using Xunit;

namespace WorkplaceTasks.Tests.Unit.Services;

public class AuthServiceTests
{
    private readonly AppDbContext _dbContext;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new AppDbContext(options);
        
        _tokenServiceMock = new Mock<ITokenService>();
        _service = new AuthService(_dbContext, _tokenServiceMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsLoginResponse()
    {
        // Arrange
        var password = "Password123!";
        var user = TestDataBuilder.CreateUser("auth@example.com");
        user.PasswordHash = HashPassword(password);
        
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<User>()))
            .Returns("fake-jwt-token");

        var request = new LoginRequest { Email = user.Email, Password = password };

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("fake-jwt-token");
        result.Email.Should().Be(user.Email);
        _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser("auth@example.com");
        user.PasswordHash = HashPassword("CorrectPassword");
        
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var request = new LoginRequest { Email = user.Email, Password = "WrongPassword" };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.LoginAsync(request));
    }

    [Fact]
    public async Task LoginAsync_UserNotFound_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var request = new LoginRequest { Email = "nonexistent@example.com", Password = "pw" };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.LoginAsync(request));
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
