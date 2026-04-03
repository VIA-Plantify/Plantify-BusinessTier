using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
            return CreatedAtAction(
                nameof(GetUser),
                new { username = created.Username },
                created
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
            return Ok(user);
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
}