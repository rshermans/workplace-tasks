using System.Security.Cryptography;
using System.Text;
using WorkplaceTasks.Api.Domain.Entities;

namespace WorkplaceTasks.Api.Infrastructure.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (context.Users.Any())
        {
            return; // DB has been seeded
        }

        var users = new List<User>
        {
            new User
            {
                Email = "admin@example.com",
                PasswordHash = HashPassword("Password123!"),
                Role = Role.Admin
            },
            new User
            {
                Email = "manager@example.com",
                PasswordHash = HashPassword("Password123!"),
                Role = Role.Manager
            },
            new User
            {
                Email = "member@example.com",
                PasswordHash = HashPassword("Password123!"),
                Role = Role.Member
            }
        };

        context.Users.AddRange(users);
        context.SaveChanges();
    }

    private static string HashPassword(string password)
    {
        // Simple SHA256 for demo purposes. In production use BCrypt or Argon2
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
