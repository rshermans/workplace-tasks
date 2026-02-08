using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkplaceTasks.Api.Application.Dtos;
using WorkplaceTasks.Api.Domain.Entities;
using WorkplaceTasks.Api.Domain.Policies;
using WorkplaceTasks.Api.Infrastructure.Data;
using WorkplaceTasks.Api.Infrastructure.Repositories;
using TaskStatus = WorkplaceTasks.Api.Domain.Enums.TaskStatus;

namespace WorkplaceTasks.Api.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly AppDbContext _dbContext; // For User lookup

    public TaskService(ITaskRepository repository, AppDbContext dbContext)
    {
        _repository = repository;
        _dbContext = dbContext;
    }

    public async Task<TaskResponse> CreateAsync(TaskCreateRequest request, Guid userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null) throw new UnauthorizedAccessException("User not found");

        if (!TaskAuthorizationPolicy.CanCreate(user))
        {
             throw new UnauthorizedAccessException("Not allowed to create tasks");
        }

        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            Status = TaskStatus.Pending,
            CreatedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(task);
        await _repository.SaveChangesAsync();

        return MapToResponse(task, user.Email);
    }

    public async Task<List<TaskResponse>> GetAllAsync(Guid userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null) throw new UnauthorizedAccessException("User not found");

        // All roles can read all tasks (as per client specification)
        var tasks = await _repository.GetAllAsync();

        return tasks.Select(t => MapToResponse(t, t.CreatedBy?.Email ?? "Unknown")).ToList();
    }

    public async Task<PagedResponse<TaskResponse>> GetPagedAsync(TaskQueryRequest query, Guid userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null) throw new UnauthorizedAccessException("User not found");

        // All roles can read all tasks (as per client specification)
        var result = await _repository.GetPagedAsync(query.Page, query.PageSize, query.Status);

        var totalPages = (int)Math.Ceiling(result.TotalCount / (double)query.PageSize);
        
        return new PagedResponse<TaskResponse>(
            result.Items.Select(t => MapToResponse(t, t.CreatedBy?.Email ?? "Unknown")).ToList(),
            result.TotalCount,
            query.Page,
            query.PageSize,
            totalPages
        );
    }

    public async Task UpdateAsync(Guid id, TaskUpdateRequest request, Guid userId)
    {
        var task = await _repository.GetByIdAsync(id);
        if (task == null) throw new KeyNotFoundException($"Task {id} not found");

        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null) throw new UnauthorizedAccessException("User not found");

        if (!TaskAuthorizationPolicy.CanUpdate(user, task))
        {
             throw new UnauthorizedAccessException($"User {user.Role} cannot update this task");
        }

        if (request.Title != null) task.Title = request.Title;
        if (request.Description != null) task.Description = request.Description;
        if (request.Status.HasValue) task.Status = request.Status.Value;

        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var task = await _repository.GetByIdAsync(id);
        if (task == null) throw new KeyNotFoundException($"Task {id} not found");

        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null) throw new UnauthorizedAccessException("User not found");

        if (!TaskAuthorizationPolicy.CanDelete(user, task))
        {
             throw new UnauthorizedAccessException($"User {user.Role} cannot delete this task");
        }

        await _repository.DeleteAsync(task);
        await _repository.SaveChangesAsync();
    }

    private static TaskResponse MapToResponse(TaskItem t, string email)
    {
        return new TaskResponse(
            t.Id,
            t.Title,
            t.Description,
            t.Status,
            t.CreatedAt,
            t.CreatedByUserId,
            email
        );
    }
}
