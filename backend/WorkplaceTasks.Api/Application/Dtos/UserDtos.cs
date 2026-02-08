using WorkplaceTasks.Api.Domain.Entities;

namespace WorkplaceTasks.Api.Application.Dtos;

/// <summary>
/// User data transfer object
/// </summary>
public record UserDto(
    Guid Id,
    string Email,
    string Role,
    int TaskCount
);

/// <summary>
/// Request to update a user's role
/// </summary>
public record UpdateUserRoleDto(Role Role);

/// <summary>
/// Request to create a new user (admin only)
/// </summary>
public record CreateUserDto
{
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.EmailAddress]
    public string Email { get; init; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.MinLength(6)]
    public string Password { get; init; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required]
    public Role Role { get; init; } = Role.Member;
}

/// <summary>
/// Request to update a user's details
/// </summary>
public record UpdateUserDto
{
    [System.ComponentModel.DataAnnotations.EmailAddress]
    public string? Email { get; init; }

    [System.ComponentModel.DataAnnotations.MinLength(6)]
    public string? Password { get; init; }
}
