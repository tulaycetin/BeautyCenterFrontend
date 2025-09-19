using BeautyCenterApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BeautyCenterApi.Data
{
    public static class DataSeeder
    {
        public static async Task UpdateUsersAndTenants(BeautyCenterDbContext context)
        {
            // Yeni tenant'ları oluştur
            var gulhanTenant = await context.Tenants.FirstOrDefaultAsync(t => t.SubDomain == "gulhan");
            if (gulhanTenant == null)
            {
                gulhanTenant = new Tenant
                {
                    Name = "Gülhan Güzellik Merkezi",
                    SubDomain = "gulhan",
                    Description = "Gülhan Güzellik Merkezi",
                    Address = "İstanbul, Kadıköy",
                    Phone = "555-0002",
                    Email = "gulhan@beautycenter.com",
                    City = "İstanbul",
                    Country = "Türkiye",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    SubscriptionPlan = "Premium",
                    SubscriptionStartDate = new DateTime(2024, 1, 1),
                    SubscriptionEndDate = new DateTime(2024, 12, 31),
                    MaxUsers = 10,
                    MaxCustomers = 500
                };
                context.Tenants.Add(gulhanTenant);
            }

            var ardaTenant = await context.Tenants.FirstOrDefaultAsync(t => t.SubDomain == "arda");
            if (ardaTenant == null)
            {
                ardaTenant = new Tenant
                {
                    Name = "Arda Güzellik Merkezi",
                    SubDomain = "arda",
                    Description = "Arda Güzellik Merkezi",
                    Address = "Ankara, Çankaya",
                    Phone = "555-0003",
                    Email = "arda@beautycenter.com",
                    City = "Ankara",
                    Country = "Türkiye",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    SubscriptionPlan = "Premium",
                    SubscriptionStartDate = new DateTime(2024, 1, 1),
                    SubscriptionEndDate = new DateTime(2024, 12, 31),
                    MaxUsers = 10,
                    MaxCustomers = 500
                };
                context.Tenants.Add(ardaTenant);
            }

            var elifTenant = await context.Tenants.FirstOrDefaultAsync(t => t.SubDomain == "elif");
            if (elifTenant == null)
            {
                elifTenant = new Tenant
                {
                    Name = "Elif Güzellik Merkezi",
                    SubDomain = "elif",
                    Description = "Elif Güzellik Merkezi",
                    Address = "İzmir, Alsancak",
                    Phone = "555-0004",
                    Email = "elif@beautycenter.com",
                    City = "İzmir",
                    Country = "Türkiye",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    SubscriptionPlan = "Premium",
                    SubscriptionStartDate = new DateTime(2024, 1, 1),
                    SubscriptionEndDate = new DateTime(2024, 12, 31),
                    MaxUsers = 10,
                    MaxCustomers = 500
                };
                context.Tenants.Add(elifTenant);
            }

            // Demo Güzellik Merkezi tenant'ını oluştur
            var demoTenant = await context.Tenants.FirstOrDefaultAsync(t => t.SubDomain == "demo");
            if (demoTenant == null)
            {
                demoTenant = new Tenant
                {
                    Name = "Demo Güzellik Merkezi",
                    SubDomain = "demo",
                    Description = "Demo Güzellik Merkezi - Test Amaçlı",
                    Address = "İstanbul, Beşiktaş",
                    Phone = "555-0005",
                    Email = "demo@beautycenter.com",
                    City = "İstanbul",
                    Country = "Türkiye",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    SubscriptionPlan = "Premium",
                    SubscriptionStartDate = new DateTime(2024, 1, 1),
                    SubscriptionEndDate = new DateTime(2024, 12, 31),
                    MaxUsers = 10,
                    MaxCustomers = 500
                };
                context.Tenants.Add(demoTenant);
            }

            await context.SaveChangesAsync();

            // Tenant ID'lerini al
            gulhanTenant = await context.Tenants.FirstAsync(t => t.SubDomain == "gulhan");
            ardaTenant = await context.Tenants.FirstAsync(t => t.SubDomain == "arda");
            elifTenant = await context.Tenants.FirstAsync(t => t.SubDomain == "elif");
            demoTenant = await context.Tenants.FirstAsync(t => t.SubDomain == "demo");

            // Kullanıcıları güncelle
            var gulhanUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "gulhan");
            if (gulhanUser != null)
            {
                gulhanUser.Role = "TenantAdmin";
                gulhanUser.TenantId = gulhanTenant.Id;
                gulhanUser.UpdatedAt = DateTime.UtcNow;
            }

            var ardaUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "arda");
            if (ardaUser != null)
            {
                ardaUser.Role = "TenantAdmin";
                ardaUser.TenantId = ardaTenant.Id;
                ardaUser.UpdatedAt = DateTime.UtcNow;
            }

            var elifUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "elif");
            if (elifUser != null)
            {
                elifUser.Role = "TenantAdmin";
                elifUser.TenantId = elifTenant.Id;
                elifUser.UpdatedAt = DateTime.UtcNow;
            }

            // Demo kullanıcısını oluştur
            var demoUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "demo");
            if (demoUser == null)
            {
                demoUser = new User
                {
                    Username = "demo",
                    Email = "demo@beautycenter.com",
                    FirstName = "Demo",
                    LastName = "Kullanıcı",
                    PasswordHash = "$2a$11$dm4AgGac/is.r.qEOtFP5.xAOUJCqrfVWJXzbe9O53OfpJvb0dEmK", // admin123
                    Role = "TenantAdmin",
                    TenantId = demoTenant.Id,
                    Phone = "555-0005",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                context.Users.Add(demoUser);
            }
            else if (demoUser.TenantId == null)
            {
                demoUser.TenantId = demoTenant.Id;
                demoUser.Role = "TenantAdmin";
                demoUser.UpdatedAt = DateTime.UtcNow;
            }

            // SuperAdmin'in sadece 'superadmin' olduğundan emin ol
            var superadmin = await context.Users.FirstOrDefaultAsync(u => u.Username == "superadmin");
            if (superadmin != null)
            {
                superadmin.Role = "SuperAdmin";
                superadmin.TenantId = null;
            }

            await context.SaveChangesAsync();
        }
    }
}