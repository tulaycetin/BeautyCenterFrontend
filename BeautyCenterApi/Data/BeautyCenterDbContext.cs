using Microsoft.EntityFrameworkCore;
using BeautyCenterApi.Models;

namespace BeautyCenterApi.Data
{
    public class BeautyCenterDbContext : DbContext
    {
        public BeautyCenterDbContext(DbContextOptions<BeautyCenterDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ServiceType> ServiceTypes { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal properties
            modelBuilder.Entity<ServiceType>()
                .Property(s => s.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Appointment>()
                .Property(a => a.TotalPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Appointment>()
                .Property(a => a.DiscountAmount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Appointment>()
                .Property(a => a.FinalPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(10, 2);

            // Configure relationships
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.ServiceType)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Customer)
                .WithMany(c => c.Payments)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Appointment)
                .WithMany(a => a.Payments)
                .HasForeignKey(p => p.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // Seed data
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@beautycenter.com",
                    PasswordHash = "$2a$11$dm4AgGac/is.r.qEOtFP5.xAOUJCqrfVWJXzbe9O53OfpJvb0dEmK", // admin123
                    FirstName = "Admin",
                    LastName = "User",
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            modelBuilder.Entity<ServiceType>().HasData(
                new ServiceType { Id = 1, Name = "Cilt Bakımı", Description = "Genel cilt bakım hizmeti", Price = 150, DurationMinutes = 60, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ServiceType { Id = 2, Name = "Makyaj", Description = "Özel gün makyajı", Price = 200, DurationMinutes = 90, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ServiceType { Id = 3, Name = "Kaş Dizaynı", Description = "Kaş şekillendirme ve boyama", Price = 100, DurationMinutes = 45, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ServiceType { Id = 4, Name = "Masaj", Description = "Rahatlatıcı masaj hizmeti", Price = 250, DurationMinutes = 90, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ServiceType { Id = 5, Name = "Epilasyon", Description = "Lazer epilasyon hizmeti", Price = 300, DurationMinutes = 60, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}