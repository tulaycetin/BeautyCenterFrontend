using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautyCenterApi.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TenantId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public decimal TotalAmount { get; set; } // Toplam tutar

        [Required]
        public decimal PaidAmount { get; set; } // Ödenen tutar

        [Required]
        public decimal RemainingAmount { get; set; } // Kalan tutar

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Nakit"; // Nakit, Kart, Havale

        [Required]
        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Partial"; // Completed, Partial, Pending

        [Required]
        public DateTime PaymentDate { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? ReferenceNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; } = null!;

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } = null!;

        [ForeignKey("AppointmentId")]
        public virtual Appointment Appointment { get; set; } = null!;
        
        public virtual ICollection<PaymentInstallment> Installments { get; set; } = new List<PaymentInstallment>();
    }
    
    public class PaymentInstallment
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int PaymentId { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public DateTime DueDate { get; set; }
        
        public DateTime? PaidDate { get; set; }
        
        [Required]
        public bool IsPaid { get; set; } = false;
        
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Nakit";
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; } = null!;
    }
}