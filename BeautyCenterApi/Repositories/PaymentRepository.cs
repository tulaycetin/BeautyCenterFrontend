using Microsoft.EntityFrameworkCore;
using BeautyCenterApi.Data;
using BeautyCenterApi.Interfaces;
using BeautyCenterApi.Models;

namespace BeautyCenterApi.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(BeautyCenterDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Payment>> GetByCustomerIdAsync(int customerId)
        {
            return await _dbSet
                .Include(p => p.Appointment)
                .ThenInclude(a => a.ServiceType)
                .Where(p => p.CustomerId == customerId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByAppointmentIdAsync(int appointmentId)
        {
            return await _dbSet
                .Where(p => p.AppointmentId == appointmentId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(p => p.Customer)
                .Include(p => p.Appointment)
                .ThenInclude(a => a.ServiceType)
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByPaymentMethodAsync(string paymentMethod)
        {
            return await _dbSet
                .Include(p => p.Customer)
                .Include(p => p.Appointment)
                .ThenInclude(a => a.ServiceType)
                .Where(p => p.PaymentMethod == paymentMethod)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalPaymentsAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .SumAsync(p => p.Amount);
        }

        public async Task<decimal> GetCustomerTotalPaymentsAsync(int customerId)
        {
            return await _dbSet
                .Where(p => p.CustomerId == customerId)
                .SumAsync(p => p.Amount);
        }

        public async Task<decimal> GetCustomerRemainingBalanceAsync(int customerId)
        {
            var totalAppointmentCost = await _context.Appointments
                .Where(a => a.CustomerId == customerId)
                .SumAsync(a => a.FinalPrice);

            var totalPayments = await GetCustomerTotalPaymentsAsync(customerId);

            return totalAppointmentCost - totalPayments;
        }
    }
}