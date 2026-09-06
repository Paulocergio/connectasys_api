using Microsoft.EntityFrameworkCore;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Domain.Entities;
using connectasys_api.Infrastructure.Persistence.Context;

namespace connectasys_api.Infrastructure.Persistence.Repositories
{
    public class OrdemServicoRepository : IOrdemServicoRepository
    {
        private readonly AppDbContext _context;

        public OrdemServicoRepository(AppDbContext context) => _context = context;

        public async Task<List<OrdemServico>> GetAllAsync() =>
            await _context.OrdensServico.Include(o => o.Itens).ToListAsync();

        public async Task<OrdemServico?> GetByIdAsync(int id) =>
            await _context.OrdensServico.Include(o => o.Itens).FirstOrDefaultAsync(o => o.Id == id);

        public async Task<List<OrdemServico>> GetByClienteIdAsync(int clienteId) =>
            await _context.OrdensServico.Include(o => o.Itens).Where(o => o.ClienteId == clienteId).ToListAsync();

        public async Task<List<OrdemServico>> GetByVeiculoIdAsync(int veiculoId) =>
            await _context.OrdensServico.Include(o => o.Itens).Where(o => o.VeiculoId == veiculoId).ToListAsync();

        public async Task AddAsync(OrdemServico ordemServico)
        {
            _context.OrdensServico.Add(ordemServico);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OrdemServico ordemServico)
        {
            _context.OrdensServico.Update(ordemServico);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(OrdemServico ordemServico)
        {
            _context.OrdensServico.Remove(ordemServico);
            await _context.SaveChangesAsync();
        }

        public async Task<ItemOrdemServico?> GetItemByIdAsync(int itemId) =>
            await _context.ItensOrdemServico.FindAsync(itemId);

        public async Task AddItemAsync(ItemOrdemServico item)
        {
            _context.ItensOrdemServico.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(ItemOrdemServico item)
        {
            _context.ItensOrdemServico.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task SalvarComContaReceberAsync(
            OrdemServico ordemServico,
            ContaReceber? contaReceberNova,
            ContaReceber? contaReceberParaAtualizar,
            ContaReceber? contaReceberParaRemover)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            _context.OrdensServico.Update(ordemServico);
            if (contaReceberNova is not null)
                _context.ContasReceber.Add(contaReceberNova);
            if (contaReceberParaAtualizar is not null)
                _context.ContasReceber.Update(contaReceberParaAtualizar);
            if (contaReceberParaRemover is not null)
                _context.ContasReceber.Remove(contaReceberParaRemover);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
    }
}
