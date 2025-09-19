using Microsoft.EntityFrameworkCore;
using BeautyCenterApi.Data;
using BeautyCenterApi.Interfaces;
using BeautyCenterApi.Models;
using BeautyCenterApi.Services;

namespace BeautyCenterApi.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(BeautyCenterDbContext context, ITenantService tenantService)
            : base(context, tenantService)
        {
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            // Username ile giriş yaparken tenant filtresi uygulama (global olarak unique)
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            // Email ile giriş yaparken tenant filtresi uygulama (global olarak unique)
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query.Where(u => u.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            // SuperAdmin için tüm kullanıcıları döndür (tenant filtresi uygulama)
            return await _dbSet
                .Include(u => u.Tenant)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUserCountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByTenantAsync(int tenantId)
        {
            return await _dbSet.Where(u => u.TenantId == tenantId && u.IsActive).ToListAsync();
        }

        public async Task<bool> ValidatePasswordAsync(string username, string password)
        {
            var user = await GetByUsernameAsync(username);
            if (user == null) return false;

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }
    }
}