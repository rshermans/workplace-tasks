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
using TaskStatus = WorkplaceTasks.Api.Domain.Enums.TaskStatus;

namespace WorkplaceTasks.Tests.Unit.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly AppDbContext _dbContext;
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        
        // We use In-Memory for DbContext to handle User lookups correctly
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new AppDbContext(options);
        
        _service = new TaskService(_repositoryMock.Object, _dbContext);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsTaskResponse()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser(role: Role.Admin);
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var request = new TaskCreateRequest { Title = "Test Task", Description = "Test" };
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<TaskItem>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(request, user.Id);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(request.Title);
        result.CreatedByEmail.Should().Be(user.Email);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_UserNotFound_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var request = new TaskCreateRequest { Title = "Test Task" };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
            _service.CreateAsync(request, Guid.NewGuid()));
    }

    [Fact]
    public async Task GetAllAsync_AsAdmin_ReturnsAllTasks()
    {
        // Arrange
        var admin = TestDataBuilder.CreateUser(role: Role.Admin);
        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

        var tasks = new List<TaskItem>
        {
            TestDataBuilder.CreateTask(admin.Id, "Task 1"),
            TestDataBuilder.CreateTask(Guid.NewGuid(), "Task 2")
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);

        // Act
        var result = await _service.GetAllAsync(admin.Id);

        // Assert
        result.Should().HaveCount(2);
        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_AsMember_ReturnsOnlyOwnTasks()
    {
        // Arrange
        var member = TestDataBuilder.CreateUser(role: Role.Member);
        _dbContext.Users.Add(member);
        await _dbContext.SaveChangesAsync();

        var tasks = new List<TaskItem> { TestDataBuilder.CreateTask(member.Id, "My Task") };
        _repositoryMock.Setup(r => r.GetByUserIdAsync(member.Id)).ReturnsAsync(tasks);

        // Act
        var result = await _service.GetAllAsync(member.Id);

        // Assert
        result.Should().HaveCount(1);
        _repositoryMock.Verify(r => r.GetByUserIdAsync(member.Id), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_UpdatesTask()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser(role: Role.Admin);
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var task = TestDataBuilder.CreateTask(user.Id);
        _repositoryMock.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var request = new TaskUpdateRequest { Title = "Updated Title", Status = TaskStatus.InProgress };

        // Act
        await _service.UpdateAsync(task.Id, request, user.Id);

        // Assert
        task.Title.Should().Be(request.Title);
        task.Status.Should().Be(request.Status);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_AsAdmin_DeletesTask()
    {
        // Arrange
        var admin = TestDataBuilder.CreateUser(role: Role.Admin);
        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

        var task = TestDataBuilder.CreateTask(Guid.NewGuid());
        _repositoryMock.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        // Act
        await _service.DeleteAsync(task.Id, admin.Id);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(task), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
