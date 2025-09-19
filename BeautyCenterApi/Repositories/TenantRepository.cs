using BeautyCenterApi.Data;
using BeautyCenterApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BeautyCenterApi.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly BeautyCenterDbContext _context;

        public TenantRepository(BeautyCenterDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tenant>> GetAllTenantsAsync()
        {
            return await _context.Tenants
                .Include(t => t.Users)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<Tenant?> GetTenantByIdAsync(int id)
        {
            return await _context.Tenants
                .Include(t => t.Users)
                .Include(t => t.Customers)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tenant?> GetTenantBySubDomainAsync(string subDomain)
        {
            return await _context.Tenants
                .FirstOrDefaultAsync(t => t.SubDomain == subDomain);
        }

        public async Task<Tenant> CreateTenantAsync(Tenant tenant)
        {
            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();
            return tenant;
        }

        public async Task<Tenant> UpdateTenantAsync(Tenant tenant)
        {
            tenant.UpdatedAt = DateTime.UtcNow;
            _context.Entry(tenant).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return tenant;
        }

        public async Task<bool> DeleteTenantAsync(int id)
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null)
                return false;

            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TenantExistsAsync(int id)
        {
            return await _context.Tenants.AnyAsync(t => t.Id == id);
        }

        public async Task<bool> SubDomainExistsAsync(string subDomain)
        {
            return await _context.Tenants.AnyAsync(t => t.SubDomain == subDomain);
        }

        public async Task<IEnumerable<Tenant>> GetActiveTenantsAsync()
        {
            return await _context.Tenants
                .Where(t => t.IsActive)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<int> GetTenantUserCountAsync(int tenantId)
        {
            return await _context.Users
                .CountAsync(u => u.TenantId == tenantId && u.IsActive);
        }

        public async Task<int> GetTenantCustomerCountAsync(int tenantId)
        {
            return await _context.Customers
                .CountAsync(c => c.TenantId == tenantId && c.IsActive);
        }
    }
}