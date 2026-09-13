using Microsoft.EntityFrameworkCore;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Application.Interfaces.Services;
using connectasys_api.Core.Domain.Entities;
using connectasys_api.Infrastructure.Persistence.Context;

namespace connectasys_api.Infrastructure.Persistence.Repositories
{
    public class ContaReceberRepository : IContaReceberRepository
    {
        private readonly AppDbContext _context;
        private readonly Guid _empresaId;

        public ContaReceberRepository(AppDbContext context, ITenantContext tenantContext)
        {
            _context = context;
            _empresaId = tenantContext.EmpresaId;
        }

        public async Task<List<ContaReceber>> GetAllAsync() =>
            await _context.ContasReceber.Where(c => c.EmpresaId == _empresaId).ToListAsync();

        public async Task<ContaReceber?> GetByIdAsync(int id) =>
            await _context.ContasReceber.FirstOrDefaultAsync(c => c.Id == id && c.EmpresaId == _empresaId);

        public async Task<List<ContaReceber>> GetByClienteIdAsync(int clienteId) =>
            await _context.ContasReceber
                .Where(c => c.ClienteId == clienteId && c.EmpresaId == _empresaId)
                .ToListAsync();

        public async Task<ContaReceber?> GetByOrdemServicoIdAsync(int ordemServicoId) =>
            await _context.ContasReceber
                .FirstOrDefaultAsync(c => c.OrdemServicoId == ordemServicoId && c.EmpresaId == _empresaId);

        public async Task AddAsync(ContaReceber contaReceber)
        {
            contaReceber.EmpresaId = _empresaId;
            _context.ContasReceber.Add(contaReceber);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ContaReceber contaReceber)
        {
            _context.ContasReceber.Update(contaReceber);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ContaReceber contaReceber)
        {
            _context.ContasReceber.Remove(contaReceber);
            await _context.SaveChangesAsync();
        }
    }
}
