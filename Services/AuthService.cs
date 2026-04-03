using Entities;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using ServiceContracts;
using Services.SecurityUtils;

namespace Services;

/// <summary>
/// Provides authentication functionality for user login operations.
/// Handles user credential verification through repository interaction and password hashing validation.
/// </summary>
/// <remarks>
/// This service coordinates between the authentication repository and user service to validate user credentials.
/// Implements error handling for authentication failures including logging exceptions and propagating appropriate error messages.
/// </remarks>
/// <param name="authRepository">Repository interface for accessing authentication-related data operations</param>
/// <param name="logger">Logger instance for tracking authentication-related events and errors</param>
/// <param name="userService">Service interface for user-related operations</param>
/// <exception cref="InvalidOperationException">Thrown when authentication fails due to invalid credentials</exception>
/// <exception cref="InvalidDataException">Thrown when password verification fails after repository lookup</exception>
public class AuthService(IAuthRepository authRepository, ILogger<AuthService> logger, IUserService userService)
    : IAuthService
{
    /// <summary>
    /// Authenticates a user based on provided credentials.
    /// </summary>
    /// <param name="user">The user object containing username and password for authentication.</param>
    /// <return>Returns the authenticated user if successful, otherwise throws an exception.</return>
    /// <exception cref="InvalidOperationException">Thrown when authentication fails due to invalid credentials.</exception>
    /// <exception cref="InvalidDataException">Thrown when password verification fails after repository lookup.</exception>
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