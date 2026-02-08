using System.Net;
using System.Text.Json;
using WorkplaceTasks.Api.Models;

namespace WorkplaceTasks.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next, 
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Map exception to HTTP status code and error code
        var (statusCode, errorCode) = MapExceptionToStatusCode(exception);
        
        // Log with structured context
        _logger.LogError(exception, 
            "Unhandled exception occurred. Path: {Path}, Method: {Method}, StatusCode: {StatusCode}, User: {User}",
            context.Request.Path,
            context.Request.Method,
            statusCode,
            context.User?.Identity?.Name ?? "Anonymous");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new ErrorResponse
        {
            Code = errorCode,
            Message = GetUserFriendlyMessage(exception),
            TraceId = context.TraceIdentifier
        };

        // Add detailed information in development mode only
        if (_environment.IsDevelopment())
        {
            response.Details = exception.Message;
            response.StackTrace = exception.StackTrace;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    private static (int statusCode, string errorCode) MapExceptionToStatusCode(Exception exception)
    {
        return exception switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, "RESOURCE_NOT_FOUND"),
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "ACCESS_DENIED"),
            ArgumentException => (StatusCodes.Status400BadRequest, "INVALID_ARGUMENT"),
            InvalidOperationException => (StatusCodes.Status400BadRequest, "INVALID_OPERATION"),
            _ => (StatusCodes.Status500InternalServerError, "INTERNAL_ERROR")
        };
    }

    private static string GetUserFriendlyMessage(Exception exception)
    {
        return exception switch
        {
            KeyNotFoundException => "The requested resource was not found.",
            UnauthorizedAccessException => "You do not have permission to perform this action.",
            ArgumentException => "Invalid input provided.",
            InvalidOperationException => "The operation cannot be completed.",
            _ => "An unexpected error occurred. Please try again later."
        };
    }
}
