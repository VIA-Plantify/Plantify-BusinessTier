using Entities;

namespace RepositoryContracts;

public interface IAuthRepository
{
    
    Task<User> LoginAsync(User user);
}