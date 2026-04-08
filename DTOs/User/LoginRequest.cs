namespace DTOs;

/// <summary>
/// Represents a request object containing user login credentials. Includes optional username, required password, and optional email. At least one of the username or email must be provided for the login to be valid.
/// </summary>
public record LoginRequest()
{
    /// <summary>
    /// The username used to identify the user during login. This property is optional but must be provided along with the Email property if left empty.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    /// The password associated with the user account. This property is required and must be provided
    /// to authenticate the user during the login process. It is used to verify the user's identity
    /// against the system's stored credentials.
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    /// Gets or sets the email address associated with the user account. Used as an alternative to Username for authentication purposes. May be null if not provided.
    /// </summary>
    public string? Email { get; init; }
}