using System.ComponentModel.DataAnnotations;

namespace BeautyCenterApi.DTOs
{
    public class PaymentDto
    {
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public int? AppointmentId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string PaymentType { get; set; } = string.Empty;

        [Required]
        public DateTime PaymentDate { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? ReferenceNumber { get; set; }

        // Navigation properties for display
        public string CustomerName { get; set; } = string.Empty;
        public string? ServiceTypeName { get; set; }
    }

    public class CreatePaymentDto
    {
        [Required]
        public int CustomerId { get; set; }

        public int? AppointmentId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string PaymentType { get; set; } = string.Empty;

        [Required]
        public DateTime PaymentDate { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? ReferenceNumber { get; set; }
    }
}