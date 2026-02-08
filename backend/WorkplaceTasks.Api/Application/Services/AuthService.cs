using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WorkplaceTasks.Api.Application.Dtos;
using WorkplaceTasks.Api.Auth;
using WorkplaceTasks.Api.Infrastructure.Data;

namespace WorkplaceTasks.Api.Application.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthService(AppDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .SingleOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var token = _tokenService.GenerateToken(user);
        
        return new LoginResponse(token, user.Role.ToString(), user.Email);
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        var computedHash = Convert.ToBase64String(bytes);
        return computedHash == storedHash;
    }
}

