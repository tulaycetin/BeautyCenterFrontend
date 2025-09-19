using Microsoft.EntityFrameworkCore;
using BeautyCenterApi.Data;
using BeautyCenterApi.Interfaces;
using BeautyCenterApi.Models;
using BeautyCenterApi.Services;

namespace BeautyCenterApi.Repositories
{
    public class ServiceTypeRepository : GenericRepository<ServiceType>, IServiceTypeRepository
    {
        public ServiceTypeRepository(BeautyCenterDbContext context, ITenantService tenantService)
            : base(context, tenantService)
        {
        }

        public async Task<IEnumerable<ServiceType>> GetActiveServicesAsync()
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query.Where(s => s.IsActive).ToListAsync();
        }

        public async Task<ServiceType?> GetByNameAsync(string name)
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query.FirstOrDefaultAsync(s => s.Name == name);
        }

        public async Task<IEnumerable<ServiceType>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query
                .Where(s => s.Price >= minPrice && s.Price <= maxPrice && s.IsActive)
                .ToListAsync();
        }
    }
}