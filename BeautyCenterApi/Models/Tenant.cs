using System.ComponentModel.DataAnnotations;

namespace BeautyCenterApi.Models
{
    public class Tenant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty; // Güzellik merkezi adı

        [Required]
        [StringLength(50)]
        public string SubDomain { get; set; } = string.Empty; // Alt domain veya benzersiz kod

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(15)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? Website { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? Country { get; set; } = "Türkiye";

        [StringLength(20)]
        public string? PostalCode { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Subscription bilgileri
        [StringLength(50)]
        public string SubscriptionPlan { get; set; } = "Basic"; // Basic, Premium, Enterprise

        public DateTime? SubscriptionStartDate { get; set; }

        public DateTime? SubscriptionEndDate { get; set; }

        public int MaxUsers { get; set; } = 5; // Plan kapsamında maksimum kullanıcı sayısı

        public int MaxCustomers { get; set; } = 100; // Plan kapsamında maksimum müşteri sayısı

        // Navigation properties
        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
        public virtual ICollection<ServiceType> ServiceTypes { get; set; } = new List<ServiceType>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}