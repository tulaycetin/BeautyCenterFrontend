using System.ComponentModel.DataAnnotations;

namespace BeautyCenterFrontend.Models
{
    public class CustomerModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public string FullName => $"{FirstName} {LastName}";
        
        // Backward compatibility
        public DateTime? DateOfBirth
        {
            get => BirthDate;
            set => BirthDate = value;
        }
    }

    public class CustomerCreateRequest
    {
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon alanı zorunludur.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [StringLength(15, ErrorMessage = "Telefon en fazla 15 karakter olabilir.")]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [StringLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir.")]
        public string? Email { get; set; }

        [StringLength(200, ErrorMessage = "Adres en fazla 200 karakter olabilir.")]
        public string? Address { get; set; }

        public DateTime? BirthDate { get; set; }

        [StringLength(10, ErrorMessage = "Cinsiyet en fazla 10 karakter olabilir.")]
        public string? Gender { get; set; }

        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir.")]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class CustomerWithDetailsModel : CustomerModel
    {
        public List<AppointmentModel> Appointments { get; set; } = new();
        public List<PaymentModel> Payments { get; set; } = new();
        public decimal TotalPaid { get; set; }
        public decimal RemainingBalance { get; set; }
    }
}