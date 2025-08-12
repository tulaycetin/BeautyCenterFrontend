using System.ComponentModel.DataAnnotations;

namespace BeautyCenterFrontend.Models
{
    public class PaymentModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int? AppointmentId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public string? Description { get; set; }
        public string? ReferenceNumber { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? ServiceTypeName { get; set; }
        public string? Notes { get; set; }
        
        public string AmountText => $"₺{Amount:N2}";
        
        public string PaymentMethodText => PaymentMethod switch
        {
            "Cash" => "Nakit",
            "Card" => "Kart",
            "Transfer" => "Transfer",
            _ => PaymentMethod
        };
        
        public string PaymentTypeText => PaymentType switch
        {
            "Full" => "Tam Ödeme",
            "Partial" => "Kısmi Ödeme",
            "Advance" => "Avans",
            _ => PaymentType
        };
    }

    public class CreatePaymentModel
    {
        public int CustomerId { get; set; }
        public int? AppointmentId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public string? Description { get; set; }
        public string? ReferenceNumber { get; set; }
    }

    public class PaymentCreateRequest
    {
        [Required(ErrorMessage = "Müşteri seçimi zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir müşteri seçiniz.")]
        public int CustomerId { get; set; }

        public int? AppointmentId { get; set; }

        [Required(ErrorMessage = "Tutar zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Tutar 0'dan büyük olmalıdır.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Ödeme yöntemi zorunludur.")]
        public string PaymentMethod { get; set; } = "Cash";

        [Required(ErrorMessage = "Ödeme türü zorunludur.")]
        public string PaymentType { get; set; } = "Full";

        [Required(ErrorMessage = "Ödeme tarihi zorunludur.")]
        public DateTime PaymentDate { get; set; }

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "Referans numarası en fazla 50 karakter olabilir.")]
        public string? ReferenceNumber { get; set; }
    }
}