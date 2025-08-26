using BeautyCenterApi.Models;

namespace BeautyCenterApi.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<IEnumerable<Payment>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Payment>> GetByAppointmentIdAsync(int appointmentId);
        Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Payment>> GetByPaymentMethodAsync(string paymentMethod);
        Task<decimal> GetTotalPaymentsAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetCustomerTotalPaymentsAsync(int customerId);
        Task<decimal> GetCustomerRemainingBalanceAsync(int customerId);
    }
}