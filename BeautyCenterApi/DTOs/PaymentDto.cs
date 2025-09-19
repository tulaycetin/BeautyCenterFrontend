using System.ComponentModel.DataAnnotations;

namespace BeautyCenterApi.DTOs
{
    public class PaymentDto
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
        public List<PaymentInstallmentDto> Installments { get; set; } = new();
    }

    public class CreatePaymentDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public decimal InitialPayment { get; set; } // İlk ödeme

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Nakit";

        [Required]
        public DateTime PaymentDate { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? ReferenceNumber { get; set; }
        
        // Taksit bilgileri
        public int? InstallmentCount { get; set; } // Taksit sayısı
        public List<CreateInstallmentDto>? Installments { get; set; }
    }
    
    public class UpdatePaymentDto
    {
        public int Id { get; set; }
        
        [Required]
        public decimal PaymentAmount { get; set; } // Yeni ödeme tutarı
        
        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Nakit";
        
        [StringLength(500)]
        public string? Description { get; set; }
    }
    
    public class PaymentInstallmentDto
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public bool IsPaid { get; set; }
        public string PaymentMethod { get; set; } = "Nakit";
        public string? Notes { get; set; }
    }
    
    public class CreateInstallmentDto
    {
        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public DateTime DueDate { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
    }
    
    public class PayInstallmentDto
    {
        [Required]
        public int InstallmentId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Nakit";
        
        [StringLength(500)]
        public string? Notes { get; set; }
    }
    
    public class PaymentSummaryDto
    {
        public int TotalPayments { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalRemaining { get; set; }
        public int CompletedPayments { get; set; }
        public int PartialPayments { get; set; }
        public int PendingPayments { get; set; }
        public List<PaymentDto> RecentPayments { get; set; } = new();
        public List<PaymentInstallmentDto> UpcomingInstallments { get; set; } = new();
    }
}