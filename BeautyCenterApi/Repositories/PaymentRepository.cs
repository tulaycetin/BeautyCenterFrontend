using Microsoft.EntityFrameworkCore;
using BeautyCenterApi.Data;
using BeautyCenterApi.Interfaces;
using BeautyCenterApi.Models;
using BeautyCenterApi.Services;

namespace BeautyCenterApi.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(BeautyCenterDbContext context, ITenantService tenantService)
            : base(context, tenantService)
        {
        }

        public new async Task<IEnumerable<Payment>> GetAllAsync()
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query
                .Include(p => p.Customer)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a!.ServiceType)
                .Include(p => p.Installments)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public new async Task<Payment?> GetByIdAsync(int id)
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query
                .Include(p => p.Customer)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a!.ServiceType)
                .Include(p => p.Installments)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Payment>> GetByCustomerIdAsync(int customerId)
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query
                .Include(p => p.Appointment)
                    .ThenInclude(a => a!.ServiceType)
                .Include(p => p.Installments)
                .Where(p => p.CustomerId == customerId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByAppointmentIdAsync(int appointmentId)
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query
                .Include(p => p.Installments)
                .Where(p => p.AppointmentId == appointmentId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query
                .Include(p => p.Customer)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a!.ServiceType)
                .Include(p => p.Installments)
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByPaymentMethodAsync(string paymentMethod)
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query
                .Include(p => p.Customer)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a!.ServiceType)
                .Include(p => p.Installments)
                .Where(p => p.PaymentMethod == paymentMethod)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalPaymentsAsync(DateTime startDate, DateTime endDate)
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .SumAsync(p => p.PaidAmount);
        }

        public async Task<decimal> GetCustomerTotalPaymentsAsync(int customerId)
        {
            var query = ApplyTenantFilter(_dbSet);
            return await query
                .Where(p => p.CustomerId == customerId)
                .SumAsync(p => p.PaidAmount);
        }

        public async Task<decimal> GetCustomerRemainingBalanceAsync(int customerId)
        {
            var tenantId = _tenantService.GetCurrentTenantId();

            var totalAppointmentCost = await _context.Appointments
                .Where(a => a.CustomerId == customerId && a.TenantId == tenantId)
                .SumAsync(a => a.FinalPrice);

            var totalPayments = await GetCustomerTotalPaymentsAsync(customerId);

            return totalAppointmentCost - totalPayments;
        }
    }
}