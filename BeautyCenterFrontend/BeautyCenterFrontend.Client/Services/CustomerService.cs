using BeautyCenterFrontend.Models;

namespace BeautyCenterFrontend.Services
{
    public class CustomerService
    {
        private readonly ApiService _apiService;

        public CustomerService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<CustomerModel>> GetAllCustomersAsync()
        {
            var result = await _apiService.GetAsync<List<CustomerModel>>("api/customers");
            return result ?? new List<CustomerModel>();
        }

        public async Task<List<CustomerModel>> GetActiveCustomersAsync()
        {
            var result = await _apiService.GetAsync<List<CustomerModel>>("api/customers/active");
            return result ?? new List<CustomerModel>();
        }

        public async Task<CustomerModel?> GetCustomerByIdAsync(int id)
        {
            return await _apiService.GetAsync<CustomerModel>($"api/customers/{id}");
        }

        public async Task<CustomerWithDetailsModel?> GetCustomerWithDetailsAsync(int id)
        {
            return await _apiService.GetAsync<CustomerWithDetailsModel>($"api/customers/{id}/details");
        }

        public async Task<List<CustomerModel>> SearchCustomersAsync(string searchTerm)
        {
            var result = await _apiService.GetAsync<List<CustomerModel>>($"api/customers/search?searchTerm={searchTerm}");
            return result ?? new List<CustomerModel>();
        }

        public async Task<CustomerModel?> CreateCustomerAsync(CustomerModel customer)
        {
            return await _apiService.PostAsync<CustomerModel>("api/customers", customer);
        }

        public async Task<bool> CreateCustomerAsync(CustomerCreateRequest customer)
        {
            // Convert CustomerCreateRequest to the format API expects
            var customerDto = new 
            {
                Id = 0,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.Phone,
                Email = customer.Email,
                Address = customer.Address,
                BirthDate = customer.BirthDate,
                Gender = customer.Gender,
                Notes = customer.Notes,
                IsActive = customer.IsActive
            };
            
            var result = await _apiService.PostAsync<CustomerModel>("api/customers", customerDto);
            return result != null;
        }

        public async Task<ApiResult<CustomerModel>> CreateCustomerWithErrorAsync(CustomerCreateRequest customer)
        {
            // Convert CustomerCreateRequest to the format API expects
            var customerDto = new 
            {
                Id = 0,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.Phone,
                Email = customer.Email,
                Address = customer.Address,
                BirthDate = customer.BirthDate,
                Gender = customer.Gender,
                Notes = customer.Notes,
                IsActive = customer.IsActive
            };
            
            return await _apiService.PostWithErrorAsync<CustomerModel>("api/customers", customerDto);
        }

        public async Task<bool> UpdateCustomerAsync(int id, CustomerModel customer)
        {
            return await _apiService.PutAsync($"api/customers/{id}", customer);
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            return await _apiService.DeleteAsync($"api/customers/{id}");
        }
    }
}