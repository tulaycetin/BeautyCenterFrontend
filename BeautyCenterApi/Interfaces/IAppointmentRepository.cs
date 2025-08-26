using BeautyCenterApi.Models;

namespace BeautyCenterApi.Interfaces
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Appointment>> GetByStatusAsync(string status);
        Task<IEnumerable<Appointment>> GetByServiceTypeIdAsync(int serviceTypeId);
        Task<IEnumerable<Appointment>> GetTodaysAppointmentsAsync();
        Task<Appointment?> GetWithDetailsAsync(int appointmentId);
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int customerId);
        Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate);
    }
}