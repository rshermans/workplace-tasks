using WorkplaceTasks.Api.Application.Dtos;

namespace WorkplaceTasks.Api.Application.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto);
    Task<UserDto> UpdateRoleAsync(Guid id, UpdateUserRoleDto dto);
    Task DeleteAsync(Guid id);
}
