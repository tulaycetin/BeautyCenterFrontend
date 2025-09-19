namespace BeautyCenterApi.Services
{
    public interface ITenantService
    {
        int? GetCurrentTenantId();
        void SetCurrentTenantId(int? tenantId);
        bool IsSuperAdmin();
        bool HasTenantAccess(int tenantId);
    }
}