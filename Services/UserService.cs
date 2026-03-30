using System.Text.RegularExpressions;
using DTOs;
using Entities;
using RepositoryContracts;
using ServiceContracts;

namespace Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserDto> CreateAsync(CreateUserDto createUserDto)
    {
        if (createUserDto == null)
        {
            throw new ArgumentNullException(nameof(createUserDto));
        }

        if (string.IsNullOrWhiteSpace(createUserDto.Password))
        {
            throw new ArgumentException("Password is required and can't be blank");
        }

        if (!Regex.IsMatch(createUserDto.Password,
                @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d])[A-Za-z\d\S]{8,64}$"))
        {
            throw new ArgumentException("Password does not meet the requirements");
        }

        if (string.IsNullOrWhiteSpace(createUserDto.Username))
        {
            throw new ArgumentException("Username is required and can't be blank");
        }

        if (string.IsNullOrWhiteSpace(createUserDto.Email))
        {
            throw new ArgumentException("Email is required and can't be blank");
        }

        if (!Regex.IsMatch(createUserDto.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
        {
            throw new ArgumentException("Email format is not valid");
        }

        if (string.IsNullOrWhiteSpace(createUserDto.Name))
        {
            throw new ArgumentException("Name is required and can't be blank");
        }

        try
        {
            var createdUser = await _repository.CreateAsync(new User
            {
                Name = createUserDto.Name,
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                Password = createUserDto.Password
            });

            return new UserDto()
            {
                Email = createdUser.Email,
                Username = createdUser.Username,
                Name = createdUser.Name
            };
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("User with email"))
        {
            throw new InvalidOperationException($"User with email {createUserDto.Email} already exists");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("User with username"))
        {
            throw new InvalidOperationException($"User with username {createUserDto.Username} already exists");
        }
    }

    public async Task<UserDto> GetByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required and can't be blank");
        }

        if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
        {
            throw new ArgumentException("Email format is not valid");
        }

        var fetchedUser = await _repository.GetByEmailAsync(email);

        if (fetchedUser == null)
        {
            throw new InvalidOperationException($"User with email {email} not found");
        }

        return new UserDto()
        {
            Name = fetchedUser.Name,
            Email = fetchedUser.Email,
            Username = fetchedUser.Username
        };
    }

    public async Task<UserDto> GetByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required and can't be blank");
        }

        var fetchedUser = await _repository.GetByUsernameAsync(username);

        if (fetchedUser == null)
        {
            throw new KeyNotFoundException($"User with username {username} not found");
        }

        return new UserDto()
        {
            Name = fetchedUser.Name,
            Email = fetchedUser.Email,
            Username = fetchedUser.Username
        };
    }

    public async Task UpdateAsync(string username, UpdateUserDto updateUserDto)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required and can't be blank");
        }

        if (updateUserDto == null)
        {
            throw new ArgumentException(nameof(updateUserDto));
        }

        var fetchedUser = await _repository.GetByUsernameAsync(username);

        if (fetchedUser == null)
        {
            throw new KeyNotFoundException($"User with username {username} not found");
        }

        fetchedUser.Name = updateUserDto.Name;
        fetchedUser.Email = updateUserDto.Email;

        try
        {
            await _repository.UpdateAsync(fetchedUser);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }

    public async Task DeleteAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required and can't be blank");
        }

        var fetchedUser = await _repository.GetByUsernameAsync(username);

        if (fetchedUser == null)
        {
            throw new KeyNotFoundException($"User with username {username} not found");
        }

        await _repository.DeleteAsync(username);
    }

    public async Task<IEnumerable<UserDto>> GetManyAsync()
    {
        var fetchedUsers = await _repository.GetManyAsync();

        if (fetchedUsers == null)
        {
            throw new InvalidOperationException("Failed to fetch any user");
        }

        return fetchedUsers.Select(u => new UserDto() { Name = u.Name, Email = u.Email, Username = u.Username });
    }
}