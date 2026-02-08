using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WorkplaceTasks.Api.Application.Dtos;
using WorkplaceTasks.Api.Infrastructure.Data;
using WorkplaceTasks.Api.Infrastructure.Repositories;

namespace WorkplaceTasks.Api.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly AppDbContext _context;

    public UserService(IUserRepository repository, AppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _context.Users
            .Include(u => u.AssignedTasks)
            .ToListAsync();

        return users.Select(u => new UserDto(
            u.Id,
            u.Email,
            u.Role.ToString(),
            u.AssignedTasks?.Count ?? 0
        ));
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.AssignedTasks)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return null;

        return new UserDto(
            user.Id,
            user.Email,
            user.Role.ToString(),
            user.AssignedTasks?.Count ?? 0
        );
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        // Check if email already exists
        var existingUser = await _context.Users
            .AnyAsync(u => u.Email == dto.Email);
        
        if (existingUser)
        {
            throw new InvalidOperationException("Email already registered");
        }

        var user = new Domain.Entities.User
        {
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),
            Role = dto.Role
        };

        await _repository.AddAsync(user);

        return new UserDto(
            user.Id,
            user.Email,
            user.Role.ToString(),
            0
        );
    }

    public async Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found");
        }

        if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
        {
            // Check if new email is taken
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == dto.Email && u.Id != id);
            
            if (emailExists)
            {
                throw new InvalidOperationException("Email already in use");
            }
            user.Email = dto.Email;
        }

        if (!string.IsNullOrEmpty(dto.Password))
        {
            user.PasswordHash = HashPassword(dto.Password);
        }

        await _repository.UpdateAsync(user);

        return new UserDto(
            user.Id,
            user.Email,
            user.Role.ToString(),
            user.AssignedTasks?.Count ?? 0
        );
    }

    public async Task<UserDto> UpdateRoleAsync(Guid id, UpdateUserRoleDto dto)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found");
        }

        user.Role = dto.Role;
        await _repository.UpdateAsync(user);

        return new UserDto(
            user.Id,
            user.Email,
            user.Role.ToString(),
            0 // Task count not loaded here
        );
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found");
        }

        await _repository.DeleteAsync(id);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
