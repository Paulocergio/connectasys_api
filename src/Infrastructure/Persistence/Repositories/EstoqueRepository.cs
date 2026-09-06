using Microsoft.EntityFrameworkCore;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Domain.Entities;
using connectasys_api.Infrastructure.Persistence.Context;

namespace connectasys_api.Infrastructure.Persistence.Repositories
{
    public class EstoqueRepository : IEstoqueRepository
    {
        private readonly AppDbContext _context;

        public EstoqueRepository(AppDbContext context) => _context = context;

        public async Task<List<Estoque>> GetAllAsync() => await _context.Estoque.ToListAsync();

        public async Task<Estoque?> GetByIdAsync(int id) => await _context.Estoque.FindAsync(id);

        public async Task AddAsync(Estoque estoque)
        {
            _context.Estoque.Add(estoque);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Estoque estoque)
        {
            _context.Estoque.Update(estoque);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Estoque estoque)
        {
            _context.Estoque.Remove(estoque);
            await _context.SaveChangesAsync();
        }
    }
}
