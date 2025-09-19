using System.Security.Claims;

namespace BeautyCenterApi.Services
{
    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private int? _currentTenantId;

        public TenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? GetCurrentTenantId()
        {
            if (_currentTenantId.HasValue)
                return _currentTenantId;

            var context = _httpContextAccessor.HttpContext;
            if (context?.User?.Identity?.IsAuthenticated == true)
            {
                var tenantIdClaim = context.User.FindFirst("TenantId");
                if (tenantIdClaim != null && int.TryParse(tenantIdClaim.Value, out int tenantId))
                {
                    _currentTenantId = tenantId;
                    return tenantId;
                }
            }

            return null;
        }

        public void SetCurrentTenantId(int? tenantId)
        {
            _currentTenantId = tenantId;
        }

        public bool IsSuperAdmin()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.User?.Identity?.IsAuthenticated == true)
            {
                var roleClaim = context.User.FindFirst(ClaimTypes.Role);
                return roleClaim?.Value == "SuperAdmin";
            }
            return false;
        }

        public bool HasTenantAccess(int tenantId)
        {
            // SuperAdmin tüm tenantlara erişebilir
            if (IsSuperAdmin())
                return true;

            // Normal kullanıcılar sadece kendi tenantlarına erişebilir
            var currentTenantId = GetCurrentTenantId();
            return currentTenantId.HasValue && currentTenantId.Value == tenantId;
        }
    }
}