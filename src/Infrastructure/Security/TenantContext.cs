using Microsoft.AspNetCore.Http;
using connectasys_api.Core.Application.Interfaces.Services;

namespace connectasys_api.Infrastructure.Security
{
    public class TenantContext : ITenantContext
    {
        public Guid EmpresaId { get; }

        public TenantContext(IHttpContextAccessor httpContextAccessor)
        {
            var claim = httpContextAccessor.HttpContext?.User?.FindFirst("empresa_id")?.Value;
            EmpresaId = Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }
    }
}
