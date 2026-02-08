using System.ComponentModel.DataAnnotations;
using WorkplaceTasks.Api.Domain.Enums;
using TaskStatus = WorkplaceTasks.Api.Domain.Enums.TaskStatus;

namespace WorkplaceTasks.Api.Application.Dtos;

public record TaskCreateRequest
{
    [Required]
    [MaxLength(100)]
    public string Title { get; init; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; init; } = string.Empty;
}

public record TaskUpdateRequest
{
    [MaxLength(100)]
    public string Title { get; init; }

    [MaxLength(500)]
    public string Description { get; init; }

    public TaskStatus? Status { get; init; }
}

public record TaskResponse(
    Guid Id,
    string Title,
    string Description,
    TaskStatus Status,
    DateTime CreatedAt,
    Guid CreatedByUserId,
    string CreatedByEmail
);

/// <summary>
/// Query parameters for paginated task listing with optional status filter.
/// </summary>
public record TaskQueryRequest
{
    /// <summary>
    /// Page number (1-based). Default is 1.
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Number of items per page. Default is 10, maximum is 50.
    /// </summary>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Optional filter by task status.
    /// </summary>
    public TaskStatus? Status { get; init; }
}

/// <summary>
/// Paginated response wrapper for any list of items.
/// </summary>
/// <typeparam name="T">The type of items in the list</typeparam>
public record PagedResponse<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);
