using BeautyCenterApi.Models;

namespace BeautyCenterApi.Repositories
{
    public interface ITenantRepository
    {
        Task<IEnumerable<Tenant>> GetAllTenantsAsync();
        Task<Tenant?> GetTenantByIdAsync(int id);
        Task<Tenant?> GetTenantBySubDomainAsync(string subDomain);
        Task<Tenant> CreateTenantAsync(Tenant tenant);
        Task<Tenant> UpdateTenantAsync(Tenant tenant);
        Task<bool> DeleteTenantAsync(int id);
        Task<bool> TenantExistsAsync(int id);
        Task<bool> SubDomainExistsAsync(string subDomain);
        Task<IEnumerable<Tenant>> GetActiveTenantsAsync();
        Task<int> GetTenantUserCountAsync(int tenantId);
        Task<int> GetTenantCustomerCountAsync(int tenantId);
    }
}