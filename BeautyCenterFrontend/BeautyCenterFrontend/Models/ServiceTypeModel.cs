using System.ComponentModel.DataAnnotations;

namespace BeautyCenterFrontend.Models
{
    public class ServiceTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public string? CategoryName { get; set; }
        public string DurationText => $"{DurationMinutes} dakika";
        public string PriceText => $"₺{Price:N2}";
    }

    public class CreateServiceTypeModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateServiceTypeModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class ServiceCreateRequest
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Hizmet adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Hizmet adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Süre zorunludur.")]
        [Range(15, 480, ErrorMessage = "Süre 15-480 dakika arasında olmalıdır.")]
        public int DurationMinutes { get; set; } = 60;

        [StringLength(50, ErrorMessage = "Kategori adı en fazla 50 karakter olabilir.")]
        public string? CategoryName { get; set; }

        [StringLength(500, ErrorMessage = "Görsel URL en fazla 500 karakter olabilir.")]
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;
    }
}