using CSIFlex.Domain.Entities.Authentication;

namespace CSIFlex.Domain.Interfaces.Services;

/// <summary>
/// Interface do serviço de autenticação
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Autentica um usuário com nome de usuário e senha
    /// </summary>
    /// <param name="userName">Nome de usuário</param>
    /// <param name="password">Senha em texto plano</param>
    /// <returns>Usuário autenticado ou null se as credenciais forem inválidas</returns>
    Task<User?> AuthenticateAsync(string userName, string password);

    /// <summary>
    /// Verifica se a senha fornecida corresponde ao hash armazenado
    /// </summary>
    /// <param name="password">Senha em texto plano</param>
    /// <param name="storedHash">Hash armazenado</param>
    /// <param name="salt">Salt usado na geração do hash</param>
    /// <returns>True se a senha for válida</returns>
    bool VerifyPassword(string password, string storedHash, string salt);

    /// <summary>
    /// Gera um hash de senha usando PBKDF2
    /// </summary>
    /// <param name="password">Senha em texto plano</param>
    /// <returns>Tupla contendo o hash e o salt</returns>
    (string hash, string salt) HashPassword(string password);
}
