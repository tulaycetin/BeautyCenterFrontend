using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautyCenterApi.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int ServiceTypeId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Cancelled, NoShow

        [Required]
        public decimal TotalPrice { get; set; }

        public decimal? DiscountAmount { get; set; }

        public decimal FinalPrice { get; set; }

        public int? SessionsTotal { get; set; }

        public int? SessionsCompleted { get; set; }

        public int? SessionsRemaining { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } = null!;

        [ForeignKey("ServiceTypeId")]
        public virtual ServiceType ServiceType { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}