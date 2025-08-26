using Microsoft.EntityFrameworkCore;
using BeautyCenterApi.Data;
using BeautyCenterApi.Interfaces;
using BeautyCenterApi.Models;

namespace BeautyCenterApi.Repositories
{
    public class ServiceTypeRepository : GenericRepository<ServiceType>, IServiceTypeRepository
    {
        public ServiceTypeRepository(BeautyCenterDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ServiceType>> GetActiveServicesAsync()
        {
            return await _dbSet.Where(s => s.IsActive).ToListAsync();
        }

        public async Task<ServiceType?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.Name == name);
        }

        public async Task<IEnumerable<ServiceType>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _dbSet
                .Where(s => s.Price >= minPrice && s.Price <= maxPrice && s.IsActive)
                .ToListAsync();
        }
    }
}