using Entities;
using Grpc.Core;
using RepositoryContracts;

namespace GrpcRepositories.Services;

public class AuthRepositoryGrpc(AuthServiceProto.AuthServiceProtoClient client) : IAuthRepository
{
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