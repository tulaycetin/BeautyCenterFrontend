using Microsoft.EntityFrameworkCore;
using BeautyCenterApi.Data;
using BeautyCenterApi.Interfaces;
using BeautyCenterApi.Models;

namespace BeautyCenterApi.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(BeautyCenterDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetByPhoneAsync(string phone)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Phone == phone);
        }

        public async Task<IEnumerable<Customer>> SearchAsync(string searchTerm)
        {
            var term = searchTerm.ToLower();
            return await _dbSet
                .Where(c => c.FirstName.ToLower().Contains(term) ||
                           c.LastName.ToLower().Contains(term) ||
                           c.Phone.Contains(term) ||
                           (c.Email != null && c.Email.ToLower().Contains(term)))
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
        {
            return await _dbSet.Where(c => c.IsActive).ToListAsync();
        }

        public async Task<Customer?> GetWithAppointmentsAsync(int customerId)
        {
            return await _dbSet
                .Include(c => c.Appointments)
                .ThenInclude(a => a.ServiceType)
                .FirstOrDefaultAsync(c => c.Id == customerId);
        }

        public async Task<Customer?> GetWithPaymentsAsync(int customerId)
        {
            return await _dbSet
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(c => c.Id == customerId);
        }
    }
}