using CSIFlex.Domain.Entities.Authentication;
using CSIFlex.Domain.Interfaces.Repositories;
using CSIFlex.Domain.Interfaces.Services;
using CSIFlex.Application.Utilities;
using Microsoft.Extensions.Logging;

namespace CSIFlex.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
        _logger.LogDebug("UserService inicializado");
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        _logger.LogInformation("Buscando todos os usuários");
        try
        {
            var users = await _userRepository.GetAllAsync();
            _logger.LogInformation("Encontrados {Count} usuários", users.Count());
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar todos os usuários");
            throw;
        }
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        _logger.LogDebug("Buscando usuário por ID: {UserId}", id);
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Usuário não encontrado com ID: {UserId}", id);
            }
            else
            {
                _logger.LogDebug("Usuário encontrado: {UserName}", user.UserName);
            }
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário por ID: {UserId}", id);
            throw;
        }
    }

    public async Task<User?> GetUserByUserNameAsync(string userName)
    {
        _logger.LogDebug("Buscando usuário por nome: {UserName}", userName);
        try
        {
            var user = await _userRepository.GetByUserNameAsync(userName);
            if (user == null)
            {
                _logger.LogWarning("Usuário não encontrado: {UserName}", userName);
            }
            else
            {
                _logger.LogDebug("Usuário encontrado: {UserName}", user.UserName);
            }
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário por nome: {UserName}", userName);
            throw;
        }
    }

    public async Task<User> CreateUserAsync(User user, string password)
    {
        _logger.LogInformation("Criando novo usuário: {UserName}", user.UserName);
        
        try
        {
            if (await _userRepository.ExistsByUserNameAsync(user.UserName))
            {
                _logger.LogWarning("Tentativa de criar usuário com nome já existente: {UserName}", user.UserName);
                throw new InvalidOperationException($"Nome de usuário '{user.UserName}' já está em uso");
            }

            if (await _userRepository.ExistsByEmailAsync(user.Email))
            {
                _logger.LogWarning("Tentativa de criar usuário com e-mail já existente: {Email}", user.Email);
                throw new InvalidOperationException($"E-mail '{user.Email}' já está em uso");
            }

            var salt = PasswordHelper.GenerateSalt();
            var hash = PasswordHelper.HashPassword(password, salt);
            
            user.Salt = salt;
            user.PasswordHash = hash;
            user.CreatedAt = DateTime.Now;
            user.IsActive = true;

            if (string.IsNullOrWhiteSpace(user.DisplayName))
            {
                user.DisplayName = $"{user.FirstName} {user.LastName}";
            }

            var createdUser = await _userRepository.AddAsync(user);
            _logger.LogInformation("Usuário criado com sucesso: {UserName} (ID: {UserId})", createdUser.UserName, createdUser.Id);
            
            return createdUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar usuário: {UserName}", user.UserName);
            throw;
        }
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        _logger.LogInformation("Atualizando usuário: {UserName} (ID: {UserId})", user.UserName, user.Id);
        
        try
        {
            var existingUser = await _userRepository.GetByIdAsync(user.Id);
            if (existingUser == null)
            {
                _logger.LogWarning("Tentativa de atualizar usuário inexistente: ID {UserId}", user.Id);
                throw new InvalidOperationException($"Usuário com ID {user.Id} não encontrado");
            }

            if (await _userRepository.ExistsByEmailAsync(user.Email, user.Id))
            {
                _logger.LogWarning("Tentativa de atualizar usuário com e-mail já existente: {Email}", user.Email);
                throw new InvalidOperationException($"E-mail '{user.Email}' já está em uso por outro usuário");
            }

            user.UpdatedAt = DateTime.Now;
            user.UserName = existingUser.UserName;
            user.PasswordHash = existingUser.PasswordHash;
            user.Salt = existingUser.Salt;
            user.CreatedAt = existingUser.CreatedAt;

            if (string.IsNullOrWhiteSpace(user.DisplayName))
            {
                user.DisplayName = $"{user.FirstName} {user.LastName}";
            }

            var updatedUser = await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Usuário atualizado com sucesso: {UserName} (ID: {UserId})", updatedUser.UserName, updatedUser.Id);
            
            return updatedUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar usuário: ID {UserId}", user.Id);
            throw;
        }
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        _logger.LogInformation("Excluindo usuário: ID {UserId}", id);
        
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Tentativa de excluir usuário inexistente: ID {UserId}", id);
                return false;
            }

            var result = await _userRepository.DeleteAsync(id);
            
            if (result)
            {
                _logger.LogInformation("Usuário excluído com sucesso: {UserName} (ID: {UserId})", user.UserName, id);
            }
            else
            {
                _logger.LogWarning("Falha ao excluir usuário: ID {UserId}", id);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir usuário: ID {UserId}", id);
            throw;
        }
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        _logger.LogInformation("Alterando senha do usuário: ID {UserId}", userId);
        
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Tentativa de alterar senha de usuário inexistente: ID {UserId}", userId);
                return false;
            }

            var currentHash = PasswordHelper.HashPassword(currentPassword, user.Salt);
            if (currentHash != user.PasswordHash)
            {
                _logger.LogWarning("Senha atual incorreta para o usuário: {UserName}", user.UserName);
                return false;
            }

            var newSalt = PasswordHelper.GenerateSalt();
            var newHash = PasswordHelper.HashPassword(newPassword, newSalt);
            
            user.Salt = newSalt;
            user.PasswordHash = newHash;
            user.UpdatedAt = DateTime.Now;

            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Senha alterada com sucesso para o usuário: {UserName}", user.UserName);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar senha do usuário: ID {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
    {
        _logger.LogInformation("Resetando senha do usuário: ID {UserId}", userId);
        
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Tentativa de resetar senha de usuário inexistente: ID {UserId}", userId);
                return false;
            }

            var newSalt = PasswordHelper.GenerateSalt();
            var newHash = PasswordHelper.HashPassword(newPassword, newSalt);
            
            user.Salt = newSalt;
            user.PasswordHash = newHash;
            user.UpdatedAt = DateTime.Now;

            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Senha resetada com sucesso para o usuário: {UserName}", user.UserName);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao resetar senha do usuário: ID {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> UserNameExistsAsync(string userName, int? excludeUserId = null)
    {
        _logger.LogDebug("Verificando existência do nome de usuário: {UserName}", userName);
        return await _userRepository.ExistsByUserNameAsync(userName, excludeUserId);
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
    {
        _logger.LogDebug("Verificando existência do e-mail: {Email}", email);
        return await _userRepository.ExistsByEmailAsync(email, excludeUserId);
    }

    public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
    {
        _logger.LogInformation("Buscando usuários com termo: {SearchTerm}", searchTerm);
        try
        {
            var users = await _userRepository.SearchAsync(searchTerm);
            _logger.LogInformation("Encontrados {Count} usuários com o termo: {SearchTerm}", users.Count(), searchTerm);
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuários com termo: {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<IEnumerable<User>> GetUsersByTypeAsync(string userType)
    {
        _logger.LogInformation("Buscando usuários do tipo: {UserType}", userType);
        try
        {
            var users = await _userRepository.GetByTypeAsync(userType);
            _logger.LogInformation("Encontrados {Count} usuários do tipo: {UserType}", users.Count(), userType);
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuários do tipo: {UserType}", userType);
            throw;
        }
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        _logger.LogInformation("Buscando usuários ativos");
        try
        {
            var users = await _userRepository.GetActiveAsync();
            _logger.LogInformation("Encontrados {Count} usuários ativos", users.Count());
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuários ativos");
            throw;
        }
    }
}
