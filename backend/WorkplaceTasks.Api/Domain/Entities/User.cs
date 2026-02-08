using System.ComponentModel.DataAnnotations;

namespace WorkplaceTasks.Api.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    public Role Role { get; set; }
    
    // Navigation property
    public ICollection<TaskItem>? AssignedTasks { get; set; }
}

