using System.ComponentModel.DataAnnotations;

namespace BeautyCenterFrontend.Models
{
    public class PaymentModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int AppointmentId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string PaymentMethod { get; set; } = "Nakit";
        public string PaymentStatus { get; set; } = "Partial";
        public DateTime PaymentDate { get; set; }
        public string? Description { get; set; }
        public string? ReferenceNumber { get; set; }
        
        // Navigation properties for display
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? ServiceTypeName { get; set; }
        public decimal? ServicePrice { get; set; }
        public DateTime? AppointmentDate { get; set; }
        
        // Installments
        public List<PaymentInstallmentModel> Installments { get; set; } = new();
        
        public string TotalAmountText => $"₺{TotalAmount:N2}";
        public string PaidAmountText => $"₺{PaidAmount:N2}";
        public string RemainingAmountText => $"₺{RemainingAmount:N2}";
        
        public string PaymentMethodText => PaymentMethod switch
        {
            "Nakit" => "Nakit",
            "Kart" => "Kredi Kartı",
            "Havale" => "Havale/EFT",
            _ => PaymentMethod
        };
        
        public string PaymentStatusText => PaymentStatus switch
        {
            "Completed" => "Tamamlandı",
            "Partial" => "Kısmi Ödeme",
            "Pending" => "Beklemede",
            _ => PaymentStatus
        };
        
        public string StatusColor => PaymentStatus switch
        {
            "Completed" => "success",
            "Partial" => "warning",
            "Pending" => "danger",
            _ => "secondary"
        };
    }

    public class CreatePaymentModel
    {
        [Required(ErrorMessage = "Müşteri seçimi zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir müşteri seçiniz.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Randevu seçimi zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir randevu seçiniz.")]
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Toplam tutar zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Tutar 0'dan büyük olmalıdır.")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "İlk ödeme tutarı zorunludur.")]
        [Range(0, double.MaxValue, ErrorMessage = "İlk ödeme tutarı 0 veya daha fazla olmalıdır.")]
        public decimal InitialPayment { get; set; }

        [Required(ErrorMessage = "Ödeme yöntemi zorunludur.")]
        public string PaymentMethod { get; set; } = "Nakit";

        [Required(ErrorMessage = "Ödeme tarihi zorunludur.")]
        public DateTime PaymentDate { get; set; } = DateTime.Today;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "Referans numarası en fazla 100 karakter olabilir.")]
        public string? ReferenceNumber { get; set; }
        
        // Taksit bilgileri
        public int? InstallmentCount { get; set; }
        public List<CreateInstallmentModel>? Installments { get; set; }
    }
    
    public class UpdatePaymentModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Ödeme tutarı zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Tutar 0'dan büyük olmalıdır.")]
        public decimal PaymentAmount { get; set; }
        
        [Required(ErrorMessage = "Ödeme yöntemi zorunludur.")]
        public string PaymentMethod { get; set; } = "Nakit";
        
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? Description { get; set; }
    }
    
    public class PaymentInstallmentModel
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public bool IsPaid { get; set; }
        public string PaymentMethod { get; set; } = "Nakit";
        public string? Notes { get; set; }
        
        public string AmountText => $"₺{Amount:N2}";
        public string DueDateText => DueDate.ToString("dd.MM.yyyy");
        public string PaidDateText => PaidDate?.ToString("dd.MM.yyyy") ?? "-";
        public string StatusText => IsPaid ? "Ödendi" : "Bekliyor";
        public string StatusColor => IsPaid ? "success" : (DueDate < DateTime.Today ? "danger" : "warning");
        public bool IsOverdue => !IsPaid && DueDate < DateTime.Today;
        public int OverdueDays => IsOverdue ? (DateTime.Today - DueDate).Days : 0;
    }
    
    public class CreateInstallmentModel
    {
        [Required(ErrorMessage = "Taksit tutarı zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Tutar 0'dan büyük olmalıdır.")]
        public decimal Amount { get; set; }
        
        [Required(ErrorMessage = "Vade tarihi zorunludur.")]
        public DateTime DueDate { get; set; }
        
        [StringLength(500, ErrorMessage = "Not en fazla 500 karakter olabilir.")]
        public string? Notes { get; set; }
    }
    
    public class PayInstallmentModel
    {
        [Required]
        public int InstallmentId { get; set; }
        
        [Required(ErrorMessage = "Ödeme yöntemi zorunludur.")]
        public string PaymentMethod { get; set; } = "Nakit";
        
        [StringLength(500, ErrorMessage = "Not en fazla 500 karakter olabilir.")]
        public string? Notes { get; set; }
    }
    
    public class PaymentSummaryModel
    {
        public int TotalPayments { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalRemaining { get; set; }
        public int CompletedPayments { get; set; }
        public int PartialPayments { get; set; }
        public int PendingPayments { get; set; }
        public List<PaymentModel> RecentPayments { get; set; } = new();
        public List<PaymentInstallmentModel> UpcomingInstallments { get; set; } = new();
        
        public string TotalRevenueText => $"₺{TotalRevenue:N2}";
        public string TotalPaidText => $"₺{TotalPaid:N2}";
        public string TotalRemainingText => $"₺{TotalRemaining:N2}";
    }
    
    public class PendingAppointmentModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerPhone { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal FinalPrice { get; set; }
        public DateTime AppointmentDate { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        
        public string PriceText => $"₺{Price:N2}";
        public string FinalPriceText => $"₺{FinalPrice:N2}";
        public string PaidAmountText => $"₺{PaidAmount:N2}";
        public string RemainingAmountText => $"₺{RemainingAmount:N2}";
        public string AppointmentDateText => AppointmentDate.ToString("dd.MM.yyyy HH:mm");
    }
    
    public class CustomerWithAppointmentModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
    }
}