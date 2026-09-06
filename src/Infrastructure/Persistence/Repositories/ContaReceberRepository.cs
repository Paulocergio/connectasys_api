using Microsoft.EntityFrameworkCore;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Domain.Entities;
using connectasys_api.Infrastructure.Persistence.Context;

namespace connectasys_api.Infrastructure.Persistence.Repositories
{
    public class ContaReceberRepository : IContaReceberRepository
    {
        private readonly AppDbContext _context;

        public ContaReceberRepository(AppDbContext context) => _context = context;

        public async Task<List<ContaReceber>> GetAllAsync() => await _context.ContasReceber.ToListAsync();

        public async Task<ContaReceber?> GetByIdAsync(int id) => await _context.ContasReceber.FindAsync(id);

        public async Task<List<ContaReceber>> GetByClienteIdAsync(int clienteId) =>
            await _context.ContasReceber.Where(c => c.ClienteId == clienteId).ToListAsync();

        public async Task<ContaReceber?> GetByOrdemServicoIdAsync(int ordemServicoId) =>
            await _context.ContasReceber.FirstOrDefaultAsync(c => c.OrdemServicoId == ordemServicoId);

        public async Task AddAsync(ContaReceber contaReceber)
        {
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
