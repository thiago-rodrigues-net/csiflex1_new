using CSIFlex.Domain.Entities.Authentication;
using CSIFlex.Domain.Interfaces.Repositories;
using CSIFlex.Infrastructure.Data;
using Dapper;

namespace CSIFlex.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de usuários usando Dapper
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly DatabaseContext _context;

    public UserRepository(DatabaseContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<User?> GetByUserNameAsync(string userName)
    {
        const string sql = @"
            SELECT 
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

        using var connection = await _context.CreateConnectionAsync();
        var result = await connection.QueryFirstOrDefaultAsync<UserDto>(sql, new { UserName = userName });

        if (result == null)
            return null;

        return User.Reconstruct(
            result.UserName,
            result.PasswordHash,
            result.Salt,
            result.FirstName,
            result.LastName,
            result.DisplayName,
            result.Email,
            result.UserType,
            result.RefId,
            result.Title,
            result.Dept,
            result.Machines,
            result.PhoneExt,
            result.EditTimeline,
            result.EditPartNumber
        );
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        const string sql = @"
            SELECT 
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
            FROM csi_auth.users";

        using var connection = await _context.CreateConnectionAsync();
        var results = await connection.QueryAsync<UserDto>(sql);

        return results.Select(r => User.Reconstruct(
            r.UserName,
            r.PasswordHash,
            r.Salt,
            r.FirstName,
            r.LastName,
            r.DisplayName,
            r.Email,
            r.UserType,
            r.RefId,
            r.Title,
            r.Dept,
            r.Machines,
            r.PhoneExt,
            r.EditTimeline,
            r.EditPartNumber
        ));
    }

    public async Task<bool> AddAsync(User user)
    {
        const string sql = @"
            INSERT INTO csi_auth.users
            (username_, password_, salt_, firstname_, Name_, displayname, email_, 
             usertype, refId, title, dept, machines, phoneext, EditTimeline, EditMasterPartData)
            VALUES
            (@UserName, @PasswordHash, @Salt, @FirstName, @LastName, @DisplayName, @Email,
             @UserType, @RefId, @Title, @Dept, @Machines, @PhoneExt, @EditTimeline, @EditPartNumber)";

        using var connection = await _context.CreateConnectionAsync();
        var rowsAffected = await connection.ExecuteAsync(sql, new
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

        return rowsAffected > 0;
    }

    public async Task<bool> UpdateAsync(User user)
    {
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
            WHERE username_ = @UserName";

        using var connection = await _context.CreateConnectionAsync();
        var rowsAffected = await connection.ExecuteAsync(sql, new
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

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(string userName)
    {
        const string sql = "DELETE FROM csi_auth.users WHERE username_ = @UserName";

        using var connection = await _context.CreateConnectionAsync();
        var rowsAffected = await connection.ExecuteAsync(sql, new { UserName = userName });

        return rowsAffected > 0;
    }

    public async Task<bool> ExistsAsync(string userName)
    {
        const string sql = "SELECT COUNT(1) FROM csi_auth.users WHERE username_ = @UserName";

        using var connection = await _context.CreateConnectionAsync();
        var count = await connection.ExecuteScalarAsync<int>(sql, new { UserName = userName });

        return count > 0;
    }

    /// <summary>
    /// DTO interno para mapeamento de dados do banco
    /// </summary>
    private class UserDto
    {
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
