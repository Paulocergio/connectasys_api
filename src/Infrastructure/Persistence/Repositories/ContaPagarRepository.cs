using Microsoft.EntityFrameworkCore;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Application.Interfaces.Services;
using connectasys_api.Core.Domain.Entities;
using connectasys_api.Infrastructure.Persistence.Context;

namespace connectasys_api.Infrastructure.Persistence.Repositories
{
    public class ContaPagarRepository : IContaPagarRepository
    {
        private readonly AppDbContext _context;
        private readonly Guid _empresaId;

        public ContaPagarRepository(AppDbContext context, ITenantContext tenantContext)
        {
            _context = context;
            _empresaId = tenantContext.EmpresaId;
        }

        public async Task<List<ContaPagar>> GetAllAsync() =>
            await _context.ContasPagar.Where(c => c.EmpresaId == _empresaId).ToListAsync();

        public async Task<ContaPagar?> GetByIdAsync(int id) =>
            await _context.ContasPagar.FirstOrDefaultAsync(c => c.Id == id && c.EmpresaId == _empresaId);

        public async Task AddAsync(ContaPagar contaPagar)
        {
            contaPagar.EmpresaId = _empresaId;
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
