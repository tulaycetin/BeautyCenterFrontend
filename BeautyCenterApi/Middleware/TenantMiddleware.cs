using BeautyCenterApi.Services;
using System.Security.Claims;

namespace BeautyCenterApi.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
        {
            // JWT token'dan tenant bilgisini al
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var tenantIdClaim = context.User.FindFirst("TenantId");
                if (tenantIdClaim != null && int.TryParse(tenantIdClaim.Value, out int tenantId))
                {
                    tenantService.SetCurrentTenantId(tenantId);
                }
                else
                {
                    // SuperAdmin için tenant null olabilir
                    var roleClaim = context.User.FindFirst(ClaimTypes.Role);
                    if (roleClaim?.Value != "SuperAdmin")
                    {
                        // Normal kullanıcıların tenant bilgisi olmak zorunda
                        context.Response.StatusCode = 403;
                        await context.Response.WriteAsync("Tenant bilgisi bulunamadı.");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}