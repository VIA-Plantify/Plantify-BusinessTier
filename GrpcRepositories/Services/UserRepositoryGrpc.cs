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
            var response = await _client.CreateAsync(new CreateUserRequest
            {
                Name = user.Name,
                Username = user.Username,
                Password = user.Password,
                Email = user.Email
            });
    
            return ParseUserResponseToEntity(response);
        }
        catch (RpcException ex)
        {
            switch (ex.StatusCode)
            {
                case StatusCode.AlreadyExists:
                    if (ex.Status.Detail.Contains("email", StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException("A user with this email already exists.");
    
                    if (ex.Status.Detail.Contains("username", StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException("A user with this username already exists.");
    
                    throw new InvalidOperationException("User already exists.");
    
                case StatusCode.InvalidArgument:
                    throw new ArgumentException($"Invalid input: {ex.Status.Detail}");
    
                case StatusCode.Internal:
                    throw new Exception("Server error occurred while creating user.");
    
                default:
                    throw new Exception($"Unexpected error: {ex.Status.Detail}");
            }
        }
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
        catch (RpcException ex)
        {
            switch (ex.StatusCode)
            {
                case StatusCode.NotFound:
                    throw new InvalidOperationException($"No user found with email: {email}");

                case StatusCode.Internal:
                    throw new Exception("Server error occurred while retrieving user by email.");

                default:
                    throw new Exception($"Unexpected error: {ex.Status.Detail}");
            }
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
        catch (RpcException ex)
        {
            switch (ex.StatusCode)
            {
                case StatusCode.NotFound:
                    throw new InvalidOperationException($"No user found with username: {username}");

                case StatusCode.Internal:
                    throw new Exception("Server error occurred while retrieving user by username.");

                default:
                    throw new Exception($"Unexpected error: {ex.Status.Detail}");
            }
        }
    }

    public async Task UpdateAsync(User user)
    {
        try
        {
            await _client.UpdateAsync(new UpdateUserRequest
            {
                Name = user.Name,
                Username = user.Username,
                Password = user.Password,
                Email = user.Email
            });
        }
        catch (RpcException ex)
        {
            switch (ex.StatusCode)
            {
                case StatusCode.NotFound:
                    throw new InvalidOperationException($"Cannot update. User '{user.Username}' not found.");

                case StatusCode.AlreadyExists:
                    if (ex.Status.Detail.Contains("email", StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException("Another user already uses this email.");

                    if (ex.Status.Detail.Contains("username", StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException("Another user already uses this username.");

                    throw;

                case StatusCode.InvalidArgument:
                    throw new ArgumentException($"Invalid data: {ex.Status.Detail}");

                case StatusCode.Internal:
                    throw new Exception("Server error occurred while updating user.");

                default:
                    throw new Exception($"Unexpected error: {ex.Status.Detail}");
            }
        }
    }
    
    public async Task DeleteAsync(string username)
    {
        try
        {
            await _client.DeleteAsync(new DeleteUserRequest
            {
                Username = username
            });
        }
        catch (RpcException ex)
        {
            switch (ex.StatusCode)
            {
                case StatusCode.NotFound:
                    throw new InvalidOperationException($"Cannot delete. No user found with username: {username}");

                case StatusCode.Internal:
                    throw new Exception("Server error occurred while deleting user.");

                default:
                    throw new Exception($"Unexpected error: {ex.Status.Detail}");
            }
        }
    }
    
    public async Task<IEnumerable<User>> GetManyAsync()
    {
        try
        {
            var response = await _client.GetAllAsync(
                new Google.Protobuf.WellKnownTypes.Empty()
            );

            return response.Users
                .Select(ParseUserResponseToEntity);
        }
        catch (RpcException ex)
        {
            switch (ex.StatusCode)
            {
                case StatusCode.Internal:
                    throw new Exception("Server error occurred while retrieving users.");

                default:
                    throw new Exception($"Unexpected error: {ex.Status.Detail}");
            }
        }
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