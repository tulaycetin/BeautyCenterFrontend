-- Önce yeni tenant'ları oluştur
INSERT INTO Tenants (Name, SubDomain, Description, Address, Phone, Email, City, Country, IsActive, CreatedAt, SubscriptionPlan, SubscriptionStartDate, SubscriptionEndDate, MaxUsers, MaxCustomers)
VALUES
    ('Gülhan Güzellik Merkezi', 'gulhan', 'Gülhan Güzellik Merkezi', 'İstanbul, Kadıköy', '555-0002', 'gulhan@beautycenter.com', 'İstanbul', 'Türkiye', 1, GETDATE(), 'Premium', '2024-01-01', '2024-12-31', 10, 500),
    ('Arda Güzellik Merkezi', 'arda', 'Arda Güzellik Merkezi', 'Ankara, Çankaya', '555-0003', 'arda@beautycenter.com', 'Ankara', 'Türkiye', 1, GETDATE(), 'Premium', '2024-01-01', '2024-12-31', 10, 500),
    ('Elif Güzellik Merkezi', 'elif', 'Elif Güzellik Merkezi', 'İzmir, Alsancak', '555-0004', 'elif@beautycenter.com', 'İzmir', 'Türkiye', 1, GETDATE(), 'Premium', '2024-01-01', '2024-12-31', 10, 500);

-- Tenant ID'lerini al
DECLARE @GulhanTenantId INT = (SELECT Id FROM Tenants WHERE SubDomain = 'gulhan');
DECLARE @ArdaTenantId INT = (SELECT Id FROM Tenants WHERE SubDomain = 'arda');
DECLARE @ElifTenantId INT = (SELECT Id FROM Tenants WHERE SubDomain = 'elif');

-- Kullanıcıların rollerini ve tenant atamalarını güncelle
UPDATE Users
SET Role = 'TenantAdmin',
    TenantId = @GulhanTenantId,
    UpdatedAt = GETDATE()
WHERE Username = 'gulhan';

UPDATE Users
SET Role = 'TenantAdmin',
    TenantId = @ArdaTenantId,
    UpdatedAt = GETDATE()
WHERE Username = 'arda';

UPDATE Users
SET Role = 'TenantAdmin',
    TenantId = @ElifTenantId,
    UpdatedAt = GETDATE()
WHERE Username = 'elif';

-- SuperAdmin sadece 'superadmin' kullanıcısı olsun
UPDATE Users
SET Role = 'SuperAdmin',
    TenantId = NULL
WHERE Username = 'superadmin';

-- Kontrol için select
SELECT
    u.Id,
    u.Username,
    u.Email,
    u.FirstName,
    u.LastName,
    u.Role,
    u.TenantId,
    t.Name as TenantName,
    t.SubDomain as TenantSubDomain
FROM Users u
LEFT JOIN Tenants t ON u.TenantId = t.Id
ORDER BY u.Id;