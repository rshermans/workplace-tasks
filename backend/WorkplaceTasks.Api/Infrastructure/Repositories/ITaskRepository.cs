using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkplaceTasks.Api.Domain.Entities;
using TaskStatus = WorkplaceTasks.Api.Domain.Enums.TaskStatus;

namespace WorkplaceTasks.Api.Infrastructure.Repositories;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task<List<TaskItem>> GetAllAsync();
    Task<List<TaskItem>> GetByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Retrieves a paginated list of all tasks with optional status filter.
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="status">Optional filter by task status</param>
    /// <returns>A tuple containing the list of tasks and the total count</returns>
    Task<(List<TaskItem> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        TaskStatus? status);
    
    /// <summary>
    /// Retrieves a paginated list of tasks for a specific user with optional status filter.
    /// </summary>
    /// <param name="userId">The user ID to filter tasks by</param>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="status">Optional filter by task status</param>
    /// <returns>A tuple containing the list of tasks and the total count</returns>
    Task<(List<TaskItem> Items, int TotalCount)> GetPagedByUserIdAsync(
        Guid userId, 
        int page, 
        int pageSize, 
        TaskStatus? status);
    
    Task AddAsync(TaskItem task);
    Task DeleteAsync(TaskItem task);
    Task SaveChangesAsync();
}
