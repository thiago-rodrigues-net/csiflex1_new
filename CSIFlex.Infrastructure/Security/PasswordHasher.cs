using System.Security.Cryptography;
using System.Text;

namespace CSIFlex.Infrastructure.Security;

/// <summary>
/// Utilitário para hash de senhas usando PBKDF2
/// Compatível com o sistema original VB.NET
/// </summary>
public static class PasswordHasher
{
    private const int SaltSize = 32; // 256 bits
    private const int HashSize = 32; // 256 bits
    private const int Iterations = 10000; // Número de iterações PBKDF2

    /// <summary>
    /// Gera um hash de senha usando PBKDF2
    /// </summary>
    /// <param name="password">Senha em texto plano</param>
    /// <returns>Tupla contendo o hash em Base64 e o salt em Base64</returns>
    public static (string hash, string salt) HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Senha não pode ser vazia", nameof(password));

        // Gera um salt aleatório
        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Gera o hash usando PBKDF2
        byte[] hash = GenerateHash(password, salt);

        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    /// <summary>
    /// Verifica se uma senha corresponde ao hash armazenado
    /// </summary>
    /// <param name="password">Senha em texto plano</param>
    /// <param name="storedHash">Hash armazenado em Base64</param>
    /// <param name="storedSalt">Salt armazenado em Base64</param>
    /// <returns>True se a senha for válida</returns>
    public static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        if (string.IsNullOrWhiteSpace(storedHash) || string.IsNullOrWhiteSpace(storedSalt))
            return false;

        try
        {
            byte[] salt = Convert.FromBase64String(storedSalt);
            byte[] hash = Convert.FromBase64String(storedHash);

            byte[] computedHash = GenerateHash(password, salt);

            // Comparação constante para evitar timing attacks
            return CryptographicOperations.FixedTimeEquals(hash, computedHash);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gera o hash PBKDF2 da senha
    /// </summary>
    private static byte[] GenerateHash(string password, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256);

        return pbkdf2.GetBytes(HashSize);
    }
}
