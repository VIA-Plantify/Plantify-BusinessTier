using Entities;

namespace RepositoryContracts;

/// <summary>
/// Defines methods for handling user authentication operations.
/// </summary>
public interface IAuthRepository
{
    /// <param name="user">The User object containing the credentials to be validated.</param>
    /// <return>A task that represents the asynchronous operation. The task result contains the authenticated User object if the login is successful.</return>
    Task<User> LoginAsync(User user);
}