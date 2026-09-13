using Microsoft.EntityFrameworkCore;
using Npgsql;
using connectasys_api.Core.Application.Exceptions;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Application.Interfaces.Services;
using connectasys_api.Core.Domain.Entities;
using connectasys_api.Infrastructure.Persistence.Context;

namespace connectasys_api.Infrastructure.Persistence.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _context;
        private readonly Guid _empresaId;

        public ClienteRepository(AppDbContext context, ITenantContext tenantContext)
        {
            _context = context;
            _empresaId = tenantContext.EmpresaId;
        }

        public async Task<List<Cliente>> GetAllAsync() =>
            await _context.Clientes.Where(c => c.EmpresaId == _empresaId).ToListAsync();

        public async Task<Cliente?> GetByIdAsync(int id) =>
            await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id && c.EmpresaId == _empresaId);

        public async Task<Cliente?> GetByCpfAsync(string cpf) =>
            await _context.Clientes.FirstOrDefaultAsync(c => c.Cpf == cpf && c.EmpresaId == _empresaId);

        public async Task<Cliente?> GetByCnpjAsync(string cnpj) =>
            await _context.Clientes.FirstOrDefaultAsync(c => c.Cnpj == cnpj && c.EmpresaId == _empresaId);

        public async Task AddAsync(Cliente cliente)
        {
            cliente.EmpresaId = _empresaId;
            _context.Clientes.Add(cliente);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (EhViolacaoDeDocumentoDuplicado(ex))
            {
                throw new DocumentoDuplicadoException();
            }
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (EhViolacaoDeDocumentoDuplicado(ex))
            {
                throw new DocumentoDuplicadoException();
            }
        }

        private static bool EhViolacaoDeDocumentoDuplicado(DbUpdateException ex) =>
            ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };

        public async Task DeleteAsync(Cliente cliente)
        {
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
        }
    }
}
