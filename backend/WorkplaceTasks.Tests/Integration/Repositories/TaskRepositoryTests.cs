using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WorkplaceTasks.Api.Domain.Entities;
using WorkplaceTasks.Api.Infrastructure.Data;
using WorkplaceTasks.Api.Infrastructure.Repositories;
using WorkplaceTasks.Tests.TestUtils;
using Xunit;

namespace WorkplaceTasks.Tests.Integration.Repositories;

public class TaskRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly TaskRepository _repository;

    public TaskRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _repository = new TaskRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ValidTask_AddsToDatabase()
    {
        // Arrange
        var task = TestDataBuilder.CreateTask(Guid.NewGuid(), "New Task");

        // Act
        await _repository.AddAsync(task);
        await _repository.SaveChangesAsync();

        // Assert
        var dbTask = await _context.Tasks.FindAsync(task.Id);
        dbTask.Should().NotBeNull();
        dbTask!.Title.Should().Be("New Task");
    }

    [Fact]
    public async Task GetByIdAsync_ExistingTask_ReturnsTask()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser();
        _context.Users.Add(user);
        var task = TestDataBuilder.CreateTask(user.Id);
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(task.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(task.Id);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsOnlyUserTasks()
    {
        // Arrange
        var user1 = TestDataBuilder.CreateUser();
        var user2 = TestDataBuilder.CreateUser();
        _context.Users.AddRange(user1, user2);
        
        _context.Tasks.Add(TestDataBuilder.CreateTask(user1.Id));
        _context.Tasks.Add(TestDataBuilder.CreateTask(user1.Id));
        _context.Tasks.Add(TestDataBuilder.CreateTask(user2.Id));
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUserIdAsync(user1.Id);

        // Assert
        result.Should().HaveCount(2);
        result.All(t => t.CreatedByUserId == user1.Id).Should().BeTrue();
    }

    [Fact]
    public async Task GetPagedAsync_CorrectlyPaginates()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser();
        _context.Users.Add(user);
        for (int i = 0; i < 15; i++)
        {
            _context.Tasks.Add(TestDataBuilder.CreateTask(user.Id, $"Task {i}"));
        }
        await _context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await _repository.GetPagedAsync(2, 5, null);

        // Assert
        totalCount.Should().Be(15);
        items.Should().HaveCount(5);
    }

    [Fact]
    public async Task DeleteAsync_RemovesTask()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser();
        _context.Users.Add(user);
        var task = TestDataBuilder.CreateTask(user.Id);
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(task);
        await _repository.SaveChangesAsync();

        // Assert
        var result = await _context.Tasks.FindAsync(task.Id);
        result.Should().BeNull();
    }
}
