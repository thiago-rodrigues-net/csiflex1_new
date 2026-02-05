using System;
using System.Security.Cryptography;

var password = args.Length > 0 ? args[0] : "admin123";

byte[] salt = new byte[32];
using (var rng = RandomNumberGenerator.Create())
{
    rng.GetBytes(salt);
}

using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
byte[] hash = pbkdf2.GetBytes(32);

Console.WriteLine($"Hash: {Convert.ToBase64String(hash)}");
Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");
