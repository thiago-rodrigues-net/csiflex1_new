using CSIFlex.Domain.Entities.Authentication;

namespace CSIFlex.Domain.Interfaces.Repositories;

/// <summary>
/// Interface do repositório de usuários seguindo o padrão Repository
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Busca um usuário pelo nome de usuário
    /// </summary>
    Task<User?> GetByUserNameAsync(string userName);

    /// <summary>
    /// Busca todos os usuários
    /// </summary>
    Task<IEnumerable<User>> GetAllAsync();

    /// <summary>
    /// Adiciona um novo usuário
    /// </summary>
    Task<bool> AddAsync(User user);

    /// <summary>
    /// Atualiza um usuário existente
    /// </summary>
    Task<bool> UpdateAsync(User user);

    /// <summary>
    /// Remove um usuário
    /// </summary>
    Task<bool> DeleteAsync(string userName);

    /// <summary>
    /// Verifica se um usuário existe
    /// </summary>
    Task<bool> ExistsAsync(string userName);
}
