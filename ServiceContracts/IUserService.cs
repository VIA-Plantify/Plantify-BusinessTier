using DTOs;

namespace ServiceContracts;

public interface IUserService
{
    Task<UserDto> CreateAsync(CreateUserDto createUserDto);
    Task<UserDto> GetByEmailAsync(string email);
    Task<UserDto> GetByUsernameAsync(string username);
    Task UpdateAsync(string username, UpdateUserDto updateUserDto);
    Task DeleteAsync(string username);
    Task<IEnumerable<UserDto>> GetManyAsync();
}