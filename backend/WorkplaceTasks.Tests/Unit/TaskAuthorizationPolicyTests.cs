using WorkplaceTasks.Api.Domain.Entities;
using WorkplaceTasks.Api.Domain.Policies;
using Xunit;

namespace WorkplaceTasks.Tests.Unit;

public class TaskAuthorizationPolicyTests
{
    private readonly User _admin = new() { Id = Guid.NewGuid(), Role = Role.Admin };
    private readonly User _manager = new() { Id = Guid.NewGuid(), Role = Role.Manager };
    private readonly User _member = new() { Id = Guid.NewGuid(), Role = Role.Member };

    [Fact]
    public void Admin_Can_Delete_Anyone_Task()
    {
        var task = new TaskItem { CreatedByUserId = _member.Id };
        Assert.True(TaskAuthorizationPolicy.CanDelete(_admin, task));
    }

    [Fact]
    public void Manager_Can_Delete_Own_Task()
    {
        var task = new TaskItem { CreatedByUserId = _manager.Id };
        Assert.True(TaskAuthorizationPolicy.CanDelete(_manager, task));
    }

    [Fact]
    public void Manager_Cannot_Delete_Other_Task()
    {
        var task = new TaskItem { CreatedByUserId = _member.Id };
        Assert.False(TaskAuthorizationPolicy.CanDelete(_manager, task));
    }

    [Fact]
    public void Member_Cannot_Delete_Any_Task()
    {
        var ownTask = new TaskItem { CreatedByUserId = _member.Id };
        Assert.False(TaskAuthorizationPolicy.CanDelete(_member, ownTask));
    }

    [Fact]
    public void Member_Can_Update_Own_Task()
    {
        var task = new TaskItem { CreatedByUserId = _member.Id };
        Assert.True(TaskAuthorizationPolicy.CanUpdate(_member, task));
    }
    
    [Fact]
    public void Member_Cannot_Update_Other_Task()
    {
        var task = new TaskItem { CreatedByUserId = _manager.Id };
        Assert.False(TaskAuthorizationPolicy.CanUpdate(_member, task));
    }
}
