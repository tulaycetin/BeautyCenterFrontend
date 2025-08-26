using BeautyCenterApi.Models;

namespace BeautyCenterApi.Interfaces
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<Customer?> GetByPhoneAsync(string phone);
        Task<IEnumerable<Customer>> SearchAsync(string searchTerm);
        Task<IEnumerable<Customer>> GetActiveCustomersAsync();
        Task<Customer?> GetWithAppointmentsAsync(int customerId);
        Task<Customer?> GetWithPaymentsAsync(int customerId);
    }
}