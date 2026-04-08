using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace WebAPI.Controllers;
[ApiController]
[Route("[controller]")]
public class UserController(ILogger<UserController> logger,IUserService userService) : ControllerBase
{
    /// <summary>
    /// Creates a new user based on the provided data transfer object.
    /// </summary>
    /// <param name="dto">The data transfer object containing user details required to create a new user.</param>
    /// <return>Returns the created user as a <see cref="UserDto"/> with a 201 Created status if successful. Returns a 400 Bad Request or 404 Not Found response if validation or data retrieval fails.</return>
    [HttpPost]
    public async Task<ActionResult<UserDto>> Post([FromBody] CreateUserDto dto)
    {
        try
        {
            var user = new User()
            {
                Email = dto.Email,
                Password = dto.Password,
                Name = dto.Name,
                Username = dto.Username
            };
            var created = await userService.CreateAsync(user);
            CheckUserDataIntegrity(created);
            var createdDto = new UserDto()
            {
                Email = user.Email,
                Name = user.Name,
                Username = user.Username
            };
            return CreatedAtAction(
                nameof(GetUser),
                new { username = createdDto.Username }, createdDto
            );
        }
        catch (InvalidDataException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentNullException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>Retrieves a user by their username and returns the corresponding UserDto.</summary>
    /// <param name="username">The username of the user to retrieve. This parameter is required as per the route configuration.</param>
    /// <return>Returns the UserDto if the user is found; otherwise, returns an appropriate error response (e.g., BadRequest if invalid data is encountered, NotFound if the user does not exist).</return>
    [HttpGet("u/{username}")]
    public async Task<ActionResult<UserDto>> GetUser([FromRoute] string? username)
    {
        try
        {
            if (username == null)
            {
                return BadRequest("Username must be provided");
            }

            var user = await userService.GetByUsernameAsync(username);
            CheckUserDataIntegrity(user);
            var dto = new UserDto()
            {
                Email = user.Email,
                Name = user.Name,
                Username = user.Username
            };
            return Ok(dto);
        }
        catch (InvalidDataException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentNullException ex)
        {
            return NotFound(ex.Message);
        }
    }
    /// <summary>Retrieves a user by their email and returns the corresponding UserDto.</summary>
    /// <param name="email">The email of the user to retrieve. This parameter is required as per the route configuration.</param>
    /// <return>Returns the UserDto if the user is found; otherwise, returns an appropriate error response (e.g., BadRequest if invalid data is encountered, NotFound if the user does not exist).</return>
    [HttpGet("u/{email}")]
    public async Task<ActionResult<UserDto>> GetUserByEmail([FromRoute] string? email)
    {
        try
        {
            if (email == null)
            {
                return BadRequest("Email must be provided");
            }

            var user = await userService.GetByEmailAsync(email);
            CheckUserDataIntegrity(user);
            var dto = new UserDto()
            {
                Email = user.Email,
                Name = user.Name,
                Username = user.Username
            };
            return Ok(dto);
        }
        catch (InvalidDataException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentNullException ex)
        {
            return NotFound(ex.Message);
        }
    }
    /// <summary>
    /// Updates the profile information of the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// The user's identity is extracted directly from the JWT "Username" claim to ensure they can only update their own account.
    /// </remarks>
    /// <param name="dto">The data transfer object containing the updated Name and Email.</param>
    /// <returns>
    /// Returns a <see cref="UserDto"/> with the updated information and a 200 OK status if successful or an appropriate error response (e.g., BadRequest if invalid data is encountered, NotFound if the user does not exist) 
    /// </returns>
    [HttpPut]
    [Authorize]
    public async Task<ActionResult<UserDto>> Update([FromBody] UpdateUserDto dto)
    {
        var loggedInUsername = User.FindFirst("Username")?.Value; 

        if (string.IsNullOrEmpty(loggedInUsername))
        {
            return Unauthorized("User identity not found in token.");
        }

        try
        {
            var existingUser = await userService.GetByUsernameAsync(loggedInUsername);
        
            if (existingUser == null) 
            {
                return NotFound("User profile not found.");
            }

            existingUser.Name = dto.Name;
            existingUser.Email = dto.Email;

            await userService.UpdateAsync(existingUser);

            var updatedDto = new UserDto()
            {
                Email = existingUser.Email,
                Name = existingUser.Name,
                Username = existingUser.Username
            };

            return Ok(updatedDto);
        }
        catch (InvalidDataException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentNullException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    /// <summary>
    /// Deletes a user from the system.
    /// </summary>
    /// <param name="username">The username of the user to delete.</param>
    [HttpDelete("{username}")]
    [Authorize]
    public async Task<ActionResult> Delete([FromRoute] string username)
    {
        var loggedInUserClaim = User.FindFirst("Username")?.Value;

        if (string.IsNullOrEmpty(loggedInUserClaim))
        {
            return Unauthorized("User identity not found in token.");
        }
        
        if (!loggedInUserClaim.Equals(username, StringComparison.OrdinalIgnoreCase))
        {
            return Forbid(); 
        }

        try
        {
            var user = await userService.GetByUsernameAsync(username);
            CheckUserDataIntegrity(user);

            await userService.DeleteAsync(username);
        
            return NoContent(); 
        }
        catch (ArgumentNullException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting user {username}", username);
            return StatusCode(500, "An internal error occurred");
        }
    }
    
    /// <summary>
    /// Validates the integrity of the user data by checking for null values and ensuring that the username and email are provided.
    /// </summary>
    /// <param name="user">The user object to validate.</param>
    private void CheckUserDataIntegrity(User? user)
    {
        if (user == null)
        {
            logger.LogInformation("User {username} not found", user?.Username);
            throw new ArgumentNullException("User not found");
        }

        if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Email))
        {
            logger.LogInformation("User username:{username} , email:{email} not found", user?.Username, user?.Email);
            throw new InvalidDataException("Username and Email must be provided");
        }
    }
    
    /// <summary>
    /// Retrieves a list of all registered users.
    /// </summary>
    /// <returns>A list of UserDto objects.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        try
        {
            var users = await userService.GetManyAsync();

            var userDtos = users.Select(user => new UserDto
            {
                Username = user.Username,
                Name = user.Name,
                Email = user.Email
            }).ToList();

            return Ok(userDtos);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while retrieving all users.");
            return StatusCode(500, "Internal server error while fetching users.");
        }
    }
}
