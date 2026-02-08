using WorkplaceTasks.Api.Application.Dtos;

namespace WorkplaceTasks.Api.Application.Services;

/// <summary>
/// Service interface for authentication operations.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Login response with user info and token</returns>
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
