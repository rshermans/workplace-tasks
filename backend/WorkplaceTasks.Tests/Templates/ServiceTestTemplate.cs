using FluentAssertions;
using Moq;
using WorkplaceTasks.Api.Application.Dtos;
using WorkplaceTasks.Api.Application.Services;
using WorkplaceTasks.Api.Domain.Entities;
using WorkplaceTasks.Api.Infrastructure.Repositories;
using WorkplaceTasks.Tests.TestUtils;
using Xunit;

namespace WorkplaceTasks.Tests.Templates;

/// <summary>
/// This template demonstrates how to write unit tests for the Service layer.
/// It uses Moq for repository isolation and FluentAssertions for readable checks.
/// </summary>
public class ServiceTestTemplate
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly TaskService _service;

    public ServiceTestTemplate()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        
        // Arrange dependencies
        _service = new TaskService(_repositoryMock.Object, /* Provide other dependencies */ null!);
    }

    [Fact]
    public async Task MethodName_Scenario_ExpectedResult()
    {
        // 1. Arrange
        var user = TestDataBuilder.CreateUser();
        var request = new TaskCreateRequest { Title = "Template Task" };
        
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<TaskItem>()))
            .Returns(Task.CompletedTask);

        // 2. Act
        var result = await _service.CreateAsync(request, user.Id);

        // 3. Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(request.Title);
        
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Once);
    }
}
