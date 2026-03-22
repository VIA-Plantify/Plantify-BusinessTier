using Entities;

namespace RepositoryContracts;

public interface IUserRepository
{
    Task<User> CreateAsync(User user);
    Task<User> GetByEmailAsync(string email);
    Task<User> GetByUsernameAsync(string username);
    Task DeleteAsync(string username);
    Task UpdateAsync(User user);
    Task<IEnumerable<User>> GetManyAsync();
}