using CSIFlex.Application.DTOs;
using CSIFlex.Domain.Entities.Authentication;
using CSIFlex.Domain.Interfaces.Repositories;
using CSIFlex.Domain.Interfaces.Services;
using System.Security.Cryptography;

namespace CSIFlex.Application.Services;

/// <summary>
/// Serviço de aplicação para autenticação de usuários
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;

    public AuthenticationService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<User?> AuthenticateAsync(string userName, string password)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            return null;

        // Busca o usuário no banco de dados
        var user = await _userRepository.GetByUserNameAsync(userName);

        if (user == null)
            return null;

        // Verifica a senha
        if (!VerifyPassword(password, user.PasswordHash, user.Salt))
            return null;

        return user;
    }

    public bool VerifyPassword(string password, string storedHash, string salt)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash) || string.IsNullOrWhiteSpace(salt))
            return false;

        try
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] hashBytes = Convert.FromBase64String(storedHash);
            byte[] computedHash = GenerateHash(password, saltBytes);
            return CryptographicOperations.FixedTimeEquals(hashBytes, computedHash);
        }
        catch
        {
            return false;
        }
    }

    public (string hash, string salt) HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Senha não pode ser vazia", nameof(password));

        byte[] salt = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        byte[] hash = GenerateHash(password, salt);
        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    private static byte[] GenerateHash(string password, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(32);
    }

    /// <summary>
    /// Autentica um usuário e retorna o resultado com informações do usuário
    /// </summary>
    public async Task<AuthenticationResultDto> LoginAsync(LoginDto loginDto)
    {
        if (string.IsNullOrWhiteSpace(loginDto.UserName))
        {
            return new AuthenticationResultDto
            {
                Success = false,
                Message = "Nome de usuário é obrigatório"
            };
        }

        if (string.IsNullOrWhiteSpace(loginDto.Password))
        {
            return new AuthenticationResultDto
            {
                Success = false,
                Message = "Senha é obrigatória"
            };
        }

        // Verifica se é o usuário master
        if (loginDto.UserName.ToLower() == "csimasteradmin")
        {
            // TODO: Implementar lógica de usuário master se necessário
            return new AuthenticationResultDto
            {
                Success = false,
                Message = "Usuário master não implementado nesta versão"
            };
        }

        var user = await AuthenticateAsync(loginDto.UserName, loginDto.Password);

        if (user == null)
        {
            return new AuthenticationResultDto
            {
                Success = false,
                Message = "Nome de usuário ou senha inválidos"
            };
        }

        // Verifica se o usuário é administrador (apenas admins podem acessar o servidor)
        if (!user.IsAdmin)
        {
            return new AuthenticationResultDto
            {
                Success = false,
                Message = "Acesso negado. Apenas administradores podem acessar o sistema."
            };
        }

        return new AuthenticationResultDto
        {
            Success = true,
            Message = "Login realizado com sucesso",
            User = new UserDto
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DisplayName = user.DisplayName,
                Email = user.Email,
                UserType = user.UserType,
                IsAdmin = user.IsAdmin,
                Machines = user.Machines
            }
        };
    }
}
