using CSIFlex.Application.DTOs;
using CSIFlex.Domain.Entities.Authentication;
using CSIFlex.Domain.Interfaces.Repositories;
using CSIFlex.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace CSIFlex.Application.Services;

/// <summary>
/// Serviço de aplicação para autenticação de usuários
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IUserRepository userRepository,
        ILogger<AuthenticationService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _logger.LogDebug("AuthenticationService inicializado");
    }

    public async Task<User?> AuthenticateAsync(string userName, string password)
    {
        _logger.LogDebug("Tentativa de autenticação para o usuário: {UserName}", userName);

        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Tentativa de autenticação com credenciais vazias");
            return null;
        }

        try
        {
            // Busca o usuário no banco de dados
            var user = await _userRepository.GetByUserNameAsync(userName);

            if (user == null)
            {
                _logger.LogWarning("Usuário não encontrado: {UserName}", userName);
                return null;
            }

            // Verifica a senha
            if (!VerifyPassword(password, user.PasswordHash, user.Salt))
            {
                _logger.LogWarning("Senha inválida para o usuário: {UserName}", userName);
                return null;
            }

            _logger.LogInformation("Usuário autenticado com sucesso: {UserName}", userName);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao autenticar usuário: {UserName}", userName);
            throw;
        }
    }

    public bool VerifyPassword(string password, string storedHash, string salt)
    {
        _logger.LogDebug("Verificando senha");

        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash) || string.IsNullOrWhiteSpace(salt))
        {
            _logger.LogWarning("Tentativa de verificação de senha com parâmetros vazios");
            return false;
        }

        try
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] hashBytes = Convert.FromBase64String(storedHash);
            byte[] computedHash = GenerateHash(password, saltBytes);
            bool isValid = CryptographicOperations.FixedTimeEquals(hashBytes, computedHash);
            
            _logger.LogDebug("Senha verificada: {IsValid}", isValid);
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar senha");
            return false;
        }
    }

    public (string hash, string salt) HashPassword(string password)
    {
        _logger.LogDebug("Gerando hash de senha");

        if (string.IsNullOrWhiteSpace(password))
        {
            _logger.LogError("Tentativa de gerar hash com senha vazia");
            throw new ArgumentException("Senha não pode ser vazia", nameof(password));
        }

        try
        {
            byte[] salt = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash = GenerateHash(password, salt);
            _logger.LogDebug("Hash de senha gerado com sucesso");
            
            return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar hash de senha");
            throw;
        }
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
        _logger.LogInformation("Tentativa de login para o usuário: {UserName}", loginDto.UserName);

        if (string.IsNullOrWhiteSpace(loginDto.UserName))
        {
            _logger.LogWarning("Tentativa de login sem nome de usuário");
            return new AuthenticationResultDto
            {
                Success = false,
                Message = "Nome de usuário é obrigatório"
            };
        }

        if (string.IsNullOrWhiteSpace(loginDto.Password))
        {
            _logger.LogWarning("Tentativa de login sem senha para o usuário: {UserName}", loginDto.UserName);
            return new AuthenticationResultDto
            {
                Success = false,
                Message = "Senha é obrigatória"
            };
        }

        // Verifica se é o usuário master
        if (loginDto.UserName.ToLower() == "csimasteradmin")
        {
            _logger.LogWarning("Tentativa de login com usuário master (não implementado): {UserName}", loginDto.UserName);
            return new AuthenticationResultDto
            {
                Success = false,
                Message = "Usuário master não implementado nesta versão"
            };
        }

        try
        {
            var user = await AuthenticateAsync(loginDto.UserName, loginDto.Password);

            if (user == null)
            {
                _logger.LogWarning("Falha no login - Credenciais inválidas para o usuário: {UserName}", loginDto.UserName);
                return new AuthenticationResultDto
                {
                    Success = false,
                    Message = "Nome de usuário ou senha inválidos"
                };
            }

            // Verifica se o usuário é administrador (apenas admins podem acessar o servidor)
            if (!user.IsAdmin)
            {
                _logger.LogWarning("Falha no login - Usuário sem permissão de administrador: {UserName} (Tipo: {UserType})", 
                    loginDto.UserName, user.UserType);
                return new AuthenticationResultDto
                {
                    Success = false,
                    Message = "Acesso negado. Apenas administradores podem acessar o sistema."
                };
            }

            _logger.LogInformation("Login realizado com sucesso para o usuário: {UserName} (Tipo: {UserType})", 
                user.UserName, user.UserType);

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico durante o processo de login para o usuário: {UserName}", loginDto.UserName);
            return new AuthenticationResultDto
            {
                Success = false,
                Message = "Erro ao processar login. Por favor, tente novamente."
            };
        }
    }
}
