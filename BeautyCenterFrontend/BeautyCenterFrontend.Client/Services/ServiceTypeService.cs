using BeautyCenterFrontend.Models;

namespace BeautyCenterFrontend.Services
{
    public class ServiceTypeService
    {
        private readonly ApiService _apiService;

        public ServiceTypeService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<ServiceTypeModel>> GetAllServiceTypesAsync()
        {
            var result = await _apiService.GetAsync<List<ServiceTypeModel>>("api/servicetypes");
            return result ?? new List<ServiceTypeModel>();
        }

        public async Task<List<ServiceTypeModel>> GetActiveServiceTypesAsync()
        {
            var result = await _apiService.GetAsync<List<ServiceTypeModel>>("api/servicetypes/active");
            return result ?? new List<ServiceTypeModel>();
        }

        public async Task<ServiceTypeModel?> GetServiceTypeByIdAsync(int id)
        {
            return await _apiService.GetAsync<ServiceTypeModel>($"api/servicetypes/{id}");
        }

        public async Task<List<ServiceTypeModel>> GetServiceTypesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            var result = await _apiService.GetAsync<List<ServiceTypeModel>>($"api/servicetypes/price-range?minPrice={minPrice}&maxPrice={maxPrice}");
            return result ?? new List<ServiceTypeModel>();
        }

        //public async Task<ServiceTypeModel?> CreateServiceTypeAsync(CreateServiceTypeModel serviceType)
        //{
        //    return await _apiService.PostAsync<ServiceTypeModel>("api/servicetypes", serviceType);
        //}

        public async Task<bool> CreateServiceTypeAsync(CreateServiceTypeModel serviceType)
        {
            var result = await _apiService.PostAsync<ServiceTypeModel>("api/servicetypes", serviceType);
            return result != null;
        }

        public async Task<bool> UpdateServiceTypeAsync(int id, UpdateServiceTypeModel
            serviceType)
        {
            return await _apiService.PutAsync($"api/servicetypes/{id}", serviceType);
        }

        public async Task<bool> UpdateServiceTypeStatusAsync(int id, bool isActive)
        {
            return await _apiService.PutAsync($"api/servicetypes/{id}/status", new { IsActive = isActive });
        }

        public async Task<bool> DeleteServiceTypeAsync(int id)
        {
            return await _apiService.DeleteAsync($"api/servicetypes/{id}");
        }
    }
}