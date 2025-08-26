using BeautyCenterApi.Models;

namespace BeautyCenterApi.Interfaces
{
    public interface IServiceTypeRepository : IGenericRepository<ServiceType>
    {
        Task<IEnumerable<ServiceType>> GetActiveServicesAsync();
        Task<ServiceType?> GetByNameAsync(string name);
        Task<IEnumerable<ServiceType>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    }
}