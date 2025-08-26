using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautyCenterApi.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public int? AppointmentId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty; // Cash, Card, Transfer

        [Required]
        [StringLength(50)]
        public string PaymentType { get; set; } = string.Empty; // Full, Partial, Advance

        [Required]
        public DateTime PaymentDate { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? ReferenceNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } = null!;

        [ForeignKey("AppointmentId")]
        public virtual Appointment? Appointment { get; set; }
    }
}