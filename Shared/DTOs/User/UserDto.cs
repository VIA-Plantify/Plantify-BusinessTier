namespace DTOs;

public record UserDto
{
    public required string Name { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
}