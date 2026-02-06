using CSIFlex.Domain.Entities.Authentication;
using CSIFlex.Domain.Interfaces.Repositories;
using CSIFlex.Infrastructure.Data;
using Dapper;
using Microsoft.Extensions.Logging;

namespace CSIFlex.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DatabaseContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(DatabaseContext context, ILogger<UserRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _logger.LogDebug("UserRepository inicializado");
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        _logger.LogDebug("Buscando usuário por ID: {UserId}", id);
        
        const string sql = @"
            SELECT 
                Id,
                username_ AS UserName,
                password_ AS PasswordHash,
                salt_ AS Salt,
                firstname_ AS FirstName,
                Name_ AS LastName,
                displayname AS DisplayName,
                email_ AS Email,
                usertype AS UserType,
                refId AS RefId,
                title AS Title,
                dept AS Dept,
                machines AS Machines,
                phoneext AS PhoneExt,
                EditTimeline,
                EditMasterPartData AS EditPartNumber
            FROM csi_auth.users
            WHERE Id = @Id";

        try
        {
            using var connection = await _context.CreateConnectionAsync();
            var result = await connection.QueryFirstOrDefaultAsync<UserDbDto>(sql, new { Id = id });

            if (result == null)
            {
                _logger.LogDebug("Usuário não encontrado com ID: {UserId}", id);
                return null;
            }
            
            _logger.LogDebug("Usuário encontrado: {UserName} (ID: {UserId})", result.UserName, id);
            return MapToEntity(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário por ID: {UserId}", id);
            throw;
        }
    }

    public async Task<User?> GetByUserNameAsync(string userName)
    {
        _logger.LogDebug("Buscando usuário por nome: {UserName}", userName);
        
        const string sql = @"
            SELECT 
                Id,
                username_ AS UserName,
                password_ AS PasswordHash,
                salt_ AS Salt,
                firstname_ AS FirstName,
                Name_ AS LastName,
                displayname AS DisplayName,
                email_ AS Email,
                usertype AS UserType,
                refId AS RefId,
                title AS Title,
                dept AS Dept,
                machines AS Machines,
                phoneext AS PhoneExt,
                EditTimeline,
                EditMasterPartData AS EditPartNumber
            FROM csi_auth.users
            WHERE username_ = @UserName";

        try
        {
            using var connection = await _context.CreateConnectionAsync();
            var result = await connection.QueryFirstOrDefaultAsync<UserDbDto>(sql, new { UserName = userName });

            if (result == null)
            {
                _logger.LogDebug("Usuário não encontrado: {UserName}", userName);
                return null;
            }
            
            _logger.LogDebug("Usuário encontrado: {UserName}", userName);
            return MapToEntity(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário: {UserName}", userName);
            throw;
        }
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        _logger.LogDebug("Buscando usuário por e-mail: {Email}", email);
        
        const string sql = @"
            SELECT 
                Id,
                username_ AS UserName,
                password_ AS PasswordHash,
                salt_ AS Salt,
                firstname_ AS FirstName,
                Name_ AS LastName,
                displayname AS DisplayName,
                email_ AS Email,
                usertype AS UserType,
                refId AS RefId,
                title AS Title,
                dept AS Dept,
                machines AS Machines,
                phoneext AS PhoneExt,
                EditTimeline,
                EditMasterPartData AS EditPartNumber
            FROM csi_auth.users
            WHERE email_ = @Email";

        try
        {
            using var connection = await _context.CreateConnectionAsync();
            var result = await connection.QueryFirstOrDefaultAsync<UserDbDto>(sql, new { Email = email });

            if (result == null)
            {
                _logger.LogDebug("Usuário não encontrado com e-mail: {Email}", email);
                return null;
            }
            
            _logger.LogDebug("Usuário encontrado: {UserName}", result.UserName);
            return MapToEntity(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário por e-mail: {Email}", email);
            throw;
        }
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        _logger.LogDebug("Buscando todos os usuários");
        
        const string sql = @"
            SELECT 
                Id,
                username_ AS UserName,
                password_ AS PasswordHash,
                salt_ AS Salt,
                firstname_ AS FirstName,
                Name_ AS LastName,
                displayname AS DisplayName,
                email_ AS Email,
                usertype AS UserType,
                refId AS RefId,
                title AS Title,
                dept AS Dept,
                machines AS Machines,
                phoneext AS PhoneExt,
                EditTimeline,
                EditMasterPartData AS EditPartNumber
            FROM csi_auth.users
            ORDER BY username_";

        try
        {
            using var connection = await _context.CreateConnectionAsync();
            var results = await connection.QueryAsync<UserDbDto>(sql);
            return results.Select(MapToEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar todos os usuários");
            throw;
        }
    }

    public async Task<IEnumerable<User>> GetActiveAsync()
    {
        _logger.LogDebug("Buscando usuários ativos");
        return await GetAllAsync();
    }

    public async Task<IEnumerable<User>> GetByTypeAsync(string userType)
    {
        _logger.LogDebug("Buscando usuários do tipo: {UserType}", userType);
        
        const string sql = @"
            SELECT 
                Id,
                username_ AS UserName,
                password_ AS PasswordHash,
                salt_ AS Salt,
                firstname_ AS FirstName,
                Name_ AS LastName,
                displayname AS DisplayName,
                email_ AS Email,
                usertype AS UserType,
                refId AS RefId,
                title AS Title,
                dept AS Dept,
                machines AS Machines,
                phoneext AS PhoneExt,
                EditTimeline,
                EditMasterPartData AS EditPartNumber
            FROM csi_auth.users
            WHERE usertype = @UserType
            ORDER BY username_";

        try
        {
            using var connection = await _context.CreateConnectionAsync();
            var results = await connection.QueryAsync<UserDbDto>(sql, new { UserType = userType });
            return results.Select(MapToEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuários do tipo: {UserType}", userType);
            throw;
        }
    }

    public async Task<IEnumerable<User>> SearchAsync(string searchTerm)
    {
        _logger.LogDebug("Buscando usuários com termo: {SearchTerm}", searchTerm);
        
        const string sql = @"
            SELECT 
                Id,
                username_ AS UserName,
                password_ AS PasswordHash,
                salt_ AS Salt,
                firstname_ AS FirstName,
                Name_ AS LastName,
                displayname AS DisplayName,
                email_ AS Email,
                usertype AS UserType,
                refId AS RefId,
                title AS Title,
                dept AS Dept,
                machines AS Machines,
                phoneext AS PhoneExt,
                EditTimeline,
                EditMasterPartData AS EditPartNumber
            FROM csi_auth.users
            WHERE username_ LIKE @SearchTerm
               OR firstname_ LIKE @SearchTerm
               OR Name_ LIKE @SearchTerm
               OR displayname LIKE @SearchTerm
               OR email_ LIKE @SearchTerm
            ORDER BY username_";

        try
        {
            using var connection = await _context.CreateConnectionAsync();
            var searchPattern = $"%{searchTerm}%";
            var results = await connection.QueryAsync<UserDbDto>(sql, new { SearchTerm = searchPattern });
            return results.Select(MapToEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuários com termo: {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<User> AddAsync(User user)
    {
        _logger.LogDebug("Adicionando novo usuário: {UserName}", user.UserName);
        
        const string sql = @"
            INSERT INTO csi_auth.users
            (username_, password_, salt_, firstname_, Name_, displayname, email_, 
             usertype, refId, title, dept, machines, phoneext, EditTimeline, EditMasterPartData)
            VALUES
            (@UserName, @PasswordHash, @Salt, @FirstName, @LastName, @DisplayName, @Email,
             @UserType, @RefId, @Title, @Dept, @Machines, @PhoneExt, @EditTimeline, @EditPartNumber);
            SELECT LAST_INSERT_ID();";

        try
        {
            using var connection = await _context.CreateConnectionAsync();
            var userId = await connection.ExecuteScalarAsync<int>(sql, new
            {
                user.UserName,
                user.PasswordHash,
                user.Salt,
                user.FirstName,
                user.LastName,
                user.DisplayName,
                user.Email,
                user.UserType,
                user.RefId,
                user.Title,
                user.Dept,
                user.Machines,
                user.PhoneExt,
                user.EditTimeline,
                user.EditPartNumber
            });

            _logger.LogInformation("Usuário adicionado com sucesso: {UserName} (ID: {UserId})", user.UserName, userId);
            
            var createdUser = await GetByIdAsync(userId);
            return createdUser!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar usuário: {UserName}", user.UserName);
            throw;
        }
    }

    public async Task<User> UpdateAsync(User user)
    {
        _logger.LogDebug("Atualizando usuário: {UserName} (ID: {UserId})", user.UserName, user.Id);
        
        const string sql = @"
            UPDATE csi_auth.users
            SET 
                password_ = @PasswordHash,
                salt_ = @Salt,
                firstname_ = @FirstName,
                Name_ = @LastName,
                displayname = @DisplayName,
                email_ = @Email,
                usertype = @UserType,
                refId = @RefId,
                title = @Title,
                dept = @Dept,
                machines = @Machines,
                phoneext = @PhoneExt,
                EditTimeline = @EditTimeline,
                EditMasterPartData = @EditPartNumber
            WHERE Id = @Id";

        try
        {
            using var connection = await _context.CreateConnectionAsync();
            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                user.Id,
                user.PasswordHash,
                user.Salt,
                user.FirstName,
                user.LastName,
                user.DisplayName,
                user.Email,
                user.UserType,
                user.RefId,
                user.Title,
                user.Dept,
                user.Machines,
                user.PhoneExt,
                user.EditTimeline,
                user.EditPartNumber
            });

            if (rowsAffected == 0)
            {
                _logger.LogWarning("Nenhuma linha afetada ao atualizar usuário: ID {UserId}", user.Id);
                throw new InvalidOperationException($"Usuário com ID {user.Id} não encontrado");
            }

            _logger.LogInformation("Usuário atualizado com sucesso: {UserName} (ID: {UserId})", user.UserName, user.Id);
            
            var updatedUser = await GetByIdAsync(user.Id);
            return updatedUser!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar usuário: ID {UserId}", user.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogDebug("Excluindo usuário: ID {UserId}", id);
        
        const string sql = "DELETE FROM csi_auth.users WHERE Id = @Id";

        try
        {
            using var connection = await _context.CreateConnectionAsync();
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });

            if (rowsAffected > 0)
            {
                _logger.LogInformation("Usuário excluído com sucesso: ID {UserId}", id);
            }
            else
            {
                _logger.LogWarning("Nenhuma linha afetada ao excluir usuário: ID {UserId}", id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir usuário: ID {UserId}", id);
            throw;
        }
    }

    public async Task<bool> ExistsByUserNameAsync(string userName, int? excludeUserId = null)
    {
        _logger.LogDebug("Verificando existência do nome de usuário: {UserName}", userName);
        
        var sql = "SELECT COUNT(1) FROM csi_auth.users WHERE username_ = @UserName";
        
        if (excludeUserId.HasValue)
        {
            sql += " AND Id != @ExcludeUserId";
        }

        try
        {
            using var connection = await _context.CreateConnectionAsync();
            var count = await connection.ExecuteScalarAsync<int>(sql, new 
            { 
                UserName = userName,
                ExcludeUserId = excludeUserId
            });

            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência do nome de usuário: {UserName}", userName);
            throw;
        }
    }

    public async Task<bool> ExistsByEmailAsync(string email, int? excludeUserId = null)
    {
        _logger.LogDebug("Verificando existência do e-mail: {Email}", email);
        
        var sql = "SELECT COUNT(1) FROM csi_auth.users WHERE email_ = @Email";
        
        if (excludeUserId.HasValue)
        {
            sql += " AND Id != @ExcludeUserId";
        }

        try
        {
            using var connection = await _context.CreateConnectionAsync();
            var count = await connection.ExecuteScalarAsync<int>(sql, new 
            { 
                Email = email,
                ExcludeUserId = excludeUserId
            });

            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência do e-mail: {Email}", email);
            throw;
        }
    }

    private User MapToEntity(UserDbDto dto)
    {
        var user = User.Reconstruct(
            dto.UserName,
            dto.PasswordHash,
            dto.Salt,
            dto.FirstName,
            dto.LastName,
            dto.DisplayName,
            dto.Email,
            dto.UserType,
            dto.RefId,
            dto.Title,
            dto.Dept,
            dto.Machines,
            dto.PhoneExt,
            dto.EditTimeline,
            dto.EditPartNumber
        );
        
        typeof(User).GetProperty("Id")!.SetValue(user, dto.Id);
        
        return user;
    }

    private class UserDbDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
        public string RefId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Dept { get; set; } = string.Empty;
        public string Machines { get; set; } = string.Empty;
        public string PhoneExt { get; set; } = string.Empty;
        public bool EditTimeline { get; set; }
        public bool EditPartNumber { get; set; }
    }
}
