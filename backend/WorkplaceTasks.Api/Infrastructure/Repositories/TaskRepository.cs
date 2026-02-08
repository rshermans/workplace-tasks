using System.Linq;
using Microsoft.EntityFrameworkCore;
using WorkplaceTasks.Api.Domain.Entities;
using WorkplaceTasks.Api.Infrastructure.Data;

namespace WorkplaceTasks.Api.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return await _context.Tasks
            .Include(t => t.CreatedBy) // Include creator for RBAC checks
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<TaskItem>> GetAllAsync()
    {
        return await _context.Tasks
            .Include(t => t.CreatedBy)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<TaskItem>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Tasks
            .Include(t => t.CreatedBy)
            .Where(t => t.CreatedByUserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<(List<TaskItem> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        Domain.Enums.TaskStatus? status)
    {
        var query = _context.Tasks.Include(t => t.CreatedBy).AsQueryable();
        
        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);
        
        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (items, totalCount);
    }

    public async Task<(List<TaskItem> Items, int TotalCount)> GetPagedByUserIdAsync(
        Guid userId, 
        int page, 
        int pageSize, 
        Domain.Enums.TaskStatus? status)
    {
        var query = _context.Tasks
            .Include(t => t.CreatedBy)
            .Where(t => t.CreatedByUserId == userId);
        
        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);
        
        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (items, totalCount);
    }

    public async Task AddAsync(TaskItem task)
    {
        await _context.Tasks.AddAsync(task);
    }

    public async Task DeleteAsync(TaskItem task)
    {
        _context.Tasks.Remove(task);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
