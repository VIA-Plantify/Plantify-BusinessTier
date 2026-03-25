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
            var exists = await GetByUsernameAsync(user.Username);
        }
        catch (InvalidOperationException)
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
        throw new InvalidOperationException($"User with username {user.Username} already exists.");
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        try
        {
            var response = await _client.GetAsync(new GetUserRequest
            {
                Email = email
            });

            return ParseUserResponseToEntity(response);
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException($"User with the following email: {email} was not found.");
        }
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        try
        {
            var response = await _client.GetAsync(new GetUserRequest
            {
                Username = username
            });

            return ParseUserResponseToEntity(response);
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException($"User with username {username} not found.");
        }
}

    public async Task UpdateAsync(User user)
    {
        try
        {
            var response = await GetByUsernameAsync(user.Username);
        await _client.UpdateAsync(new UpdateUserRequest
        {
            Name = user.Name,
            Username = user.Username,
            Password = user.Password,
            Email = user.Email
        });
        
        }
        catch(InvalidOperationException ex)
        {
            throw new InvalidOperationException(ex.Message);
        }
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
        if (userResponse == null)
        {
            throw new InvalidOperationException("Cannot parse null object");
        }
        return new User()
        {
            Email = userResponse.Email,
            Name = userResponse.Name,
            Password = userResponse.Password,
            Username = userResponse.Username
        };
    }
}