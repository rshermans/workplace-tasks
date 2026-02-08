using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkplaceTasks.Api.Application.Dtos;
using WorkplaceTasks.Api.Application.Services;

namespace WorkplaceTasks.Api.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize] // All endpoints require auth
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;

    public TasksController(ITaskService service)
    {
        _service = service;
    }

    /// <summary>
    /// Creates a new task in the workbench.
    /// </summary>
    /// <param name="request">The task details (Title, Description).</param>
    /// <returns>The created task item with its generated ID.</returns>
    /// <response code="201">Returns the newly created item.</response>
    /// <response code="400">If the request is malformed or validation fails.</response>
    /// <response code="401">If the user is not authenticated.</response>
    [HttpPost]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskResponse>> Create([FromBody] TaskCreateRequest request)
    {
        var userId = GetCurrentUserId();
        var task = await _service.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetAll), new { id = task.Id }, task);
    }

    /// <summary>
    /// Retrieves all tasks.
    /// </summary>
    /// <remarks>
    /// Note: Admins/Managers see all tasks. Members see only their own.
    /// </remarks>
    /// <returns>A list of tasks visible to the current user.</returns>
    /// <response code="200">Returns the list of tasks.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<TaskResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TaskResponse>>> GetAll()
    {
        var userId = GetCurrentUserId();
        var tasks = await _service.GetAllAsync(userId);
        return Ok(tasks);
    }

    /// <summary>
    /// Retrieves tasks with pagination and optional status filter.
    /// </summary>
    /// <param name="page">Page number (1-based, default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10, max: 50)</param>
    /// <param name="status">Optional filter by task status</param>
    /// <returns>Paginated response with tasks</returns>
    /// <response code="200">Returns paginated list of tasks</response>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResponse<TaskResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<TaskResponse>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Domain.Enums.TaskStatus? status = null)
    {
        // Validate and constrain parameters
        if (pageSize > 50) pageSize = 50;
        if (pageSize < 1) pageSize = 1;
        if (page < 1) page = 1;
        
        var query = new TaskQueryRequest
        {
            Page = page,
            PageSize = pageSize,
            Status = status
        };
        
        var userId = GetCurrentUserId();
        var result = await _service.GetPagedAsync(query, userId);
        return Ok(result);
    }
    
    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="id">The unique ID of the task.</param>
    /// <param name="request">The updated task details.</param>
    /// <response code="200">If the update was successful.</response>
    /// <response code="403">If the user does not have permission to update THIS task.</response>
    /// <response code="404">If the task ID does not exist.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] TaskUpdateRequest request)
    {
        var userId = GetCurrentUserId();
        await _service.UpdateAsync(id, request, userId);
        return Ok();
    }

    /// <summary>
    /// Deletes a specific task.
    /// </summary>
    /// <param name="id">The unique ID of the task.</param>
    /// <response code="204">If the deletion was successful.</response>
    /// <response code="403">If the user does not have permission to delete THIS task.</response>
    /// <response code="404">If the task ID does not exist.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetCurrentUserId();
        await _service.DeleteAsync(id, userId);
        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(sub, out var guid)) return guid;
        throw new UnauthorizedAccessException("Invalid User Token");
    }
}
