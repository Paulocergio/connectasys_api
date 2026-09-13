using Microsoft.EntityFrameworkCore;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Domain.Entities;
using connectasys_api.Infrastructure.Persistence.Context;

namespace connectasys_api.Infrastructure.Persistence.Repositories
{
    // Sem escopo de tenant (ITenantContext) de propósito — Empresa é a
    // própria unidade de tenant, e é lida/criada antes de haver um tenant
    // ambiente (login, cadastro self-service).
    public class EmpresaRepository : IEmpresaRepository
    {
        private readonly AppDbContext _context;

        public EmpresaRepository(AppDbContext context) => _context = context;

        public async Task<Empresa?> GetByIdAsync(Guid id) => await _context.Empresas.FindAsync(id);

        public async Task AddAsync(Empresa empresa)
        {
            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();
        }

        public async Task CriarComPrimeiroUsuarioAsync(Empresa empresa, Usuario usuario)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            _context.Empresas.Add(empresa);
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        // Ordem respeita as foreign keys (filhos antes dos pais) — ver
        // design.md da spec multitenant pro mapa completo de FKs.
        public async Task ApagarTudoAsync(Guid empresaId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM itens_ordem_servico WHERE empresa_id = {empresaId}");
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM agendamentos WHERE empresa_id = {empresaId}");
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM contas_receber WHERE empresa_id = {empresaId}");
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM contas_pagar WHERE empresa_id = {empresaId}");
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM ordens_servico WHERE empresa_id = {empresaId}");
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM veiculos WHERE empresa_id = {empresaId}");
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM clientes WHERE empresa_id = {empresaId}");
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM estoque WHERE empresa_id = {empresaId}");
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM usuarios WHERE empresa_id = {empresaId}");
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM empresas WHERE id = {empresaId}");

            await transaction.CommitAsync();
        }
    }
}
