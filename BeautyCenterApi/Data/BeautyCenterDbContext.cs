using Microsoft.EntityFrameworkCore;
using BeautyCenterApi.Models;

namespace BeautyCenterApi.Data
{
    public class BeautyCenterDbContext : DbContext
    {
        public BeautyCenterDbContext(DbContextOptions<BeautyCenterDbContext> options) : base(options)
        {
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ServiceType> ServiceTypes { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentInstallment> PaymentInstallments { get; set; }

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
                .Property(p => p.TotalAmount)
                .HasPrecision(10, 2);
                
            modelBuilder.Entity<Payment>()
                .Property(p => p.PaidAmount)
                .HasPrecision(10, 2);
                
            modelBuilder.Entity<Payment>()
                .Property(p => p.RemainingAmount)
                .HasPrecision(10, 2);
                
            modelBuilder.Entity<PaymentInstallment>()
                .Property(pi => pi.Amount)
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

            // Tenant relationships
            modelBuilder.Entity<User>()
                .HasOne(u => u.Tenant)
                .WithMany(t => t.Users)
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Tenant)
                .WithMany(t => t.Customers)
                .HasForeignKey(c => c.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServiceType>()
                .HasOne(s => s.Tenant)
                .WithMany(t => t.ServiceTypes)
                .HasForeignKey(s => s.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Tenant)
                .WithMany(t => t.Appointments)
                .HasForeignKey(a => a.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Tenant)
                .WithMany(t => t.Payments)
                .HasForeignKey(p => p.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data
            // İlk tenant oluştur
            modelBuilder.Entity<Tenant>().HasData(
                new Tenant
                {
                    Id = 1,
                    Name = "Demo Güzellik Merkezi",
                    SubDomain = "demo",
                    Description = "Demo güzellik merkezi",
                    Email = "demo@beautycenter.com",
                    Phone = "555-0001",
                    IsActive = true,
                    SubscriptionPlan = "Premium",
                    SubscriptionStartDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    SubscriptionEndDate = new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    MaxUsers = 10,
                    MaxCustomers = 500,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // SuperAdmin ve Demo tenant admin'i oluştur
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    TenantId = null, // SuperAdmin
                    Username = "superadmin",
                    Email = "superadmin@beautycenter.com",
                    PasswordHash = "$2a$11$dm4AgGac/is.r.qEOtFP5.xAOUJCqrfVWJXzbe9O53OfpJvb0dEmK", // admin123
                    FirstName = "Super",
                    LastName = "Admin",
                    Role = "SuperAdmin",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 2,
                    TenantId = 1, // Demo tenant admin
                    Username = "admin",
                    Email = "admin@demo.beautycenter.com",
                    PasswordHash = "$2a$11$dm4AgGac/is.r.qEOtFP5.xAOUJCqrfVWJXzbe9O53OfpJvb0dEmK", // admin123
                    FirstName = "Admin",
                    LastName = "Demo",
                    Role = "TenantAdmin",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Demo tenant için örnek hizmetler
            modelBuilder.Entity<ServiceType>().HasData(
                new ServiceType { Id = 1, TenantId = 1, Name = "Cilt Bakımı", Description = "Genel cilt bakım hizmeti", Price = 150, DurationMinutes = 60, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ServiceType { Id = 2, TenantId = 1, Name = "Makyaj", Description = "Özel gün makyajı", Price = 200, DurationMinutes = 90, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ServiceType { Id = 3, TenantId = 1, Name = "Kaş Dizaynı", Description = "Kaş şekillendirme ve boyama", Price = 100, DurationMinutes = 45, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ServiceType { Id = 4, TenantId = 1, Name = "Masaj", Description = "Rahatlatıcı masaj hizmeti", Price = 250, DurationMinutes = 90, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ServiceType { Id = 5, TenantId = 1, Name = "Epilasyon", Description = "Lazer epilasyon hizmeti", Price = 300, DurationMinutes = 60, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}