using Entities;
using Microsoft.Testing.Platform.Logging;
using RepositoryContracts;
using ServiceContracts;
using Services.SecurityUtils;

namespace Services;

public class AuthService(IAuthRepository authRepository, ILogger<AuthService> logger, IUserService userService)
    : IAuthService
{

    public async Task<User> LoginAsync(User user)
    {
        try
        {
            var response = await authRepository.LoginAsync(user);
            if (PasswordHasher.VerifyPassword(user.Password, response.Password))
            {
                return await Task.FromResult(user);
            }
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex.Message);
            throw new InvalidOperationException("Invalid credentials", ex);
        }
        throw new InvalidDataException("Incorect password");
    }
}