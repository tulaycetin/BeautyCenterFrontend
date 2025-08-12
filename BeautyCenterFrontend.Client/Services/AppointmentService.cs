using BeautyCenterFrontend.Models;

namespace BeautyCenterFrontend.Services
{
    public class AppointmentService
    {
        private readonly ApiService _apiService;

        public AppointmentService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<AppointmentModel>> GetAllAppointmentsAsync()
        {
            var result = await _apiService.GetAsync<List<AppointmentModel>>("api/appointments");
            return result ?? new List<AppointmentModel>();
        }

        public async Task<AppointmentModel?> GetAppointmentByIdAsync(int id)
        {
            return await _apiService.GetAsync<AppointmentModel>($"api/appointments/{id}");
        }

        public async Task<List<AppointmentModel>> GetAppointmentsByCustomerAsync(int customerId)
        {
            var result = await _apiService.GetAsync<List<AppointmentModel>>($"api/appointments/customer/{customerId}");
            return result ?? new List<AppointmentModel>();
        }

        public async Task<List<AppointmentModel>> GetUpcomingAppointmentsAsync(int customerId)
        {
            var result = await _apiService.GetAsync<List<AppointmentModel>>($"api/appointments/customer/{customerId}/upcoming");
            return result ?? new List<AppointmentModel>();
        }

        public async Task<List<AppointmentModel>> GetTodaysAppointmentsAsync()
        {
            var result = await _apiService.GetAsync<List<AppointmentModel>>("api/appointments/today");
            return result ?? new List<AppointmentModel>();
        }

        public async Task<List<AppointmentModel>> GetAppointmentsByStatusAsync(string status)
        {
            var result = await _apiService.GetAsync<List<AppointmentModel>>($"api/appointments/status/{status}");
            return result ?? new List<AppointmentModel>();
        }

        public async Task<List<AppointmentModel>> GetAppointmentsByServiceAsync(int serviceTypeId)
        {
            var result = await _apiService.GetAsync<List<AppointmentModel>>($"api/appointments/service/{serviceTypeId}");
            return result ?? new List<AppointmentModel>();
        }

        public async Task<List<AppointmentModel>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var result = await _apiService.GetAsync<List<AppointmentModel>>($"api/appointments/date-range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            return result ?? new List<AppointmentModel>();
        }

        //public async Task<AppointmentModel?> CreateAppointmentAsync(CreateAppointmentModel appointment)
        //{
        //    return await _apiService.PostAsync<AppointmentModel>("api/appointments", appointment);
        //}

        public async Task<bool> CreateAppointmentAsync(CreateAppointmentModel appointment)
        {
            var result = await _apiService.PostAsync<AppointmentModel>("api/appointments", appointment);
            return result != null;
        }

        public async Task<bool> UpdateAppointmentAsync(int id, UpdateAppointmentModel appointment)
        {
            return await _apiService.PutAsync($"api/appointments/{id}", appointment);
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int id, string status)
        {
            return await _apiService.PutAsync($"api/appointments/{id}/status", new { Status = status });
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            return await _apiService.DeleteAsync($"api/appointments/{id}");
        }
    }
}