using Microsoft.EntityFrameworkCore;
using BeautyCenterApi.Data;
using BeautyCenterApi.Interfaces;
using BeautyCenterApi.Models;
using BeautyCenterApi.Services;

namespace BeautyCenterApi.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(BeautyCenterDbContext context, ITenantService tenantService)
            : base(context, tenantService)
        {
        }

        public async Task<Customer?> GetByPhoneAsync(string phone)
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query.FirstOrDefaultAsync(c => c.Phone == phone);
        }

        public async Task<IEnumerable<Customer>> SearchAsync(string searchTerm)
        {
            var term = searchTerm.ToLower();
            var query = ApplyTenantFilter(_dbSet);
            return await query
                .Where(c => c.FirstName.ToLower().Contains(term) ||
                           c.LastName.ToLower().Contains(term) ||
                           c.Phone.Contains(term) ||
                           (c.Email != null && c.Email.ToLower().Contains(term)))
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query.Where(c => c.IsActive).ToListAsync();
        }

        public async Task<Customer?> GetWithAppointmentsAsync(int customerId)
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query
                .Include(c => c.Appointments)
                .ThenInclude(a => a.ServiceType)
                .FirstOrDefaultAsync(c => c.Id == customerId);
        }

        public async Task<Customer?> GetWithPaymentsAsync(int customerId)
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(c => c.Id == customerId);
        }
    }
}