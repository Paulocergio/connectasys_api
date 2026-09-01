using Microsoft.EntityFrameworkCore;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Domain.Entities;
using connectasys_api.Infrastructure.Persistence.Context;

namespace connectasys_api.Infrastructure.Persistence.Repositories
{
    public class ContaPagarRepository : IContaPagarRepository
    {
        private readonly AppDbContext _context;

        public ContaPagarRepository(AppDbContext context) => _context = context;

        public async Task<List<ContaPagar>> GetAllAsync() => await _context.ContasPagar.ToListAsync();

        public async Task<ContaPagar?> GetByIdAsync(int id) => await _context.ContasPagar.FindAsync(id);

        public async Task AddAsync(ContaPagar contaPagar)
        {
            _context.ContasPagar.Add(contaPagar);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ContaPagar contaPagar)
        {
            _context.ContasPagar.Update(contaPagar);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ContaPagar contaPagar)
        {
            _context.ContasPagar.Remove(contaPagar);
            await _context.SaveChangesAsync();
        }
    }
}
