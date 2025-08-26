using System.ComponentModel.DataAnnotations;

namespace BeautyCenterFrontend.Models
{
    public class AppointmentModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ServiceTypeId { get; set; }
        public int UserId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } = "Scheduled";
        public decimal TotalPrice { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
        public int? SessionsTotal { get; set; }
        public int? SessionsCompleted { get; set; }
        public int? SessionsRemaining { get; set; }
        public string? Notes { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string ServiceTypeName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int Duration { get; set; }
        public decimal ServicePrice { get; set; }
        
        public string StatusText => Status switch
        {
            "Scheduled" => "Planlandı",
            "Completed" => "Tamamlandı",
            "Cancelled" => "İptal Edildi",
            "NoShow" => "Gelmedi",
            _ => Status
        };
        
        public string StatusColor => Status switch
        {
            "Scheduled" => "text-primary",
            "Completed" => "text-success",
            "Cancelled" => "text-danger",
            "NoShow" => "text-warning",
            _ => "text-secondary"
        };
    }

    public class CreateAppointmentModel
    {
        public int CustomerId { get; set; }
        public int ServiceTypeId { get; set; }
        public int UserId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public decimal? DiscountAmount { get; set; }
        public int? SessionsTotal { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateAppointmentModel
    {
        public int CustomerId { get; set; }
        public int ServiceTypeId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } = "Scheduled";
        public decimal? DiscountAmount { get; set; }
        public int? SessionsCompleted { get; set; }
        public string? Notes { get; set; }
    }

    public class AppointmentCreateRequest
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Müşteri seçimi zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir müşteri seçiniz.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Hizmet seçimi zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir hizmet seçiniz.")]
        public int ServiceTypeId { get; set; }

        [Required(ErrorMessage = "Randevu tarihi zorunludur.")]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Saat bilgisi zorunludur.")]
        public string AppointmentTimeString { get; set; } = "";

        [Range(15, 480, ErrorMessage = "Süre 15-480 dakika arasında olmalıdır.")]
        public int Duration { get; set; } = 60;

        [Range(0, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        public decimal Price { get; set; }

        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir.")]
        public string? Notes { get; set; }

        public decimal? DiscountAmount { get; set; }
        
        public string Status { get; set; } = "Scheduled";
        
        public int? SessionsTotal { get; set; }

        // Helper property for HTML date input
        public string AppointmentDateString 
        { 
            get => AppointmentDate.ToString("yyyy-MM-dd");
            set => AppointmentDate = DateTime.TryParse(value, out var date) ? date : DateTime.Today;
        }

        public DateTime GetFullAppointmentDateTime()
        {
            if (TimeSpan.TryParse(AppointmentTimeString, out var time))
            {
                return AppointmentDate.Date.Add(time);
            }
            return AppointmentDate;
        }
    }
}