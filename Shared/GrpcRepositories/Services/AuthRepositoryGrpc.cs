using Entities;
using Grpc.Core;
using RepositoryContracts;

namespace GrpcRepositories.Services;

/// <summary>
/// Provides gRPC-based implementation for authentication repository operations.
/// Handles user authentication through gRPC communication with the AuthServiceProto service.
/// </summary>
/// <remarks>
/// This class implements the IAuthRepository interface and uses AuthServiceProtoClient
/// to communicate with the gRPC authentication service. It specifically handles login operations
/// and translates gRPC responses into domain model objects.
/// </remarks>
/// <seealso cref="IAuthRepository"/>
/// <seealso cref="AuthServiceProto.AuthServiceProtoClient"/>
public class AuthRepositoryGrpc(AuthServiceProto.AuthServiceProtoClient client) : IAuthRepository
{
    /// <summary>
    /// Authenticates a user by verifying their credentials and returns the corresponding user information if successful.
    /// </summary>
    /// <param name="user">The user credentials containing email, password, and username required to authenticate.</param>
    /// <return>Returns the authenticated user object if login is successful; otherwise, exceptions are thrown for invalid credentials or missing user data.</return>
    public async Task<User> LoginAsync(User user)
    {
        try
        {
            var response = await client.LoginAsync(new AuthRequest
            {
                Email = user.Email,
                Password = user.Password,
                Username = user.Username
            });
            
            return await Task.FromResult(new User
            {
                Email = response.Email,
                Name = response.Name,
                Password = response.Password,
                Username = response.Username
            });
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException($"User with Username: {user.Username} or Email: {user.Email} not found.");
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            throw new InvalidOperationException($"User with Username: {user.Username} or Email: {user.Email} not found.");
        }
        
    }
}