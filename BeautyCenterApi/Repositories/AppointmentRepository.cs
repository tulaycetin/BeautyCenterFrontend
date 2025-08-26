using Microsoft.EntityFrameworkCore;
using BeautyCenterApi.Data;
using BeautyCenterApi.Interfaces;
using BeautyCenterApi.Models;

namespace BeautyCenterApi.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(BeautyCenterDbContext context) : base(context)
        {
        }

        public new async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _dbSet
                .Include(a => a.Customer)
                .Include(a => a.ServiceType)
                .Include(a => a.User)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByCustomerIdAsync(int customerId)
        {
            return await _dbSet
                .Include(a => a.ServiceType)
                .Include(a => a.User)
                .Where(a => a.CustomerId == customerId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(a => a.Customer)
                .Include(a => a.ServiceType)
                .Include(a => a.User)
                .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Include(a => a.Customer)
                .Include(a => a.ServiceType)
                .Include(a => a.User)
                .Where(a => a.Status == status)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByServiceTypeIdAsync(int serviceTypeId)
        {
            return await _dbSet
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
            
            return await _dbSet
                .Include(a => a.Customer)
                .Include(a => a.ServiceType)
                .Include(a => a.User)
                .Where(a => a.AppointmentDate >= today && a.AppointmentDate < tomorrow)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<Appointment?> GetWithDetailsAsync(int appointmentId)
        {
            return await _dbSet
                .Include(a => a.Customer)
                .Include(a => a.ServiceType)
                .Include(a => a.User)
                .Include(a => a.Payments)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int customerId)
        {
            var now = DateTime.Now;
            return await _dbSet
                .Include(a => a.ServiceType)
                .Include(a => a.User)
                .Where(a => a.CustomerId == customerId && a.AppointmentDate > now && a.Status == "Scheduled")
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate && a.Status == "Completed")
                .SumAsync(a => a.FinalPrice);
        }
    }
}