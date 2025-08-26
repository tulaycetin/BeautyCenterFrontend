using System.ComponentModel.DataAnnotations;

namespace BeautyCenterApi.DTOs
{
    public class CustomerDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        public DateTime? BirthDate { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class CustomerWithDetailsDto : CustomerDto
    {
        public List<AppointmentDto> Appointments { get; set; } = new();
        public List<PaymentDto> Payments { get; set; } = new();
        public decimal TotalPaid { get; set; }
        public decimal RemainingBalance { get; set; }
    }
}