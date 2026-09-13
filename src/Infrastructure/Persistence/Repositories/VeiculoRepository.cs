using Microsoft.EntityFrameworkCore;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Application.Interfaces.Services;
using connectasys_api.Core.Domain.Entities;
using connectasys_api.Infrastructure.Persistence.Context;

namespace connectasys_api.Infrastructure.Persistence.Repositories
{
    public class VeiculoRepository : IVeiculoRepository
    {
        private readonly AppDbContext _context;
        private readonly Guid _empresaId;

        public VeiculoRepository(AppDbContext context, ITenantContext tenantContext)
        {
            _context = context;
            _empresaId = tenantContext.EmpresaId;
        }

        public async Task<List<Veiculo>> GetAllAsync() =>
            await _context.Veiculos.Where(v => v.EmpresaId == _empresaId).ToListAsync();

        public async Task<Veiculo?> GetByIdAsync(int id) =>
            await _context.Veiculos.FirstOrDefaultAsync(v => v.Id == id && v.EmpresaId == _empresaId);

        public async Task<List<Veiculo>> GetByClienteIdAsync(int clienteId) =>
            await _context.Veiculos
                .Where(v => v.ClienteId == clienteId && v.EmpresaId == _empresaId)
                .ToListAsync();

        public async Task AddAsync(Veiculo veiculo)
        {
            veiculo.EmpresaId = _empresaId;
            _context.Veiculos.Add(veiculo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Veiculo veiculo)
        {
            _context.Veiculos.Update(veiculo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Veiculo veiculo)
        {
            _context.Veiculos.Remove(veiculo);
            await _context.SaveChangesAsync();
        }
    }
}
