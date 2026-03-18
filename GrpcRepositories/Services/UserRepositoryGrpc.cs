using Entities;
using Grpc.Core;
using Microsoft.Extensions.Logging.Configuration;
using RepositoryContracts;

namespace GrpcRepositories.Services;

public class UserRepositoryGrpc(UserServiceProto.UserServiceProtoClient client)
    : IUserRepository
{
    private readonly UserServiceProto.UserServiceProtoClient _client = client;

    public async Task<User> CreateAsync(User user)
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

    public async Task<User> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(string username)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(User user)
    {
        throw new NotImplementedException();
    }

    public IQueryable GetMany()
    {
        throw new NotImplementedException();
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