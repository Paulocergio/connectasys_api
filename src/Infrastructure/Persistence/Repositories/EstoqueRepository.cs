using Microsoft.EntityFrameworkCore;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Application.Interfaces.Services;
using connectasys_api.Core.Domain.Entities;
using connectasys_api.Infrastructure.Persistence.Context;

namespace connectasys_api.Infrastructure.Persistence.Repositories
{
    public class EstoqueRepository : IEstoqueRepository
    {
        private readonly AppDbContext _context;
        private readonly Guid _empresaId;

        public EstoqueRepository(AppDbContext context, ITenantContext tenantContext)
        {
            _context = context;
            _empresaId = tenantContext.EmpresaId;
        }

        public async Task<List<Estoque>> GetAllAsync() =>
            await _context.Estoque.Where(e => e.EmpresaId == _empresaId).ToListAsync();

        public async Task<Estoque?> GetByIdAsync(int id) =>
            await _context.Estoque.FirstOrDefaultAsync(e => e.Id == id && e.EmpresaId == _empresaId);

        public async Task AddAsync(Estoque estoque)
        {
            estoque.EmpresaId = _empresaId;
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
