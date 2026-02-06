using CSIFlex.Domain.Entities.Authentication;

namespace CSIFlex.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUserNameAsync(string userName);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<IEnumerable<User>> GetActiveAsync();
    Task<IEnumerable<User>> GetByTypeAsync(string userType);
    Task<IEnumerable<User>> SearchAsync(string searchTerm);
    Task<User> AddAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsByUserNameAsync(string userName, int? excludeUserId = null);
    Task<bool> ExistsByEmailAsync(string email, int? excludeUserId = null);
}
