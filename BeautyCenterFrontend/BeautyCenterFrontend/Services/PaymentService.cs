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

        public async Task<PaymentSummaryModel?> GetPaymentSummaryAsync()
        {
            return await _apiService.GetAsync<PaymentSummaryModel>("api/payments/summary");
        }

        public async Task<List<PendingAppointmentModel>> GetPendingAppointmentsAsync()
        {
            var result = await _apiService.GetAsync<List<PendingAppointmentModel>>("api/payments/pending-appointments");
            return result ?? new List<PendingAppointmentModel>();
        }

        public async Task<List<dynamic>> GetOverdueInstallmentsAsync()
        {
            var result = await _apiService.GetAsync<List<dynamic>>("api/payments/installments/overdue");
            return result ?? new List<dynamic>();
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

        public async Task<bool> CreatePaymentAsync(CreatePaymentModel payment)
        {
            var result = await _apiService.PostAsync<PaymentModel>("api/payments", payment);
            return result != null;
        }

        public async Task<bool> AddPaymentAsync(int id, UpdatePaymentModel payment)
        {
            return await _apiService.PutAsync($"api/payments/{id}/add-payment", payment);
        }

        public async Task<bool> PayInstallmentAsync(int installmentId, PayInstallmentModel payment)
        {
            var result = await _apiService.PostAsync<object>($"api/payments/installment/{installmentId}/pay", payment);
            return result != null;
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