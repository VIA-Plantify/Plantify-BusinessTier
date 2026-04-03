using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DTOs;
using Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace WebAPI.Controllers;

/// <summary>
/// Handles authentication-related operations, including user login and JWT token generation.
/// </summary>
/// <remarks>
/// This controller provides a single endpoint for user authentication. It accepts login credentials
/// via username or email and returns a JWT token upon successful validation. The token is generated
/// using configuration settings for signing keys, issuer, and audience.
/// </remarks>
/// <param name="logger">ILogger instance for logging operations and errors.</param>
/// <param name="config">IConfiguration instance to retrieve JWT configuration values.</param>
/// <param name="authService">IAuthService instance to handle user authentication logic.</param>
/// <summary>
/// Authenticates a user based on provided credentials and returns a JWT token.
/// </summary>
/// <remarks>
/// Accepts login requests containing either a username or email along with a password.
/// Validates input, delegates authentication to the service layer, and generates a JWT
/// token if authentication succeeds. Returns error messages for invalid input or failed
/// authentication attempts.
/// </remarks>
/// <param name="userLoginDto">LoginRequest object containing user credentials.</param>
/// <returns>A JWT token as a string if successful, or an error message in case of failure.</returns>
/// <exception cref="Exception">Thrown when required fields are missing or authentication fails.</exception>
/// <summary>
/// Generates a JSON Web Token (JWT) for authenticated users.
/// </summary>
/// <remarks>
/// Creates a JWT token using claims derived from user data, configuration-based signing
/// credentials, and expiration settings. The token includes user identity information
/// and is signed using HMAC SHA-256 algorithm.
/// </remarks>
/// <param name="user">User object containing information to include in the token claims.</param>
/// <returns>A string representation of the generated JWT token.</returns>
[ApiController]
[Route("[controller]")]
public class AuthController (ILogger<AuthController> logger,IConfiguration config, IAuthService authService) :ControllerBase
{
    /// <summary>
    /// Handles user login by authenticating provided credentials and returning a JWT authentication token if successful.
    /// </summary>
    /// <param name="userLoginDto">Login credentials containing optional username, required password, and optional email. At least one of username or email must be provided.</param>
    /// <return>Returns Ok with JWT token containing authentication claims on success, or BadRequest with error details if authentication fails or invalid input is provided.</return>
    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest userLoginDto)
    {
        try
        {
            User user = new()
            {
                Password = userLoginDto.Password
            };
            if (string.IsNullOrEmpty(userLoginDto.Username) && string.IsNullOrEmpty(userLoginDto.Email))
            {
                throw new Exception("Username or Email is required");
            }

            if (!string.IsNullOrEmpty(userLoginDto.Username))
            {
                user.Username = userLoginDto.Username;
            }

            if (!string.IsNullOrEmpty(userLoginDto.Email))
            {
                user.Email = userLoginDto.Email;
            }

            var response = await authService.LoginAsync(user);
            string token = GenerateJwt(user);
            return Ok(token);
        }
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }

    /// <summary>
    /// Generates a JSON Web Token (JWT) for the provided user, containing claims such as username, name, and email.
    /// </summary>
    /// <param name="user">The user object whose information will be embedded into the JWT claims.</param>
    /// <return>A string representing the generated JWT token, which can be used for authentication purposes.</return>
    private string GenerateJwt(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? "");

        List<Claim> claims = GenerateClaims(user);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = config["Jwt:Issuer"],
            Audience = config["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Generates a list of claims based on the provided user information.
    /// </summary>
    /// <param name="user">The user object containing details to be included in the claims.</param>
    /// <return>A list of <see cref="Claim"/> objects representing the user's information and standard JWT claims.</return>
    private List<Claim> GenerateClaims(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, "FleetForward"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()
                    .ToString(),
                ClaimValueTypes.Integer64),
            new Claim("Username", user.Username),
            new Claim("Name", user.Name),
            new Claim("Email", user.Email)
        };

        return claims;
    }
}