using CSIFlex.Domain.Entities.Authentication;

namespace CSIFlex.Domain.Interfaces.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByUserNameAsync(string userName);
    Task<User> CreateUserAsync(User user, string password);
    Task<User> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<bool> ResetPasswordAsync(int userId, string newPassword);
    Task<bool> UserNameExistsAsync(string userName, int? excludeUserId = null);
    Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);
    Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);
    Task<IEnumerable<User>> GetUsersByTypeAsync(string userType);
    Task<IEnumerable<User>> GetActiveUsersAsync();
}
