using System.ComponentModel.DataAnnotations;

namespace BeautyCenterApi.DTOs
{
    public class AppointmentDto
    {
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
        public string Status { get; set; } = "Scheduled";

        [Required]
        public decimal TotalPrice { get; set; }

        public decimal? DiscountAmount { get; set; }

        public decimal FinalPrice { get; set; }

        public int? SessionsTotal { get; set; }

        public int? SessionsCompleted { get; set; }

        public int? SessionsRemaining { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        // Navigation properties for display
        public string CustomerName { get; set; } = string.Empty;
        public string ServiceTypeName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int Duration { get; set; }
        public decimal ServicePrice { get; set; }
        
        // Status display text for frontend
        public string StatusText => Status switch
        {
            "Scheduled" => "Planlanmış",
            "Completed" => "Tamamlanmış",
            "Cancelled" => "İptal Edilmiş",
            "NoShow" => "Gelmedi",
            _ => Status
        };
    }

    public class CreateAppointmentDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int ServiceTypeId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        public decimal? DiscountAmount { get; set; }

        public int? SessionsTotal { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class UpdateAppointmentDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int ServiceTypeId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Scheduled";

        public decimal? DiscountAmount { get; set; }

        public int? SessionsCompleted { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }
}