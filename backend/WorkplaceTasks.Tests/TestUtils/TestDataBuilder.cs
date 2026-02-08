using WorkplaceTasks.Api.Domain.Entities;
using WorkplaceTasks.Api.Domain.Enums;

namespace WorkplaceTasks.Tests.TestUtils;

public class TestDataBuilder
{
    public static User CreateUser(string? email = null, Role role = Role.Member)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email ?? $"{Guid.NewGuid()}@example.com",
            PasswordHash = "hashed_password",
            Role = role
        };
    }

    public static TaskItem CreateTask(Guid createdByUserId, string? title = null, WorkplaceTasks.Api.Domain.Enums.TaskStatus status = WorkplaceTasks.Api.Domain.Enums.TaskStatus.Pending)
    {
        return new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = title ?? "Test Task",
            Description = "Test Description",
            Status = status,
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
