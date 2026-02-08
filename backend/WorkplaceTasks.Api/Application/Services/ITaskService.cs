using WorkplaceTasks.Api.Application.Dtos;

namespace WorkplaceTasks.Api.Application.Services;

/// <summary>
/// Service interface for task-related business logic.
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="request">The task creation request</param>
    /// <param name="userId">The ID of the user creating the task</param>
    /// <returns>The created task</returns>
    Task<TaskResponse> CreateAsync(TaskCreateRequest request, Guid userId);

    /// <summary>
    /// Retrieves all tasks visible to the user (based on role).
    /// </summary>
    /// <param name="userId">The ID of the current user</param>
    /// <returns>List of all visible tasks</returns>
    Task<List<TaskResponse>> GetAllAsync(Guid userId);

    /// <summary>
    /// Retrieves a paginated list of tasks with optional status filter.
    /// </summary>
    /// <param name="query">Query parameters (page, pageSize, status)</param>
    /// <param name="userId">The ID of the current user</param>
    /// <returns>Paginated response with tasks</returns>
    Task<PagedResponse<TaskResponse>> GetPagedAsync(TaskQueryRequest query, Guid userId);

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="id">The ID of the task to update</param>
    /// <param name="request">The update request</param>
    /// <param name="userId">The ID of the user making the update</param>
    Task UpdateAsync(Guid id, TaskUpdateRequest request, Guid userId);

    /// <summary>
    /// Deletes a task.
    /// </summary>
    /// <param name="id">The ID of the task to delete</param>
    /// <param name="userId">The ID of the user making the deletion</param>
    Task DeleteAsync(Guid id, Guid userId);
}
