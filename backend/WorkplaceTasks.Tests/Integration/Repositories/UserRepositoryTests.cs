using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WorkplaceTasks.Api.Domain.Entities;
using WorkplaceTasks.Api.Infrastructure.Data;
using WorkplaceTasks.Api.Infrastructure.Repositories;
using WorkplaceTasks.Tests.TestUtils;
using Xunit;

namespace WorkplaceTasks.Tests.Integration.Repositories;

public class UserRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ValidUser_AddsToDatabase()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser("new@example.com");

        // Act
        await _repository.AddAsync(user);

        // Assert
        var dbUser = await _context.Users.FindAsync(user.Id);
        dbUser.Should().NotBeNull();
        dbUser!.Email.Should().Be("new@example.com");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        // Arrange
        _context.Users.Add(TestDataBuilder.CreateUser("u1@example.com"));
        _context.Users.Add(TestDataBuilder.CreateUser("u2@example.com"));
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_ModifiesUser()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser("old@example.com");
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        user.Email = "updated@example.com";
        await _repository.UpdateAsync(user);

        // Assert
        var dbUser = await _context.Users.FindAsync(user.Id);
        dbUser!.Email.Should().Be("updated@example.com");
    }

    [Fact]
    public async Task DeleteAsync_RemovesUser()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser();
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(user.Id);

        // Assert
        var dbUser = await _context.Users.FindAsync(user.Id);
        dbUser.Should().BeNull();
    }
}
