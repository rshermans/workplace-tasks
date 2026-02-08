namespace WorkplaceTasks.Api.Auth;

/// <summary>
/// Service interface for JWT token generation.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user to generate a token for</param>
    /// <returns>A JWT token string</returns>
    string GenerateToken(Domain.Entities.User user);
}
