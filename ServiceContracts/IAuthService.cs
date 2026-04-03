using Entities;
using RepositoryContracts;

namespace ServiceContracts;

/// <summary>
/// Defines the contract for authentication-related services, providing methods to handle user authentication.
/// This interface inherits from <see cref="IAuthRepository"/> and is implemented by classes that manage authentication logic.
/// </summary>
public interface IAuthService : IAuthRepository
{ 
    
}