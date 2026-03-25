using Entities;
using Grpc.Core;
using RepositoryContracts;

namespace GrpcRepositories.Services;

public class UserRepositoryGrpc(UserServiceProto.UserServiceProtoClient client)
    : IUserRepository
{
    private readonly UserServiceProto.UserServiceProtoClient _client = client;

    public async Task<User> CreateAsync(User user)
    {
        try
        {
            await GetByUsernameAsync(user.Username);
            
            // User exists, goes here.
            throw new InvalidOperationException($"User with email {user.Email} already exists.");
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        { 
            // User does not exist, goes here.
            var response = await _client.CreateAsync(new CreateUserRequest
            { 
                Name = user.Name,
                Username = user.Username,
                Password = user.Password,
                Email = user.Email
            });
                    
            return ParseUserResponseToEntity(response);
        }
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        var response = await _client.GetAsync(new GetUserRequest
        {
            Email = email
        });

        return ParseUserResponseToEntity(response);
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        var response = await _client.GetAsync(new GetUserRequest
        {
            Username = username
        });

        return ParseUserResponseToEntity(response);
    }

    public async Task UpdateAsync(User user)
    {
        await _client.UpdateAsync(new UpdateUserRequest
        {
            Name = user.Name,
            Username = user.Username,
            Password = user.Password,
            Email = user.Email
        });
    }
    
    public async Task DeleteAsync(string username)
    { 
        await _client.DeleteAsync(new DeleteUserRequest
        {
            Username = username
        });
    }
    
    public async Task<IEnumerable<User>> GetManyAsync()
    { 
        var response = await _client.GetAllAsync(new Google.Protobuf.WellKnownTypes.Empty());
        return response.Users.Select(ParseUserResponseToEntity);
    }


    private User ParseUserResponseToEntity(UserResponse userResponse)
    { 
        return new User()
        {
            Email = userResponse.Email,
            Name = userResponse.Name,
            Password = userResponse.Password,
            Username = userResponse.Username
        };
    }
}