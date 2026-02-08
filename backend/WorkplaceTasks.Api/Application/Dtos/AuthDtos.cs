using System.ComponentModel.DataAnnotations;
using WorkplaceTasks.Api.Domain.Entities;

namespace WorkplaceTasks.Api.Application.Dtos;

public record LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}

public record LoginResponse(string Token, string Role, string Email);
