using System.ComponentModel.DataAnnotations;
using TaskStatusEnum = WorkplaceTasks.Api.Domain.Enums.TaskStatus;

namespace WorkplaceTasks.Api.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public TaskStatusEnum Status { get; set; } = TaskStatusEnum.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Ownership
    public Guid CreatedByUserId { get; set; }
    public User? CreatedBy { get; set; }
}
