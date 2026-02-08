using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using WorkplaceTasks.Api.Application.Dtos;
using WorkplaceTasks.Api.Application.Services;
using WorkplaceTasks.Api.Domain.Entities;
using WorkplaceTasks.Api.Infrastructure.Data;
using WorkplaceTasks.Api.Infrastructure.Repositories;
using WorkplaceTasks.Tests.TestUtils;
using Xunit;

namespace WorkplaceTasks.Tests.Unit.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly AppDbContext _dbContext;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new AppDbContext(options);
        
        _service = new UserService(_repositoryMock.Object, _dbContext);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            TestDataBuilder.CreateUser("user1@example.com"),
            TestDataBuilder.CreateUser("user2@example.com")
        };
        _dbContext.Users.AddRange(users);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(u => u.Email == "user1@example.com");
        result.Should().Contain(u => u.Email == "user2@example.com");
    }

    [Fact]
    public async Task CreateAsync_ValidData_CreatesUser()
    {
        // Arrange
        var dto = new CreateUserDto { Email = "new@example.com", Password = "Password123", Role = Role.Member };
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(dto.Email);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_DuplicateEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var existingUser = TestDataBuilder.CreateUser("duplicate@example.com");
        _dbContext.Users.Add(existingUser);
        await _dbContext.SaveChangesAsync();

        var dto = new CreateUserDto { Email = "duplicate@example.com", Password = "pw" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task UpdateRoleAsync_ValidRole_UpdatesRole()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser(role: Role.Member);
        _repositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var dto = new UpdateUserRoleDto(Role.Admin);

        // Act
        var result = await _service.UpdateRoleAsync(user.Id, dto);

        // Assert
        result.Role.Should().Be(Role.Admin.ToString());
        user.Role.Should().Be(Role.Admin);
        _repositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ExistingUser_DeletesUser()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser();
        _repositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
        _repositoryMock.Setup(r => r.DeleteAsync(user.Id)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(user.Id);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(user.Id), Times.Once);
    }
}
