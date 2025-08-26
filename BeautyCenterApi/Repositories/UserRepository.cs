using Microsoft.EntityFrameworkCore;
using BeautyCenterApi.Data;
using BeautyCenterApi.Interfaces;
using BeautyCenterApi.Models;

namespace BeautyCenterApi.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(BeautyCenterDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _dbSet.Where(u => u.IsActive).ToListAsync();
        }

        public async Task<bool> ValidatePasswordAsync(string username, string password)
        {
            var user = await GetByUsernameAsync(username);
            if (user == null) return false;

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }
    }
}