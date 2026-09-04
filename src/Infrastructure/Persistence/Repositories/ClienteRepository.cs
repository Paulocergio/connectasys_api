using Microsoft.EntityFrameworkCore;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Domain.Entities;
using connectasys_api.Infrastructure.Persistence.Context;

namespace connectasys_api.Infrastructure.Persistence.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _context;

        public ClienteRepository(AppDbContext context) => _context = context;

        public async Task<List<Cliente>> GetAllAsync() => await _context.Clientes.ToListAsync();

        public async Task<Cliente?> GetByIdAsync(int id) => await _context.Clientes.FindAsync(id);

        public async Task<Cliente?> GetByCpfAsync(string cpf) =>
            await _context.Clientes.FirstOrDefaultAsync(c => c.Cpf == cpf);

        public async Task<Cliente?> GetByCnpjAsync(string cnpj) =>
            await _context.Clientes.FirstOrDefaultAsync(c => c.Cnpj == cnpj);

        public async Task AddAsync(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Cliente cliente)
        {
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
        }
    }
}
