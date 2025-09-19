using BeautyCenterApi.Models;

namespace BeautyCenterApi.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetActiveUsersAsync();
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<bool> ValidatePasswordAsync(string username, string password);
        Task<int> GetUserCountAsync();
        Task<IEnumerable<User>> GetUsersByTenantAsync(int tenantId);
    }
}