using System.Security.Cryptography;
using System.Text;

namespace Services.SecurityUtils;

/// <summary>
/// Provides methods for hashing and verifying passwords using PBKDF2 with HMAC-SHA256.
/// </summary>
/// <remarks>
/// The class uses a random salt and configurable iterations to generate secure password hashes.
/// Hashes are stored as a combination of Base64-encoded salt and hash, separated by a dot.
/// </remarks>
public static class PasswordHasher
{
    /// Number of iterations for the PBKDF2 key derivation function, chosen to balance security and performance.
    private const int Iterations = 100_000;

    /// Size of the salt in bytes used during password hashing
    private const int SaltSize = 16;

    /// Hash size in bytes for PBKDF2 derived key (matches SHA256 output length)
    private const int HashSize = 32;

    /// <summary>
    /// Generates a secure hash of the provided password using PBKDF2 with HMAC-SHA256.
    /// The result is a Base64 encoded string combining a randomly generated salt and the derived hash, separated by a dot.
    /// </summary>
    /// <param name="password">The plaintext password to be hashed. Must not be null.</param>
    /// <return>A string in the format "saltBase64.hashBase64" representing the salt and hash of the password.</return>
    public static string HashPassword(string password)
    {
        if (password == null) throw new ArgumentNullException("Cannot hash a null value");
        
        // Generate a random salt
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        // Derive the hash
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize
        );
        // Combine salt and hash as Base64
        string saltBase64 = Convert.ToBase64String(salt);
        string hashBase64 = Convert.ToBase64String(hash);

        return $"{saltBase64}.{hashBase64}";
    }

    /// <summary>
    /// Verifies whether the provided password matches the stored hash.
    /// </summary>
    /// <param name="password">The plain text password input by the user.</param>
    /// <param name="storedHash">The hashed password stored in the database, formatted as "salt.hash" using Base64 encoding.</param>
    /// <return>True if the password matches the stored hash, false otherwise.</return>
    public static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.', 2);
        if (parts.Length != 2) return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] expectedHash = Convert.FromBase64String(parts[1]);

        byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize
        );

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}