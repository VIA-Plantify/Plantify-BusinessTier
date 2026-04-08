using System.Text.RegularExpressions;
using Entities;
using RepositoryContracts;
using ServiceContracts;
using Services.SecurityUtils;

namespace Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<User> CreateAsync(User user)
    {

        if (!Regex.IsMatch(user.Password,
                @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d])[A-Za-z\d\S]{8,64}$"))
        {
            throw new ArgumentException("Password does not meet the requirements");
        }

        user.Password = PasswordHasher.HashPassword(user.Password);
        
        try
        {
            var createdUser = await _repository.CreateAsync(user);

            return createdUser;
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("User with email"))
        {
            throw new InvalidOperationException($"User with email {user.Email} already exists");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("User with username"))
        {
            throw new InvalidOperationException($"User with username {user.Username} already exists");
        }
    }

    public async Task<User> GetByEmailAsync(string email)
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

        return fetchedUser;
    }

    public async Task<User> GetByUsernameAsync(string username)
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

        return fetchedUser;
    }

    public async Task UpdateAsync(User user)
    {
        var fetchedUser = await _repository.GetByUsernameAsync(user.Username);

        if (fetchedUser == null)
        {
            throw new KeyNotFoundException($"User with username {user.Username} not found");
        }

        fetchedUser.Name = user.Name;
        fetchedUser.Email = user.Email;

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

    public async Task<IEnumerable<User>> GetManyAsync()
    {
        var fetchedUsers = await _repository.GetManyAsync();

        if (fetchedUsers == null)
        {
            throw new InvalidOperationException("Failed to fetch any user");
        }

        return fetchedUsers;
    }
}