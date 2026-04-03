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

[ApiController]
[Route("[controller]")]
public class AuthController (ILogger<AuthController> logger,IConfiguration config, IAuthService authService) :ControllerBase
{
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