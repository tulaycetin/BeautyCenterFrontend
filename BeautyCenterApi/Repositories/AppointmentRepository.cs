using Microsoft.EntityFrameworkCore;
using BeautyCenterApi.Data;
using BeautyCenterApi.Interfaces;
using BeautyCenterApi.Models;
using BeautyCenterApi.Services;

namespace BeautyCenterApi.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(BeautyCenterDbContext context, ITenantService tenantService)
            : base(context, tenantService)
        {
        }

        public new async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query
                .Include(a => a.Customer)
                .Include(a => a.ServiceType)
                .Include(a => a.User)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByCustomerIdAsync(int customerId)
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query
                .Include(a => a.ServiceType)
                .Include(a => a.User)
                .Where(a => a.CustomerId == customerId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query
                .Include(a => a.Customer)
                .Include(a => a.ServiceType)
                .Include(a => a.User)
                .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByStatusAsync(string status)
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query
                .Include(a => a.Customer)
                .Include(a => a.ServiceType)
                .Include(a => a.User)
                .Where(a => a.Status == status)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByServiceTypeIdAsync(int serviceTypeId)
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query
                .Include(a => a.Customer)
                .Include(a => a.ServiceType)
                .Include(a => a.User)
                .Where(a => a.ServiceTypeId == serviceTypeId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetTodaysAppointmentsAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var query = ApplyTenantFilter(_dbSet);

            return await query
                .Include(a => a.Customer)
                .Include(a => a.ServiceType)
                .Include(a => a.User)
                .Where(a => a.AppointmentDate >= today && a.AppointmentDate < tomorrow)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<Appointment?> GetWithDetailsAsync(int appointmentId)
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query
                .Include(a => a.Customer)
                .Include(a => a.ServiceType)
                .Include(a => a.User)
                .Include(a => a.Payments)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int customerId)
        {
            var now = DateTime.Now;
            var query = ApplyTenantFilter(_dbSet);
            return await query
                .Include(a => a.ServiceType)
                .Include(a => a.User)
                .Where(a => a.CustomerId == customerId && a.AppointmentDate > now && a.Status == "Scheduled")
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate)
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query
                .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate && a.Status == "Completed")
                .SumAsync(a => a.FinalPrice);
        }
    }
}