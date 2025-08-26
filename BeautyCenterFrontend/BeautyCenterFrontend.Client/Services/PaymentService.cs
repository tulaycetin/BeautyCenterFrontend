using BeautyCenterFrontend.Models;

namespace BeautyCenterFrontend.Services
{
    public class PaymentService
    {
        private readonly ApiService _apiService;

        public PaymentService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<PaymentModel>> GetAllPaymentsAsync()
        {
            var result = await _apiService.GetAsync<List<PaymentModel>>("api/payments");
            return result ?? new List<PaymentModel>();
        }

        public async Task<PaymentModel?> GetPaymentByIdAsync(int id)
        {
            return await _apiService.GetAsync<PaymentModel>($"api/payments/{id}");
        }

        public async Task<List<PaymentModel>> GetPaymentsByCustomerAsync(int customerId)
        {
            var result = await _apiService.GetAsync<List<PaymentModel>>($"api/payments/customer/{customerId}");
            return result ?? new List<PaymentModel>();
        }

        public async Task<List<PaymentModel>> GetPaymentsByAppointmentAsync(int appointmentId)
        {
            var result = await _apiService.GetAsync<List<PaymentModel>>($"api/payments/appointment/{appointmentId}");
            return result ?? new List<PaymentModel>();
        }

        public async Task<List<PaymentModel>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var result = await _apiService.GetAsync<List<PaymentModel>>($"api/payments/date-range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            return result ?? new List<PaymentModel>();
        }

        public async Task<List<PaymentModel>> GetPaymentsByMethodAsync(string paymentMethod)
        {
            var result = await _apiService.GetAsync<List<PaymentModel>>($"api/payments/method/{paymentMethod}");
            return result ?? new List<PaymentModel>();
        }

        public async Task<PaymentModel?> CreatePaymentAsync(CreatePaymentModel payment)
        {
            return await _apiService.PostAsync<PaymentModel>("api/payments", payment);
        }

        public async Task<bool> UpdatePaymentAsync(int id, CreatePaymentModel payment)
        {
            return await _apiService.PutAsync($"api/payments/{id}", payment);
        }

        public async Task<bool> DeletePaymentAsync(int id)
        {
            return await _apiService.DeleteAsync($"api/payments/{id}");
        }
    }
}