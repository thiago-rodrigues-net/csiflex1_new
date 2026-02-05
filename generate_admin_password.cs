// Utilitário para gerar hash de senha para o usuário admin inicial
// Compile e execute: dotnet script generate_admin_password.cs

using System;
using System.Security.Cryptography;

class PasswordGenerator
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Gerador de Hash de Senha CSIFLEX ===\n");
        
        string password = "admin123"; // Senha padrão inicial
        
        if (args.Length > 0)
        {
            password = args[0];
        }
        
        Console.WriteLine($"Gerando hash para a senha: {password}");
        Console.WriteLine("Algoritmo: PBKDF2-SHA256");
        Console.WriteLine("Iterações: 10000");
        Console.WriteLine("Salt Size: 32 bytes");
        Console.WriteLine("Hash Size: 32 bytes\n");
        
        var (hash, salt) = HashPassword(password);
        
        Console.WriteLine("=== RESULTADO ===");
        Console.WriteLine($"Hash: {hash}");
        Console.WriteLine($"Salt: {salt}");
        Console.WriteLine("\n=== SQL UPDATE ===");
        Console.WriteLine($"UPDATE csi_auth.users SET");
        Console.WriteLine($"  password_ = '{hash}',");
        Console.WriteLine($"  salt_ = '{salt}'");
        Console.WriteLine($"WHERE username_ = 'admin';");
        Console.WriteLine("\n⚠️  IMPORTANTE: Altere a senha após o primeiro login!");
    }
    
    static (string hash, string salt) HashPassword(string password)
    {
        // Gera um salt aleatório
        byte[] salt = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        
        // Gera o hash usando PBKDF2
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            10000,
            HashAlgorithmName.SHA256);
        
        byte[] hash = pbkdf2.GetBytes(32);
        
        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }
}
