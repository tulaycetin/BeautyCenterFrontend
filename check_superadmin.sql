-- SuperAdmin kullanıcısını kontrol et
SELECT
    Id,
    Username,
    Email,
    Role,
    TenantId,
    IsActive,
    CreatedAt
FROM Users
WHERE Role = 'SuperAdmin' OR Username = 'superadmin';

-- Tüm kullanıcıları göster
SELECT
    Id,
    Username,
    Email,
    Role,
    TenantId,
    IsActive
FROM Users;

-- Tenantları göster
SELECT Id, Name, SubDomain, IsActive FROM Tenants;