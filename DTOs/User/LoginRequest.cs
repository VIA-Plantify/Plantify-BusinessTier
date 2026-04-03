namespace DTOs;

public record LoginRequest()
{
    
    public string? Username { get; init; }
    public required string Password { get; init; }
    public string? Email { get; init; }
}