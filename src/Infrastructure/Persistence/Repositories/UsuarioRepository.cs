using Microsoft.EntityFrameworkCore;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Application.Interfaces.Services;
using connectasys_api.Core.Domain.Entities;
using connectasys_api.Infrastructure.Persistence.Context;

namespace connectasys_api.Infrastructure.Persistence.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;
        private readonly Guid _empresaId;

        public UsuarioRepository(AppDbContext context, ITenantContext tenantContext)
        {
            _context = context;
            _empresaId = tenantContext.EmpresaId;
        }

        public async Task<List<Usuario>> GetAllAsync() =>
            await _context.Usuarios.Where(u => u.EmpresaId == _empresaId).ToListAsync();

        public async Task<Usuario?> GetByIdAsync(Guid id) =>
            await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id && u.EmpresaId == _empresaId);

        // Sem escopo de empresa de propósito — usado no login, antes de
        // existir um tenant ambiente (é assim que descobrimos a empresa do
        // usuário: pelo e-mail, que é único no sistema inteiro).
        public async Task<Usuario?> GetByEmailAsync(string email) =>
            await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

        // Sem auto-stamp de EmpresaId (diferente dos outros repositórios) —
        // quem chama precisa já ter definido `usuario.EmpresaId` antes:
        // CreateUsuarioHandler usa a empresa do token atual; o cadastro
        // self-service (RegistrarEmpresaHandler) usa a empresa recém-criada,
        // sem tenant ambiente ainda.
        public async Task AddAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Usuario usuario)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }
    }
}
