using System.Security.Cryptography;
using System.Text;

namespace Services.SecurityUtils;

public static class PasswordHasher
{
    /// Number of iterations for PBKDF2 (high enough for security)
    private const int Iterations = 100_000;

    /// Salt size in bytes
    private const int SaltSize = 16;

    /// Hash size in bytes
    private const int HashSize = 32;

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