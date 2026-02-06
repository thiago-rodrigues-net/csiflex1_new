using System.Security.Cryptography;
using System.Text;

namespace CSIFlex.Application.Utilities;

public static class PasswordHelper
{
    public static string GenerateSalt()
    {
        var saltBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    public static string HashPassword(string password, string salt)
    {
        const int iterations = 10000;
        const int hashSize = 32;

        using var pbkdf2 = new Rfc2898DeriveBytes(
            Encoding.UTF8.GetBytes(password),
            Convert.FromBase64String(salt),
            iterations,
            HashAlgorithmName.SHA256
        );

        var hash = pbkdf2.GetBytes(hashSize);
        return Convert.ToBase64String(hash);
    }

    public static bool VerifyPassword(string password, string salt, string hash)
    {
        var computedHash = HashPassword(password, salt);
        return computedHash == hash;
    }
}
