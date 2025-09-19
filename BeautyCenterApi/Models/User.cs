using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautyCenterApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        // Tenant ilişkisi - SuperAdmin için null olabilir
        public int? TenantId { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "Employee"; // SuperAdmin, TenantAdmin, Employee

        [StringLength(15)]
        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        [ForeignKey("TenantId")]
        public virtual Tenant? Tenant { get; set; }
    }

    // Kullanıcı rolleri için enum
    public enum UserRole
    {
        SuperAdmin,     // Sistem geneli yönetici
        TenantAdmin,    // Tenant yöneticisi
        Employee        // Çalışan
    }
}