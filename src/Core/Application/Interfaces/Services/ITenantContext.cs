namespace connectasys_api.Core.Application.Interfaces.Services
{
    // Empresa (tenant) do usuário autenticado na requisição atual — lido do
    // claim "empresa_id" do JWT. Repositórios injetam isso pra filtrar toda
    // consulta pela empresa certa, sem precisar passar EmpresaId manualmente
    // em cada Command/Query. Ver Infrastructure/Security/TenantContext.cs.
    public interface ITenantContext
    {
        Guid EmpresaId { get; }
    }
}
