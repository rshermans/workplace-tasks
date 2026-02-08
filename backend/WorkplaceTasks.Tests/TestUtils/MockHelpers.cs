using Moq;
using Microsoft.EntityFrameworkCore;
using WorkplaceTasks.Api.Infrastructure.Data;

namespace WorkplaceTasks.Tests.TestUtils;

public static class MockHelpers
{
    public static Mock<AppDbContext> CreateMockDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
            
        return new Mock<AppDbContext>(options);
    }
    
    // Additional helpers can be added here once we start implementing specific service tests
}
