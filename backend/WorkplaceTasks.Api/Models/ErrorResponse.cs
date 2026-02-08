namespace WorkplaceTasks.Api.Models;

/// <summary>
/// Standardized error response model for all API errors.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Error code for programmatic handling (e.g., "RESOURCE_NOT_FOUND", "ACCESS_DENIED").
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// User-friendly error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Trace ID for request tracking and debugging.
    /// </summary>
    public string TraceId { get; set; } = string.Empty;

    /// <summary>
    /// Detailed exception message (Development mode only).
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Stack trace (Development mode only).
    /// </summary>
    public string? StackTrace { get; set; }
}
